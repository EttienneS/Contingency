﻿#region Using Statements

using Difficult_circumstances.Model;
using Difficult_circumstances.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Threading;

#endregion Using Statements

namespace Difficult_circumstances
{
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        public WorldModel WorldModel;

        public Game1()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 768
            };
            graphics.ApplyChanges();

            View.View.ScreenWidth = graphics.PreferredBackBufferWidth;
            View.View.ScreenHeight = graphics.PreferredBackBufferHeight;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            WorldModel = new WorldModel(15);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var fontFilePath = Path.Combine(Content.RootDirectory, "font.fnt");
            using (var stream = TitleContainer.OpenStream(fontFilePath))
            {
                View.View.FontRenderer = new FontRenderer(FontRenderer.FontLoader.Load(stream), Content.Load<Texture2D>("font_0"));
                stream.Close();
            }
        }

        protected override void UnloadContent()
        {
        }

        private Thread _updateThread;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_updateThread == null || !_updateThread.IsAlive)
            {
                bool turnEnded = Controller.Controller.ParseInput(WorldModel);

                if (turnEnded)
                {
                    if (_updateThread == null || !_updateThread.IsAlive)
                    {
                        _updateThread = new Thread(unused => Controller.Controller.Update(WorldModel, gameTime));
                        _updateThread.Start();
                    }

                    WorldModel.UpdateTime();

                   
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            View.View.UpdateView(WorldModel, GraphicsDevice, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}