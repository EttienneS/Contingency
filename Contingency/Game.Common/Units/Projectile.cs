using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    public class Projectile : Sprite
    {
        public Projectile(int damage)
        {
            Damage = damage;
        }

        public Projectile()
        {
            Damage = 3;
            CollisionRadius = Width / 2;
        }

        public int Damage { get; set; }

        public string OwnerId { get; set; }

        public Texture2D GetSprite(Unit owner)
        {
            return owner.BulletSprite;
        }

        internal XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Projectile/>");

            doc.ImportNode(SpriteSerialize(doc, doc.DocumentElement), true);

            Helper.AddAttribute(doc, doc.DocumentElement, "Damage", Damage.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "OwnerId", OwnerId);

            return doc.DocumentElement;
        }

        internal static Projectile Deserialize(XmlNode node)
        {
            Projectile p = new Projectile
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
                Damage = int.Parse(Helper.GetAttribute(node, "Damage")),
                OwnerId = Helper.GetAttribute(node, "OwnerId")
            };

            return p;
        }
    }

    public class ProjectileList : List<Projectile>
    {
        public XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<ProjectileList/>");

            foreach (Projectile p in this)
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(p.Serialize(), true));
            }

            return doc.DocumentElement;
        }

        internal static ProjectileList Deserialize(XmlNode node)
        {
            ProjectileList p = new ProjectileList();

            foreach (XmlNode child in node.ChildNodes)
            {
                p.Add(Projectile.Deserialize(child));
            }

            return p;
        }
    }
}