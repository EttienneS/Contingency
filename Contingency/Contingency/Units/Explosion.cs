using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    internal class Explosion : Sprite
    {
        private int curFrameX;
        private int curFrameY;

        public const int ImageNbr = 25;

        public const float Interval = 40f;

        public readonly int SpriteHeight;

        public Rectangle SpriteRect;

        public readonly int SpriteWidth;

        public float timer;

        public Explosion(Texture2D sprite, Vector2 location)
        {
            SpriteSheet = sprite;
            SpriteWidth = SpriteSheet.Width / (int)Math.Sqrt(ImageNbr);
            SpriteHeight = SpriteSheet.Height / (int)Math.Sqrt(ImageNbr);
            SpriteRect = new Rectangle(curFrameX * SpriteWidth, 0, SpriteWidth, SpriteHeight);

            Location = location;
        }

        public bool Done { get; set; }

        public Texture2D SpriteSheet { get; set; }

        public override Texture2D GetSprite()
        {
            return SpriteSheet;
        }

        public void Update(float elapsedSeconds)
        {
            timer += elapsedSeconds;

            if (timer >= Interval)
            {
                timer = 0;
                curFrameX++;
                if (curFrameX >= 5)
                {
                    curFrameX = 0;
                    curFrameY++;
                }
            }

            SpriteRect.X = curFrameX * SpriteWidth;
            SpriteRect.Y = curFrameY * SpriteHeight;

            if (curFrameY > 5)
            {
                Done = true;
            }
        }
    }
}