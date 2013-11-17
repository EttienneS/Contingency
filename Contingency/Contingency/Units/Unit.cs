using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    class Unit : ISprite
    {
        public Unit(int width, int height, int x, int y, Color color)
        {
            Width = width;
            Height = height;
            Location = new Vector2(x, y);
            Color = color;

            _collisionRectangle = new Rectangle(x, y, width, height);
        }

        public Vector2 Location { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color;
        private Rectangle _collisionRectangle;

        public Rectangle CollisionRectangle
        {
            get
            {
                _collisionRectangle.X = (int)Location.X;
                _collisionRectangle.Y = (int)Location.Y;

                return _collisionRectangle;
            }
        }

        public Texture2D GetSprite(GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, Width, Height);

            Color[] data = new Color[Width * Height];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color;

            texture.SetData(data);

            return texture;
        }
    }
}
