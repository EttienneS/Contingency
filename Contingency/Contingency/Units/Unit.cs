using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Unit : Sprite, ISerializable
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

            OrderQueue = new List<Order>();
            ShotCount = 0;
            ShootRate = 100;
            CanShoot = true;
        }

        protected Unit(SerializationInfo information, StreamingContext context)
        {
            Location = (Vector2)information.GetValue("Location", typeof(Vector2));
            CurrentAngle = (float)information.GetValue("CurrentAngle", typeof(float));
            TargetAngle = (float)information.GetValue("TargetAngle", typeof(float));
            Momentum = (Vector2)information.GetValue("Momentum", typeof(Vector2));
            MaxHP = (int)information.GetValue("MaxHP", typeof(int));
            CurrentHP = (int)information.GetValue("CurrentHP", typeof(int));
            Height = (int)information.GetValue("Height", typeof(int));
            Width = (int)information.GetValue("Width", typeof(int));
            OrderQueue = (List<Order>)information.GetValue("OrderQueue", typeof(List<Order>));
            Team = (string)information.GetValue("Team", typeof(string));
            CollisionRadius = (double)information.GetValue("CollisionRadius", typeof(double));
            CanShoot = (bool)information.GetValue("CanShoot", typeof(bool));
            ShootRate = (float)information.GetValue("ShootRate", typeof(float));
            ShotCount = (int)information.GetValue("ShotCount", typeof(int));
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

        public List<Order> OrderQueue { get; set; }

        public bool Selected { get; set; }

        public int ShotCount { get; set; }

        private bool CanShoot { get; set; }

        private float ShootRate { get; set; }

        private float ShootTimer { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
            info.AddValue("CurrentAngle", CurrentAngle);
            info.AddValue("TargetAngle", TargetAngle);
            info.AddValue("Momentum", Momentum);
            info.AddValue("MaxHP", MaxHP);
            info.AddValue("CurrentHP", CurrentHP);
            info.AddValue("OrderQueue", OrderQueue);
            info.AddValue("Team", Team);
            info.AddValue("CollisionRadius", CollisionRadius);
            info.AddValue("Height", Height);
            info.AddValue("Width", Width);
            info.AddValue("CanShoot", CanShoot);
            info.AddValue("ShootRate", ShootRate);
            info.AddValue("ShotCount", ShotCount);
        }

        public override Texture2D GetSprite()
        {
            if (Team == "red")
            {
                return Selected ? SpriteList.ContentSprites["unitRedSelected"] : SpriteList.ContentSprites["unitRed"];
            }
            return Selected ? SpriteList.ContentSprites["unitBlueSelected"] : SpriteList.ContentSprites["unitBlue"];
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

        internal void Hit(Projectile p)
        {
            if (Team == p.Owner.Team)
                p.Damage = 0;

            CurrentHP -= p.Damage;
            p.Momentum = new Vector2(0f);
        }

        internal void OrderComplete()
        {
            if (OrderQueue.Count > 0)
            {
                if (OrderQueue.Count == 1 && CurrentOrder.Type == OrderType.Attack)
                    return; // do not remove last order if its an attack order

                OrderQueue.RemoveAt(0);
            }
        }

        internal void Shoot(ref List<Projectile> projectiles)
        {
            if (CanShoot)
            {
                Projectile p = new Projectile
                {
                    TargetAngle = TargetAngle + (float)Helper.Rand.Next(-3, 3) / 100
                };  // add randomness to angle for long distance targeting

                p.Momentum = new Vector2((float)Math.Cos(p.TargetAngle) * -5, (float)Math.Sin(p.TargetAngle) * -5);

                p.Location = Location;
                p.Owner = this;

                CanShoot = false;
                projectiles.Add(p);

                ShotCount++;
            }
        }
    }
}