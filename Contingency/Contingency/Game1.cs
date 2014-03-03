using System;
using System.Collections.Generic;
using System.Threading;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Contingency
{
    public partial class Game1
    {
        public const string ClientIp = "192.168.137.1";
        public const string ServerIp = "192.168.137.1";

        public bool Server;
        private const int TotalPlayTime = 5000;
        private readonly ButtonList _buttons = new ButtonList();
        private readonly Menu _menu = new Menu();
        private GameState _gameState = new GameState();
        private bool _inPlanningMode = true;
        private Texture2D _lineTexture;
        private bool _menuMode = true;
        private Dictionary<string, Vector2> _messages = new Dictionary<string, Vector2>();
        private MouseState _mouseStateCurrent, _mouseStatePrevious;
        private int _remainingPlayTime = TotalPlayTime;
        private SpriteBatch _spriteBatch;
        private Thread _waitJoin;
        private string CurrentTeam = "red";

        private Rectangle _view;

        private Vector2 ViewOffset
        {
            get
            {
                return new Vector2(_view.X, _view.Y);
            }
        }

        private Rectangle _map;

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

                Special special = new Special(i%2 == 0 ? "build" : "blink", 1000, 200);

                _gameState.Units[_gameState.Units.Count - 2].Special = special;
                _gameState.Units[_gameState.Units.Count - 1].Special = special;
            }

            GenerateStage(800, 5, 7, 10, 10); // big blocks
            GenerateStage(800, 3, 4, 6, 6); // medium blocks
            GenerateStage(800, 0, 0, 3, 3); // small blocks
        }

        protected override void UnloadContent()
        {
            SocketManager.Stop = true;
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
            Server = false;
            CurrentTeam = "blue";

            StartListener(12000);

            _messages.Add("Joining server: 11000", new Vector2(100, 100));

            
            _view = new Rectangle((_map.Width - _view.Width) * -1, 0, _view.Width,_view.Height);
            _waitJoin = new Thread(() => JoinServer(ServerIp));
            _waitJoin.Start();

            ToggleMenuMode(false);
        }

        private void StartGame()
        {
            Server = true;

            CreateStage();

            StartListener(11000);

            _messages.Add("Waiting for clients on: 11000", new Vector2(100, 100));

            _waitJoin = new Thread(WaitForOpponentToJoin);
            _waitJoin.Start();

            ToggleMenuMode(false);
        }

        private void StartListener(int port)
        {
            Thread listener = new Thread(() => SocketManager.StartListening(ClientIp, port));
            listener.Start();
            SocketManager.DataRecieved += SocketManager_DataRecieved;
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