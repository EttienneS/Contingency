using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Contingency
{
    public class Game1 : Game
    {
        private GameState _gameState = new GameState();
        private GraphicsDeviceManager _graphics;
        private Texture2D _lineTexture;
        private readonly Menu _menu = new Menu();
        private MouseState _mouseStateCurrent, _mouseStatePrevious;
        private bool _paused = true;
        private SpriteBatch _spriteBatch;
        private const int TotalPlanningTime = 15000;
        private const int TotalPlayTime = 5000;
        private int _remainingPlanningTime = TotalPlanningTime;
        private int _remainingPlayTime = TotalPlayTime;
        private bool _menuMode = true;
        private const string CurrentTeam = "red";

        #region Init

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
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

            // spawn units
            for (int i = 0; i < 10; i++)
            {
                Unit red = new Unit(20, 20, 15, 50 + i * 40, 50, "red");
                red.CurrentAngle = red.CurrentAngle + (float)Math.PI;
                red.TargetAngle = red.CurrentAngle;

                GameState.Units.Add(red);
                GameState.Units.Add(new Unit(20, 20, GraphicsDevice.Viewport.Width - 15, 50 + i * 40, 50, "blue"));
            }

            while (GameState.Blocks.Count < 400)
            {
                int x = Helper.Rand.Next(0, GraphicsDevice.Viewport.Width);
                int y = Helper.Rand.Next(0, GraphicsDevice.Viewport.Height);

                Vector2 v = new Vector2(x, y);
                int w = Helper.Rand.Next(0, 8);
                int h = Helper.Rand.Next(0, 8);

                bool invalid = false;
                foreach (Unit u in GameState.Units)
                {
                    if (u.Touches(v, w * 10) || u.Touches(v, h * 10))
                    {
                        invalid = true;
                    }
                }
                foreach (Block b in GameState.Blocks)
                {
                    if (b.Touches(v, w * 10) || b.Touches(v, h * 10))
                    {
                        invalid = true;
                    }
                }

                if (!invalid)
                {
                    for (int width = 0; width < w; width++)
                    {
                        for (int height = 0; height < h; height++)
                        {
                            Vector2 location = new Vector2(v.X + width * 10, v.Y + height * 10);
                            GameState.Blocks.Add(new Block(location));
                        }
                    }
                }
            }

            Thread listener = new Thread(SocketManager.StartListening);
            listener.Start();
            SocketManager.DataRecieved += SocketManager_DataRecieved;
        }

        protected override void UnloadContent()
        {
        }

        #endregion Init

        #region State Load/Save/IO

        private MemoryStream SaveState()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, _gameState);

            return stream;
        }

        private GameState LoadState(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return (GameState)new BinaryFormatter().Deserialize(stream);
        }

        private void SocketManager_DataRecieved(object sender, EventArgs e)
        {
            MemoryStream stream = new MemoryStream(((GameStateDataEventArgs)e).Data);
            _gameState = LoadState(stream);
        }

        #endregion State Load/Save/IO

        #region Drawing

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (!_menuMode)
            {
                DrawBlocks();

                DrawProjectiles();

                DrawUnits();

                DrawExplosions();

                DrawText();

                DrawMenu();
            }
            else
            {
                _spriteBatch.Draw(SpriteList.ContentSprites["start"], new Vector2(100, 100), Color.White);
                _spriteBatch.Draw(SpriteList.ContentSprites["join"], new Vector2(100, 200), Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawBlocks()
        {
            foreach (Block b in GameState.Blocks)
            {
                if (b != null)
                    _spriteBatch.Draw(b.GetSprite(), b.Location, null, Color.White, b.CurrentAngle, new Vector2(b.Width / 2, b.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawExplosions()
        {
            foreach (Explosion exp in GameState.Explosions)
            {
                if (exp != null)
                    _spriteBatch.Draw(exp.SpriteSheet, new Rectangle((int)exp.Location.X - 32, (int)exp.Location.Y - 32, exp.SpriteWidth, exp.SpriteHeight), exp.SpriteRect, Color.White);
            }
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, int width)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(_lineTexture, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), width), null, color, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        private void DrawMenu()
        {
            if (_menu.Visible)
            {
                _spriteBatch.Draw(_menu.GetSprite(), _menu.Location, Color.White);

                Dictionary<string, Rectangle> quads = _menu.Quads;
                _spriteBatch.DrawString(SpriteList.Font, "A", new Vector2(quads["Attack"].X + 15, quads["Attack"].Y + 6), Color.Black);
                _spriteBatch.DrawString(SpriteList.Font, "M", new Vector2(quads["Move"].X + 3, quads["Move"].Y + 6), Color.Black);
                _spriteBatch.DrawString(SpriteList.Font, "S", new Vector2(quads["Stop"].X + 15, quads["Stop"].Y), Color.Black);
                _spriteBatch.DrawString(SpriteList.Font, "X", new Vector2(quads["Special"].X + 3, quads["Special"].Y), Color.Black);
            }
        }

        private void DrawProjectiles()
        {
            foreach (Projectile sprite in GameState.Projectiles)
            {
                if (sprite != null)
                    _spriteBatch.Draw(sprite.GetSprite(), sprite.Location, null, Color.White, sprite.CurrentAngle, new Vector2(sprite.Width / 2, sprite.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawText()
        {
            if (_paused)
            {
                _spriteBatch.DrawString(SpriteList.Font, TimeSpan.FromMilliseconds(_remainingPlanningTime).ToString(), new Vector2(300, 0), Color.LimeGreen);
            }
            else
            {
                _spriteBatch.DrawString(SpriteList.Font, TimeSpan.FromMilliseconds(_remainingPlayTime).ToString(), new Vector2(300, 0), Color.Red);
            }
        }

        private void DrawUnits()
        {
            foreach (Unit u in GameState.Units)
            {
                if (u != null)
                {
                    _spriteBatch.Draw(u.GetSprite(), u.Location, null, Color.White, u.CurrentAngle, new Vector2(u.Width / 2, u.Height / 2), 1.0f, SpriteEffects.None, 0f);

                    Vector2 startPoint = new Vector2(u.Location.X - u.Width / 2, u.Location.Y - 15);
                    Vector2 endPoint = new Vector2(startPoint.X + u.Width, u.Location.Y - 15);
                    Vector2 endPointHp = new Vector2(startPoint.X + (u.Width * (u.CurrentHP / (float)u.MaxHP)),
                        u.Location.Y - 15);

                    DrawLine(_spriteBatch, startPoint, endPoint, Color.DarkRed, 3);
                    DrawLine(_spriteBatch, startPoint, endPointHp, Color.LimeGreen, 3);
                }
            }

            foreach (Unit u in GameState.Units)
            {
                for (int i = 0; i < u.OrderQueue.Count; i++)
                {
                    Order o = u.OrderQueue[i];
                    Texture2D sprite = SpriteList.ContentSprites["targetAttack"];
                    Color color = Color.Red;

                    if (o.Type == OrderType.Move)
                    {
                        sprite = SpriteList.ContentSprites["targetMove"];
                        color = Color.LimeGreen;
                    }

                    Vector2 source = u.Location;

                    if (i > 0)
                    {
                        Vector2 lastMoveLocation = u.Location;
                        for (int l = 0; l < i; l++)
                        {
                            if (u.OrderQueue[l].Type == OrderType.Move)
                            {
                                lastMoveLocation = u.OrderQueue[l].Target;
                            }
                        }
                        source = lastMoveLocation;
                        source += new Vector2(5f);
                    }

                    _spriteBatch.Draw(sprite, o.Target, null, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                    DrawLine(_spriteBatch, source, o.Target + new Vector2(5), color, 2);
                }
            }
        }

        #endregion Drawing

        #region Porcess Input

        private void CatchInputs()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // save / load example
                MemoryStream state = SaveState();
                state.Seek(0, SeekOrigin.Begin);
                using (FileStream file = new FileStream(@"c:\file.bin", FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = new byte[state.Length];
                    state.Read(bytes, 0, (int)state.Length);
                    file.Write(bytes, 0, bytes.Length);
                    //state.Close();
                }
                _gameState = new GameState();
                _gameState = LoadState(state);

                Exit();
            }

            _mouseStateCurrent = Mouse.GetState();

            if (!_menuMode)
            {
                if (_mouseStateCurrent.LeftButton == ButtonState.Pressed && _mouseStatePrevious.LeftButton == ButtonState.Released)
                {
                    MouseClickedGame();
                }
                else if (_mouseStateCurrent.RightButton == ButtonState.Pressed && _mouseStatePrevious.RightButton == ButtonState.Released)
                {
                    Unit selected = GetSelctedUnit();

                    if (selected != null)
                    {
                        selected.Selected = false;
                        _menu.Visible = false;
                    }
                }
            }
            else
            {
                if (_mouseStateCurrent.LeftButton == ButtonState.Pressed && _mouseStatePrevious.LeftButton == ButtonState.Released)
                {
                    MouseClickedMenu();
                }
            }

            _mouseStatePrevious = _mouseStateCurrent;
        }

        private void MouseClickedMenu()
        {
            _menuMode = false;
        }

        private void MouseClickedGame()
        {
            Unit selectedUnit = GetSelctedUnit();

            if (selectedUnit == null)
            {
                foreach (Unit u in GameState.Units)
                {
                    if (u.Touches(new Vector2(_mouseStateCurrent.X, _mouseStateCurrent.Y), 2.0) && u.Team == CurrentTeam)
                    {
                        u.Selected = !u.Selected;
                        break;
                    }
                }
            }
            else
            {
                Vector2 mouseVector = new Vector2(_mouseStateCurrent.X, _mouseStateCurrent.Y);
                if (!_menu.Visible)
                {
                    _menu.Visible = true;
                    _menu.Location = new Vector2(_mouseStateCurrent.X - _menu.Width / 2, _mouseStateCurrent.Y - _menu.Height / 2);
                }
                else
                {
                    if (_menu.TouchesWithOffset(mouseVector, 1.0, _menu.Width / 2))
                    {
                        switch (_menu.GetMenuSelection(mouseVector))
                        {
                            case "Attack":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Attack, mouseVector));
                                break;

                            case "Move":
                                selectedUnit.OrderQueue.Add(new Order(OrderType.Move, mouseVector));
                                break;

                            case "Special":
                                selectedUnit.OrderQueue.Clear();
                                break;

                            case "Stop":
                                Vector2 target = selectedUnit.Location;

                                if (selectedUnit.OrderQueue.Count > 1)
                                {
                                    target = selectedUnit.OrderQueue.Last().Target;
                                }
                                selectedUnit.OrderQueue.Add(new Order(OrderType.None, target));
                                break;
                        }
                    }
                }
            }
        }

        #endregion Porcess Input

        #region State Updates

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_paused)
            {
                CatchInputs();
                if (_remainingPlanningTime <= 0)
                {
                    _paused = false;
                    _remainingPlanningTime = 0;
                    _remainingPlayTime = TotalPlayTime;
                }
                else
                {
                    _remainingPlanningTime -= gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            else
            {
                if (_remainingPlayTime <= 0)
                {
                    _paused = true;
                    _remainingPlayTime = 0;
                    _remainingPlanningTime = TotalPlanningTime;
                }
                else
                {
                    _remainingPlayTime -= gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            if (!_menuMode)
            {
                if (_paused)
                    return;

                UpdateExplosions(gameTime);

                UpdateUnits(gameTime);

                UpdateProjectiles();

                UpdateDeaths();
            }
        }

        private static void UpdateDeaths()
        {
            for (int i = 0; i < GameState.Units.Count; i++)
            {
                if (GameState.Units[i].CurrentHP <= 0)
                {
                    GameState.Explosions.Add(new Explosion(new Vector2(GameState.Units[i].Location.X - GameState.Units[i].Width / 2, GameState.Units[i].Location.Y - GameState.Units[i].Height / 2)));
                    GameState.Units.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < GameState.Blocks.Count; i++)
            {
                if (GameState.Blocks[i].CurrentHP <= 0)
                {
                    GameState.Explosions.Add(new Explosion(new Vector2(GameState.Blocks[i].Location.X - GameState.Blocks[i].Width / 2, GameState.Blocks[i].Location.Y - GameState.Blocks[i].Height / 2)));
                    GameState.Blocks.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void UpdateExplosions(GameTime gameTime)
        {
            for (int x = 0; x < GameState.Explosions.Count; x++)
            {
                Explosion exp = GameState.Explosions[x];
                if (exp.Done)
                {
                    GameState.Explosions.Remove(exp);
                    x--;
                }
                else
                {
                    exp.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                }
            }
        }

        private void UpdateProjectiles()
        {
            List<Projectile> killSprites = new List<Projectile>();

            foreach (Projectile p in GameState.Projectiles)
            {
                if (p.Location.X < 0 || p.Location.X > GraphicsDevice.Viewport.Width || p.Location.Y > GraphicsDevice.Viewport.Height || p.Location.Y < 0 || (p.Momentum.X == 0 && p.Momentum.Y == 0))
                {
                    killSprites.Add(p);
                    break;
                }
                p.UpdateState();

                bool hitUnit = false;
                foreach (Unit u in GameState.Units)
                {
                    if (u.Touches(p) && p.Owner != u)
                    {
                        u.Hit(p);
                        hitUnit = true;
                        break;
                    }
                }

                if (!hitUnit)
                {
                    foreach (Block b in GameState.Blocks)
                    {
                        if (b.Touches(p) && p.Owner != b.Owner)
                        {
                            b.Hit(p);
                            break;
                        }
                    }
                }
            }

            foreach (Projectile x in killSprites)
            {
                GameState.Projectiles.Remove(x);
            }
        }

        private void UpdateUnits(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (Unit u in GameState.Units)
            {
                u.ReloadGun(elapsed);

                switch (u.CurrentOrder.Type)
                {
                    case OrderType.None:
                        u.OrderComplete();
                        break;

                    case OrderType.Move:
                        u.Target(u.CurrentOrder.Target);

                        if (u.CurrentAngle == u.TargetAngle)
                        {
                            u.Momentum = new Vector2((float)Math.Cos(u.TargetAngle) * -1, (float)Math.Sin(u.TargetAngle) * -1);
                        }
                        else
                        {
                            u.Momentum = new Vector2(0f);
                        }

                        if (u.CurrentAngle == u.TargetAngle && Sprite.AlmostEquals(u.Location.X, u.CurrentOrder.Target.X, 2) && Sprite.AlmostEquals(u.Location.Y, u.CurrentOrder.Target.Y, 2))
                        {
                            u.Location = u.CurrentOrder.Target;
                            u.Momentum = new Vector2(0f);
                            u.OrderComplete();
                        }

                        if (u.Momentum.X > 0f || u.Momentum.Y > 0f)
                        {
                            foreach (Block b in GameState.Blocks)
                            {
                                if (u.Touches(b))
                                {
                                    u.Momentum = new Vector2(0f);
                                    break;
                                }
                            }
                        }

                        break;

                    case OrderType.Attack:
                        u.Momentum = new Vector2(0f);
                        u.Target(u.CurrentOrder.Target);

                        if (u.CurrentAngle == u.TargetAngle)
                        {
                            u.Shoot(ref GameState.Projectiles);
                        }

                        if (u.ShotCount == 5)
                        {
                            u.ShotCount = 0;
                            u.OrderComplete();
                        }
                        break;
                }

                u.UpdateState();
            }
        }

        #endregion State Updates

        #region Helpers

        private static Unit GetSelctedUnit()
        {
            foreach (Unit unit in GameState.Units)
            {
                if (unit.Selected)
                {
                    return unit;
                }
            }

            return null;
        }

        #endregion Helpers
    }
}