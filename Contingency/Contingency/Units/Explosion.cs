using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Explosion : Sprite, ISerializable
    {
        private int curFrameX;
        private int curFrameY;

        public const int ImageNbr = 25;

        public const float Interval = 40f;

        public readonly int SpriteHeight;

        public Rectangle SpriteRect;

        public readonly int SpriteWidth;

        public float timer;

        public Explosion(SerializationInfo information, StreamingContext context)
        {
            Location = (Vector2)information.GetValue("Location", typeof(Vector2));
            CurrentAngle = (float)information.GetValue("CurrentAngle", typeof(float));
            TargetAngle = (float)information.GetValue("TargetAngle", typeof(float));
            Momentum = (Vector2)information.GetValue("Momentum", typeof(Vector2));
            MaxHP = (int)information.GetValue("MaxHP", typeof(int));
            CurrentHP = (int)information.GetValue("CurrentHP", typeof(int));
            Height = (int)information.GetValue("Height", typeof(int));
            Width = (int)information.GetValue("Width", typeof(int));
            Team = (string)information.GetValue("Team", typeof(string));
            CollisionRadius = (double)information.GetValue("CollisionRadius", typeof(double));

            SpriteHeight = (int)information.GetValue("SpriteHeight", typeof(int));
            SpriteWidth = (int)information.GetValue("SpriteWidth", typeof(int));
            SpriteRect = (Rectangle)information.GetValue("SpriteRect", typeof(Rectangle));
            timer = (float)information.GetValue("timer", typeof(float));
            curFrameX = (int)information.GetValue("curFrameX", typeof(int));
            curFrameY = (int)information.GetValue("curFrameY", typeof(int));
        }

        public Explosion(Vector2 location)
        {
            SpriteWidth = SpriteSheet.Width / (int)Math.Sqrt(ImageNbr);
            SpriteHeight = SpriteSheet.Height / (int)Math.Sqrt(ImageNbr);
            SpriteRect = new Rectangle(curFrameX * SpriteWidth, 0, SpriteWidth, SpriteHeight);

            Location = location;
        }

        public bool Done { get; set; }

        public Texture2D SpriteSheet
        {
            get
            {
                return SpriteList.ContentSprites["explosion"];
            }
        }

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
            info.AddValue("CurrentAngle", CurrentAngle);
            info.AddValue("TargetAngle", TargetAngle);
            info.AddValue("Momentum", Momentum);
            info.AddValue("MaxHP", MaxHP);
            info.AddValue("CurrentHP", CurrentHP);
            info.AddValue("Team", Team);
            info.AddValue("CollisionRadius", CollisionRadius);
            info.AddValue("Height", Height);
            info.AddValue("Width", Width);
            info.AddValue("SpriteHeight", SpriteHeight);
            info.AddValue("SpriteRect", SpriteRect);
            info.AddValue("SpriteWidth", SpriteWidth);
            info.AddValue("timer", timer);
            info.AddValue("curFrameX", curFrameX);
            info.AddValue("curFrameY", curFrameY);
        }
    }
}