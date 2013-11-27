using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    internal class Explosion : Sprite
    {
        public int curFrameX = 0;
        public int curFrameY = 0;

        public int imageNbr = 25;

        public float interval = 40f;

        public int spriteHeight;

        public Rectangle spriteRect;

        public int spriteWidth;

        public float timer = 0;

        public Explosion(Texture2D sprite, Vector2 location)
        {
            SpriteSheet = sprite;
            spriteWidth = SpriteSheet.Width / (int)Math.Sqrt(imageNbr);
            spriteHeight = SpriteSheet.Height / (int)Math.Sqrt(imageNbr);
            spriteRect = new Rectangle(curFrameX * spriteWidth, 0, spriteWidth, spriteHeight);

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

            if (timer >= interval)
            {
                timer = 0;
                curFrameX++;
                if (curFrameX >= 5)
                {
                    curFrameX = 0;
                    curFrameY++;
                }
            }

            spriteRect.X = curFrameX * spriteWidth;
            spriteRect.Y = curFrameY * spriteHeight;

            if (curFrameY > 5)
            {
                this.Done = true;
            }
        }
    }
}