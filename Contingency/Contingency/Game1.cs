#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Contingency.Units;
#endregion

namespace Contingency
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Unit> units = new List<Unit>();
        MouseState mouseStateCurrent, mouseStatePrevious;

        Random r = new Random();

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < 10; i++)
            {
                units.Add(new Unit(20, 20, r.Next(GraphicsDevice.Viewport.Width), r.Next(GraphicsDevice.Viewport.Height), Color.Blue));
                units.Add(new Unit(20, 20, r.Next(GraphicsDevice.Viewport.Width), r.Next(GraphicsDevice.Viewport.Height), Color.Red));
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mouseStateCurrent = Mouse.GetState();

            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                Rectangle mouseState = new Rectangle(mouseStateCurrent.X, mouseStateCurrent.Y, 1, 1);

                foreach (Unit u in units)
                {


                    if (u.CollisionRectangle.Intersects(mouseState))
                    {
                        //u.Location = Vector2.Add(u.Location, new Vector2(30, 30));
                        u.Color = Color.White;
                    }

                }

            }

            mouseStatePrevious = mouseStateCurrent;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (Unit unit in units)
            {
                spriteBatch.Draw(unit.GetSprite(GraphicsDevice), unit.Location, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

    }

}
