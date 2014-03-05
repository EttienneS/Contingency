using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    public class Unit : Sprite
    {
        public Unit(int width, int height, int x, int y, int maxHp, string teamName)
        {
            Width = width;
            Height = height;
            Location = new Vector2(x, y);
            CurrentHP = maxHp;
            MaxHP = maxHp;

            CollisionRadius = width / 2;
            Team = teamName;

            OrderQueue = new OrderList();
            ShotCount = 0;
            ShootRate = 200;
            CanShoot = true;

            ID = Guid.NewGuid().ToString();
        }

        public Texture2D BulletSprite
        {
            get
            {
                return Team == "red"
                    ? SpriteList.ContentSprites["projectileRed"]
                    : SpriteList.ContentSprites["projectileBlue"];
            }
        }

        public Order CurrentOrder
        {
            get
            {
                if (OrderQueue != null && OrderQueue.Count > 0)
                {
                    return OrderQueue[0];
                }
                return new Order(OrderType.None, Location);
            }
        }

        public string ID { get; set; }

        public OrderList OrderQueue { get; private set; }

        public bool Selected { get; set; }

        public int ShotCount { get; set; }

        public Special Special { get; set; }

        private bool CanShoot { get; set; }

        private float ShootRate { get; set; }

        private float ShootTimer { get; set; }

        public Texture2D GetSprite()
        {
            switch (Special.Type)
            {
                case "blink":
                    if (Team == "red")
                    {
                        return Selected ? SpriteList.ContentSprites["blinkerRedSelected"] : SpriteList.ContentSprites["blinkerRed"];
                    }
                    return Selected ? SpriteList.ContentSprites["blinkerBlueSelected"] : SpriteList.ContentSprites["blinkerBlue"];

                case "build":
                    if (Team == "red")
                    {
                        return Selected ? SpriteList.ContentSprites["builderRedSelected"] : SpriteList.ContentSprites["builderRed"];
                    }
                    return Selected ? SpriteList.ContentSprites["builderBlueSelected"] : SpriteList.ContentSprites["builderBlue"];
            }

            return null;
        }

        public void Hit(Projectile p, Unit owner)
        {
            if (owner != null && Team == owner.Team)
                p.Damage = 0;

            CurrentHP -= p.Damage;
            p.Momentum = new Vector2(0f);
        }

        public void OrderComplete()
        {
            if (OrderQueue.Count > 0)
            {
                if (OrderQueue.Count == 1 && CurrentOrder.Type == OrderType.Attack)
                    return; // do not remove last order if its an attack order

                OrderQueue.RemoveAt(0);
            }
        }

        public void ReloadGun(float elapsedMiliSeconds)
        {
            if (!CanShoot)
            {
                ShootTimer += elapsedMiliSeconds;

                if (ShootTimer > ShootRate)
                {
                    ShootTimer = 0;
                    CanShoot = true;
                }
            }
        }

        public void Shoot(ref ProjectileList projectiles)
        {
            if (CanShoot)
            {
                Projectile p = new Projectile
                {
                    TargetAngle = TargetAngle
                };

                p.Momentum = new Vector2((float)Math.Cos(p.TargetAngle) * -5, (float)Math.Sin(p.TargetAngle) * -5);

                p.Location = Location;
                p.OwnerId = ID;

                CanShoot = false;
                projectiles.Add(p);

                ShotCount++;
            }
        }

        public override string ToString()
        {
            return Team + " - " + Special.Type + " - " + CurrentHP + "/" + MaxHP;
        }

        internal XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Unit/>");

            doc.ImportNode(SpriteSerialize(doc, doc.DocumentElement), true);

            Helper.AddAttribute(doc, doc.DocumentElement, "Selected", Selected.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "ShotCount", ShotCount.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "CanShoot", CanShoot.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "ShootRate", ShootRate.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "ShootTimer", ShootTimer.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "ID", ID);

            doc.DocumentElement.AppendChild(doc.ImportNode(Special.Serialize(), true));
            doc.DocumentElement.AppendChild(doc.ImportNode(OrderQueue.Serialize(), true));

            return doc.DocumentElement;
        }

        public Unit()
        { }

        internal static Unit Deserialize(XmlNode node)
        {
            Unit u = new Unit
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
                Selected = bool.Parse(Helper.GetAttribute(node, "Selected")),
                ShotCount = int.Parse(Helper.GetAttribute(node, "ShotCount")),
                CanShoot = bool.Parse(Helper.GetAttribute(node, "CanShoot")),
                ShootRate = float.Parse(Helper.GetAttribute(node, "ShootRate")),
                ShootTimer = float.Parse(Helper.GetAttribute(node, "ShootTimer")),
                ID = Helper.GetAttribute(node, "ID")
            };

            u.Special = Special.Deserialize(node.SelectSingleNode("Special"));
            u.OrderQueue = OrderList.Deserialize(node.SelectSingleNode("OrderList"));

            return u;
        }
    }

    public class UnitList : List<Unit>
    {
        public Unit this[string id]
        {
            get { return this.First(unit => unit.ID == id); }
        }

        public XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<UnitList/>");

            foreach (Unit u in this)
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(u.Serialize(), true));
            }

            return doc.DocumentElement;
        }

        internal static UnitList Deserialize(XmlNode node)
        {
            UnitList u = new UnitList();

            foreach (XmlNode child in node.ChildNodes)
            {
                u.Add(Unit.Deserialize(child));
            }

            return u;
        }
    }
}