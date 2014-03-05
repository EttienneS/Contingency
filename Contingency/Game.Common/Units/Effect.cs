using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    public class Effect : Sprite
    {
        public int SpriteHeight;

        public int SpriteWidth;

        public int CurFrameX;
        public int CurFrameY;
        public Rectangle SpriteRect;

        public float Timer;
        private const int ImageNbr = 25;

        private const float Interval = 40f;

        public Effect(Vector2 location)
        {
            SpriteWidth = SpriteSheet.Width / (int)Math.Sqrt(ImageNbr);
            SpriteHeight = SpriteSheet.Height / (int)Math.Sqrt(ImageNbr);
            SpriteRect = new Rectangle(CurFrameX * SpriteWidth, 0, SpriteWidth, SpriteHeight);

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

        public Texture2D GetSprite()
        {
            return SpriteSheet;
        }

        public void Update(float elapsedSeconds)
        {
            Timer += elapsedSeconds;

            if (Timer >= Interval)
            {
                Timer = 0;
                CurFrameX++;
                if (CurFrameX >= 5)
                {
                    CurFrameX = 0;
                    CurFrameY++;
                }
            }

            SpriteRect.X = CurFrameX * SpriteWidth;
            SpriteRect.Y = CurFrameY * SpriteHeight;

            if (CurFrameY > 5)
            {
                Done = true;
            }
        }

        internal XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Effect/>");

            doc.ImportNode(SpriteSerialize(doc, doc.DocumentElement), true);

            Helper.AddAttribute(doc, doc.DocumentElement, "SpriteHeight", SpriteHeight.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "SpriteWidth", SpriteWidth.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "SpriteRect", SpriteRect);
            Helper.AddAttribute(doc, doc.DocumentElement, "CurFrameX", CurFrameX.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "CurFrameY", CurFrameY.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Timer", Timer.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Done", Done.ToString());

            return doc.DocumentElement;
        }

        public Effect()
        {
        }

        internal static Effect Deserialize(XmlNode node)
        {
            Effect b = new Effect
            {
                TurnSpeed = float.Parse(Helper.GetAttribute(node, "TurnSpeed")),
                Momentum = Helper.GetVectorAttribute(node, "Momentum"),
                TargetAngle = float.Parse(Helper.GetAttribute(node, "TargetAngle")),
                CollisionRadius = float.Parse(Helper.GetAttribute(node, "CollisionRadius")),
                CurrentAngle = float.Parse(Helper.GetAttribute(node, "CurrentAngle")),
                CurrentHP = int.Parse(Helper.GetAttribute(node, "CurrentHP")),
                Location = Helper.GetVectorAttribute(node, "Location"),
                MaxHP = int.Parse(Helper.GetAttribute(node, "MaxHP")),
                Team = Helper.GetAttribute(node, "Team"),
                Width = int.Parse(Helper.GetAttribute(node, "Width")),
                Height = int.Parse(Helper.GetAttribute(node, "Height")),

                SpriteHeight = int.Parse(Helper.GetAttribute(node, "SpriteHeight")),
                SpriteWidth = int.Parse(Helper.GetAttribute(node, "SpriteWidth")),
                SpriteRect = Helper.GetRectangleAttribute(node, "SpriteRect"),
                CurFrameX = int.Parse(Helper.GetAttribute(node, "CurFrameX")),
                CurFrameY = int.Parse(Helper.GetAttribute(node, "CurFrameY")),
                Timer = float.Parse(Helper.GetAttribute(node, "Timer")),
                Done = bool.Parse(Helper.GetAttribute(node, "Done")),
            };

            return b;
        }
    }

    public class EffectList : List<Effect>
    {
        public XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<EffectList/>");

            foreach (Effect e in this)
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(e.Serialize(), true));
            }

            return doc.DocumentElement;
        }

        internal static EffectList Deserialize(XmlNode node)
        {
            EffectList e = new EffectList();

            foreach (XmlNode child in node.ChildNodes)
            {
                e.Add(Effect.Deserialize(child));
            }

            return e;
        }
    }
}