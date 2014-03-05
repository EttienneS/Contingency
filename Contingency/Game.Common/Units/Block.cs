using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    public class Block : Sprite
    {
        public Block(Vector2 location)
        {
            Location = location;
            Width = 10;
            Height = 10;
            CollisionRadius = 5;
            MaxHP = 10;
            CurrentHP = MaxHP;
        }

        public string OwnerId { get; private set; }

        public Texture2D GetSprite()
        {
            return SpriteList.ContentSprites["block"];
        }

        public void Hit(Projectile p, Unit owner)
        {
            if (Team != null && Team == owner.Team)
                p.Damage = 0;

            CurrentHP -= p.Damage;
            p.Momentum = new Vector2(0f);
        }

        public new XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Block/>");

            doc.ImportNode(SpriteSerialize(doc, doc.DocumentElement), true);

            if (OwnerId != null)
            {
                Helper.AddAttribute(doc, doc.DocumentElement, "OwnerId", OwnerId);
            }

            return doc.DocumentElement;
        }

        public Block()
        { }

        internal static Block Deserialize(XmlNode node)
        {
            Block b = new Block
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
                OwnerId = Helper.GetAttribute(node, "OwnerId")
            };

            return b;
        }
    }

    public class BlockList : List<Block>
    {
        public XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<BlockList/>");

            foreach (Block b in this)
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(b.Serialize(), true));
            }

            return doc.DocumentElement;
        }

        internal static BlockList Deserialize(XmlNode node)
        {
            BlockList b = new BlockList();

            foreach (XmlNode child in node.ChildNodes)
            {
                b.Add(Block.Deserialize(child));
            }

            return b;
        }
    }
}