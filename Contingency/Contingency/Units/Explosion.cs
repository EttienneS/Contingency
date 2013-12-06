using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Explosion : Sprite, ISerializable
    {
        public readonly int SpriteHeight;
        public readonly int SpriteWidth;
        public Rectangle SpriteRect;
        private const int ImageNbr = 25;
        private const float Interval = 40f;
        private int _curFrameX;
        private int _curFrameY;
        private float _timer;

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

            int x = (int)information.GetValue("SpriteRectX", typeof(int));
            int y = (int)information.GetValue("SpriteRectY", typeof(int));
            int w = (int)information.GetValue("SpriteRectW", typeof(int));
            int h = (int)information.GetValue("SpriteRectH", typeof(int));

            SpriteRect = new Rectangle(x, y, w, h);
            _timer = (float)information.GetValue("timer", typeof(float));
            _curFrameX = (int)information.GetValue("curFrameX", typeof(int));
            _curFrameY = (int)information.GetValue("curFrameY", typeof(int));
        }

        public Explosion(Vector2 location)
        {
            SpriteWidth = SpriteSheet.Width / (int)Math.Sqrt(ImageNbr);
            SpriteHeight = SpriteSheet.Height / (int)Math.Sqrt(ImageNbr);
            SpriteRect = new Rectangle(_curFrameX * SpriteWidth, 0, SpriteWidth, SpriteHeight);

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

            info.AddValue("SpriteWidth", SpriteWidth);
            info.AddValue("timer", _timer);
            info.AddValue("curFrameX", _curFrameX);
            info.AddValue("curFrameY", _curFrameY);

            info.AddValue("SpriteRectX", SpriteRect.X);
            info.AddValue("SpriteRectY", SpriteRect.Y);
            info.AddValue("SpriteRectW", SpriteRect.Width);
            info.AddValue("SpriteRectH", SpriteRect.Height);
        }

        public override Texture2D GetSprite()
        {
            return SpriteSheet;
        }

        public void Update(float elapsedSeconds)
        {
            _timer += elapsedSeconds;

            if (_timer >= Interval)
            {
                _timer = 0;
                _curFrameX++;
                if (_curFrameX >= 5)
                {
                    _curFrameX = 0;
                    _curFrameY++;
                }
            }

            SpriteRect.X = _curFrameX * SpriteWidth;
            SpriteRect.Y = _curFrameY * SpriteHeight;

            if (_curFrameY > 5)
            {
                Done = true;
            }
        }
    }
}