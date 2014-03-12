using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Contingency.GameDataService;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Contingency
{
    public partial class Game1
    {
        public GameDataServiceSoapClient DataService = new GameDataServiceSoapClient();
        public Thread WaitThread;
        private const int TotalPlayTime = 5000;
        private readonly ButtonList _buttons = new ButtonList();
        private readonly Menu _menu = new Menu();
        private string _currentGameId = "1";
        private GameState _gameState;
        private bool _inPlanningMode = true;
        private Texture2D _lineTexture;
        private Rectangle _map;
        private bool _menuMode = true;
        private Dictionary<string, Vector2> _messages = new Dictionary<string, Vector2>();
        private MouseState _mouseStateCurrent, _mouseStatePrevious;
        private string _playerId = Environment.MachineName;
        private int _remainingPlayTime = TotalPlayTime;
        private SpriteBatch _spriteBatch;
        private Rectangle _view;
        private Thread _waitJoin;
        private string CurrentTeam = "red";

        public Game1()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };
            graphics.ApplyChanges();

            _view = new Rectangle(0, 0, 800, 600);
            _map = new Rectangle(0, 0, 1920, 1080);

            Content.RootDirectory = "Content";
        }

        private Vector2 ViewOffset
        {
            get
            {
                return new Vector2(_view.X, _view.Y);
            }
        }

        public void WaitForOppent()
        {
            while (true)
            {
                XmlNode tempState = DataService.GetRefreshedData(_currentGameId, _playerId);
                while (tempState == null)
                {
                    Thread.Sleep(100);
                }

                _gameState = GameState.Deserialize(tempState);
                _messages.Remove("Waiting for opponent");

                _inPlanningMode = false;
                _remainingPlayTime = 5000;

                break;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            _mouseStateCurrent = Mouse.GetState();
            _mouseStatePrevious = Mouse.GetState();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteList.LoadSprites(Content);

            _lineTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = new Color[_lineTexture.Width * _lineTexture.Height];

            _lineTexture.GetData(data);
            data[0] = Color.White;
            _lineTexture.SetData(data);

            _buttons.Add(new Button("EndTurn", 90, 25, "End Turn", new Vector2(0, 0), Color.LimeGreen, Color.Black, GraphicsDevice));
            _buttons["EndTurn"].Visible = false;
            _buttons["EndTurn"].ButtonClicked += EndTurnClicked;

            _buttons.Add(new Button("Start", 200, 100, "Start Server", new Vector2(100, 100), Color.LimeGreen, Color.Black, GraphicsDevice));
            _buttons["Start"].ButtonClicked += (sender, args) => StartGame();

            _buttons.Add(new Button("Join", 200, 100, "Join Server", new Vector2(100, 250), Color.LimeGreen, Color.Black, GraphicsDevice));
            _buttons["Join"].ButtonClicked += (sender, args) => JoinGame();
        }

        private void CreateStage()
        {
            // spawn units
            for (int i = 0; i < 10; i++)
            {
                Unit red = new Unit(20, 20, 15, 50 + i * 55, 50, "red");
                red.CurrentAngle = red.CurrentAngle + (float)Math.PI;
                red.TargetAngle = red.CurrentAngle;

                _gameState.Units.Add(red);
                _gameState.Units.Add(new Unit(20, 20, _map.Width - 15, 50 + i * 55, 50, "blue"));

                Special special = new Special(i % 2 == 0 ? "build" : "blink", 1000, 200);

                _gameState.Units[_gameState.Units.Count - 2].Special = special;
                _gameState.Units[_gameState.Units.Count - 1].Special = special;
            }

            GenerateStage(800, 5, 7, 10, 10); // big blocks
            GenerateStage(800, 3, 4, 6, 6); // medium blocks
            GenerateStage(800, 0, 0, 3, 3); // small blocks
        }

        private void EndTurnClicked(object sender, EventArgs e)
        {
            _messages.Add("Waiting for opponent", new Vector2(100, 100));
            DataService.SendState(_currentGameId, (XmlElement)_gameState.Serialize(), _playerId, CurrentTeam);

            if (!DataService.RefreshAvailable(_currentGameId, _playerId))
            {
                WaitThread = new Thread(WaitForOppent);
                WaitThread.Start();
            }
        }

        private void GenerateStage(int maxBlocks, int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            int currentBlockCounter = 0;
            while (currentBlockCounter < maxBlocks)
            {
                int x = Helper.Rand.Next(0, _map.Width);
                int y = Helper.Rand.Next(0, _map.Height);

                Vector2 v = new Vector2(x, y);
                int w = Helper.Rand.Next(minWidth, maxWidth);
                int h = Helper.Rand.Next(minHeight, maxHeight);

                bool invalid = false;
                foreach (Unit u in _gameState.Units)
                {
                    if (u.Touches(v, w * 10) || u.Touches(v, h * 10))
                    {
                        invalid = true;
                        break;
                    }
                }

                if (!invalid)
                {
                    foreach (Block b in _gameState.Blocks)
                    {
                        if (b.Touches(v, w * 10) || b.Touches(v, h * 10))
                        {
                            invalid = true;
                            break;
                        }
                    }
                }

                if (!invalid)
                {
                    for (int width = 0; width < w; width++)
                    {
                        for (int height = 0; height < h; height++)
                        {
                            Vector2 location = new Vector2(v.X + width * 10, v.Y + height * 10);
                            _gameState.Blocks.Add(new Block(location));

                            currentBlockCounter++;
                        }
                    }
                }
            }
        }

        private void JoinGame()
        {
            if (_gameState != null && !DataService.NeedsTurn(_currentGameId, _playerId, CurrentTeam))
            {
                WaitThread = new Thread(WaitForOppent);
                WaitThread.Start();

                return;
            }

            var tempState = DataService.GetState(_currentGameId, _playerId);

            if (tempState == null)
            {
                return;
            }

            _gameState = GameState.Deserialize(tempState);

            CurrentTeam = DataService.GetTeam(_currentGameId, _playerId);

            ToggleMenuMode(false);
        }

        private void StartGame()
        {
            CreateStage();
            DataService.SendState(_currentGameId, (XmlElement)_gameState.Serialize(), _playerId, CurrentTeam);

            ToggleMenuMode(false);
        }

        private void ToggleMenuMode(bool enabled)
        {
            _buttons["EndTurn"].Visible = !enabled;
            _buttons["Start"].Visible = enabled;
            _buttons["Join"].Visible = enabled;
            _menuMode = enabled;
        }
    }
}