using System;
using System.Collections.Generic;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Contingency
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<Explosion> _explosions = new List<Explosion>();
        private readonly List<Unit> _units = new List<Unit>();
        private GraphicsDeviceManager _graphics;
        private Texture2D _lineTexture;
        private Menu _menu = new Menu();
        private MouseState _mouseStateCurrent, _mouseStatePrevious;
        private bool _paused = true;
        private List<Projectile> _projectiles = new List<Projectile>();
        private SpriteBatch _spriteBatch;

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
                Unit red = new Unit(20, 20, 40, 50 + i * 40, SpriteList.ContentSprites["unitRed"], SpriteList.ContentSprites["unitRedSelected"], 50, "red", SpriteList.ContentSprites["projectileRed"]);

                red.CurrentAngle = red.CurrentAngle + (float)Math.PI;
                red.TargetAngle = red.CurrentAngle;
                _units.Add(red);
                _units.Add(new Unit(20, 20, GraphicsDevice.Viewport.Width - 60, 50 + i * 40, SpriteList.ContentSprites["unitBlue"], SpriteList.ContentSprites["unitBlueSelected"], 50, "blue", SpriteList.ContentSprites["projectileBlue"]));
            }

            for (int i = 0; i < 40; i++)
            {
                _blocks.Add(new Block(new Vector2(80, 40 + i * 10)));
                _blocks.Add(new Block(new Vector2(GraphicsDevice.Viewport.Width - 100, 40 + i * 10)));
            }
        }

        protected override void UnloadContent()
        {
        }

        #endregion Init

        #region Drawing

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            DrawMenu();

            DrawText();

            DrawBlocks();

            DrawProjectiles();

            DrawUnits();

            DrawExplosions();

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawBlocks()
        {
            foreach (Block b in _blocks)
            {
                _spriteBatch.Draw(b.GetSprite(), b.Location, null, Color.White, b.CurrentAngle, new Vector2(b.Width / 2, b.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawExplosions()
        {
            foreach (Explosion exp in _explosions)
            {
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
            foreach (Projectile sprite in _projectiles)
            {
                _spriteBatch.Draw(sprite.GetSprite(), sprite.Location, null, Color.White, sprite.CurrentAngle, new Vector2(sprite.Width / 2, sprite.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawText()
        {
            _spriteBatch.DrawString(SpriteList.Font, "Red", new Vector2(0, 0), Color.White);
        }

        private void DrawUnits()
        {
            foreach (Unit u in _units)
            {
                _spriteBatch.Draw(u.GetSprite(), u.Location, null, Color.White, u.CurrentAngle, new Vector2(u.Width / 2, u.Height / 2), 1.0f, SpriteEffects.None, 0f);

                Vector2 startPoint = new Vector2(u.Location.X - u.Width / 2, u.Location.Y - 15);
                Vector2 endPoint = new Vector2(startPoint.X + u.Width, u.Location.Y - 15);
                Vector2 endPointHp = new Vector2(startPoint.X + (u.Width * ((float)u.CurrentHP / (float)u.MaxHP)), u.Location.Y - 15);

                DrawLine(_spriteBatch, startPoint, endPoint, Color.DarkRed, 3);
                DrawLine(_spriteBatch, startPoint, endPointHp, Color.LimeGreen, 3);
            }

            foreach (Unit u in _units)
            {
                if (u.CurrentOrder.Type != OrderType.None)
                {
                    Texture2D sprite = SpriteList.ContentSprites["targetAttack"];
                    Color color = Color.Red;

                    if (u.CurrentOrder.Type == OrderType.Move)
                    {
                        sprite = SpriteList.ContentSprites["targetMove"];
                        color = Color.LimeGreen;
                    }

                    _spriteBatch.Draw(sprite, u.CurrentOrder.Target, null, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                    DrawLine(_spriteBatch, u.Location, u.CurrentOrder.Target + new Vector2(5), color, 2);
                }
            }
        }

        #endregion Drawing

        #region Porcess Input

        private void CatchInputs()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _paused = false;
            }
            else
            {
                _paused = true;
            }

            _mouseStateCurrent = Mouse.GetState();

            if (_mouseStateCurrent.LeftButton == ButtonState.Pressed && _mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                MouseClicked();
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

            _mouseStatePrevious = _mouseStateCurrent;
        }

        private void MouseClicked()
        {
            Unit selectedUnit = GetSelctedUnit();

            if (selectedUnit == null)
            {
                foreach (Unit u in _units)
                {
                    if (u.Touches(new Vector2(_mouseStateCurrent.X, _mouseStateCurrent.Y), 2.0))
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
                                selectedUnit.CurrentOrder = new Order(OrderType.Attack, mouseVector);
                                break;
                            case "Move":
                                selectedUnit.CurrentOrder = new Order(OrderType.Move, mouseVector);
                                break;
                            case "Special":
                            case "Stop":
                                selectedUnit.CurrentOrder = new Order(OrderType.None, selectedUnit.Location);
                                selectedUnit.Momentum = new Vector2(0f);
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

            CatchInputs();

            if (_paused)
                return;

            UpdateExplosions(gameTime);

            UpdateUnits(gameTime);

            UpdateProjectiles();

            UpdateDeaths();
        }

        private void UpdateDeaths()
        {
            for (int i = 0; i < _units.Count; i++)
            {
                if (_units[i].CurrentHP <= 0)
                {
                    _explosions.Add(new Explosion(SpriteList.ContentSprites["explosion"], new Vector2(_units[i].Location.X - _units[i].Width / 2, _units[i].Location.Y - _units[i].Height / 2)));
                    _units.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < _blocks.Count; i++)
            {
                if (_blocks[i].HP <= 0)
                {
                    _explosions.Add(new Explosion(SpriteList.ContentSprites["explosion"], new Vector2(_blocks[i].Location.X - _blocks[i].Width / 2, _blocks[i].Location.Y - _blocks[i].Height / 2)));
                    _blocks.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int x = 0; x < _explosions.Count; x++)
            {
                Explosion exp = _explosions[x];
                if (exp.Done)
                {
                    _explosions.Remove(exp);
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

            foreach (Projectile p in _projectiles)
            {
                if (p.Location.X < 0 || p.Location.X > GraphicsDevice.Viewport.Width || p.Location.Y > GraphicsDevice.Viewport.Height || p.Location.Y < 0 || (p.Momentum.X == 0 && p.Momentum.Y == 0))
                {
                    killSprites.Add(p);
                    break;
                }
                p.UpdateState();

                bool hitUnit = false;
                foreach (Unit u in _units)
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
                    foreach (Block b in _blocks)
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
                _projectiles.Remove(x);
            }
        }

        private void UpdateUnits(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (Unit u in _units)
            {
                u.ReloadGun(elapsed);

                switch (u.CurrentOrder.Type)
                {
                    case OrderType.None:
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
                            u.CurrentOrder.Type = OrderType.None;
                            u.Location = u.CurrentOrder.Target;
                            u.Momentum = new Vector2(0f);
                        }

                        if (u.Momentum.X > 0f || u.Momentum.Y > 0f)
                        {
                            foreach (Block b in _blocks)
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
                            u.Shoot(ref _projectiles);
                        }

                        break;
                }

                u.UpdateState();
            }
        }

        #endregion State Updates

        private Unit GetSelctedUnit()
        {
            foreach (Unit unit in _units)
            {
                if (unit.Selected)
                {
                    return unit;
                }
            }

            return null;
        }
    }
}