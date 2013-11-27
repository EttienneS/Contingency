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
        public bool Paused = true;
        private Dictionary<string, Texture2D> contentSprites = new Dictionary<string, Texture2D>();
        private List<Explosion> explosions = new List<Explosion>();
        private GraphicsDeviceManager graphics;
        private MouseState mouseStateCurrent, mouseStatePrevious;
        private List<Projectile> projectiles = new List<Projectile>();
        //private Random r = new Random();
        private SpriteBatch spriteBatch;
        private List<Unit> units = new List<Unit>();

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            DrawProjectiles();

            DrawUnits();

            DrawExplosions();

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            base.Initialize();

            mouseStateCurrent = Mouse.GetState();
            mouseStatePrevious = Mouse.GetState();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadSprites();

            // spawn units
            for (int i = 0; i < 10; i++)
            {
                Unit red = new Unit(20, 20, 40, 50 + i * 30, contentSprites["unitRed"], contentSprites["unitRedSelected"], 5, "red", contentSprites["projectileRed"]);
                
                red.CurrentAngle = red.CurrentAngle + (float)Math.PI;
                red.TargetAngle = red.CurrentAngle;

                units.Add(red);

                units.Add(new Unit(20, 20, GraphicsDevice.Viewport.Width - 60, 50 + i * 30, contentSprites["unitBlue"], contentSprites["unitBlueSelected"], 5, "blue", contentSprites["projectileBlue"]));
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CatchInputs();

            if (Paused)
                return;

            UpdateExplosions(gameTime);

            UpdateUnits(gameTime);

            UpdateProjectiles();

            UpdateDeaths();
        }

        private void CatchInputs()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Paused = false;
            }
            else
            {
                Paused = true;
            }

            mouseStateCurrent = Mouse.GetState();

            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                MouseClicked();
            }
            else if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton == ButtonState.Released)
            {
                Unit selected = GetSelctedUnit();

                if (selected != null)
                    selected.Selected = false;
            }

            mouseStatePrevious = mouseStateCurrent;
        }

        private void DrawProjectiles()
        {
            foreach (Projectile sprite in projectiles)
            {
                spriteBatch.Draw(sprite.GetSprite(), sprite.Location, null, Color.White, sprite.CurrentAngle, new Vector2(sprite.Width / 2, sprite.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawUnits()
        {
            foreach (Unit u in units)
            {
                spriteBatch.Draw(u.GetSprite(), u.Location, null, Color.White, u.CurrentAngle, new Vector2(u.Width / 2, u.Height / 2), 1.0f, SpriteEffects.None, 0f);

            }
        }

        private void DrawExplosions()
        {
            foreach (Explosion exp in explosions)
            {
                spriteBatch.Draw(exp.SpriteSheet, new Rectangle((int)exp.Location.X - 32, (int)exp.Location.Y - 32, exp.spriteWidth, exp.spriteHeight), exp.spriteRect, Color.White);


            }
        }

        private Unit GetSelctedUnit()
        {
            foreach (Unit unit in units)
            {
                if (unit.Selected)
                {
                    return unit;
                }
            }

            return null;
        }

        private void LoadSprites()
        {
            contentSprites.Add("unitRed", Content.Load<Texture2D>("unitRed"));
            contentSprites.Add("unitBlue", Content.Load<Texture2D>("unitBlue"));
            contentSprites.Add("unitRedSelected", Content.Load<Texture2D>("unitRedSelected"));
            contentSprites.Add("unitBlueSelected", Content.Load<Texture2D>("unitBlueSelected"));
            contentSprites.Add("projectileRed", Content.Load<Texture2D>("projectileRed"));
            contentSprites.Add("projectileBlue", Content.Load<Texture2D>("projectileBlue"));
            contentSprites.Add("explosion", Content.Load<Texture2D>("explosion"));
        }

        private void MouseClicked()
        {
            Unit selectedUnit = GetSelctedUnit();

            if (selectedUnit == null)
            {
                foreach (Unit u in units)
                {
                    if (u.Touches(new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y), 2.0))
                    {
                        u.Selected = !u.Selected;
                        selectedUnit = u;
                        break;
                    }
                }
            }
            else
            {
                Vector2 mouseVector = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                bool clickedUnit = false;
                foreach (Unit u in units)
                {
                    if (u != selectedUnit && u.Touches(mouseVector, 2.0))
                    {
                        if (u.Team == selectedUnit.Team)
                        {
                            selectedUnit.CurrentOrder = new Order(OrderType.Move, mouseVector);
                        }
                        else
                        {
                            selectedUnit.CurrentOrder = new Order(OrderType.Attack, u.Location);

                        }
                        clickedUnit = true;
                        break;
                    }
                }
                if (!clickedUnit)
                {
                    selectedUnit.CurrentOrder = new Order(OrderType.Move, mouseVector);
                }
            }
        }

        private void UpdateDeaths()
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].HP <= 0)
                {
                    explosions.Add(new Explosion(contentSprites["explosion"], new Vector2(units[i].Location.X - units[i].Width / 2, units[i].Location.Y - units[i].Height / 2)));
                    units.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int x = 0; x < explosions.Count; x++)
            {
                Explosion exp = explosions[x];
                if (exp.Done)
                {
                    explosions.Remove(exp);
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

            foreach (Projectile p in projectiles)
            {
                if (p.Location.X < 0 || p.Location.X > GraphicsDevice.Viewport.Width || p.Location.Y > GraphicsDevice.Viewport.Height || p.Location.Y < 0 || (p.Momentum.X == 0 && p.Momentum.Y == 0))
                {
                    killSprites.Add(p);
                }
                else
                {
                    p.UpdateState();
                }

                foreach (Unit u in units)
                {
                    if (u.Touches(p) && p.Owner != u)
                    {
                        u.Hit(p);
                    }
                }
            }

            foreach (Projectile x in killSprites)
            {
                projectiles.Remove(x);
            }
        }

        private void UpdateUnits(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (Unit u in units)
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

                        break;

                    case OrderType.Attack:
                        u.Momentum = new Vector2(0f);
                        u.Target(u.CurrentOrder.Target);

                        if (u.CurrentAngle == u.TargetAngle)
                        {
                            u.Shoot(ref projectiles);
                        }

                        break;
                }

                u.UpdateState();
            }
        }
    }
}