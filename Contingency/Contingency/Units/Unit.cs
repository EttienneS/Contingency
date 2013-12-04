using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Unit : Sprite
    {
        public List<Order> OrderQueue; 
        
        private readonly Texture2D _bulletSprite;
        private readonly Texture2D _selectedSprite;
        private readonly Texture2D _sprite;
        private float _timer;

        public Unit(SerializationInfo information, StreamingContext context)
        {
            Deserialize(information,context);
        }

        public Unit(int width, int height, int x, int y, Texture2D sprite, Texture2D selectedSprite, int maxHp, string teamName, Texture2D bulletSprite)
        {
            Width = width;
            Height = height;
            Location = new Vector2(x, y);
            CurrentHP = maxHp;
            MaxHP = maxHp;
            
            CollisionRadius = width / 2;
            _bulletSprite = bulletSprite;
            _sprite = sprite;
            _selectedSprite = selectedSprite;
            Team = teamName;
            Vision = 50;

            OrderQueue = new List<Order>();
            ShotCount = 0;
            ShootRate = 100;
            CanShoot = true;
        }

        public Order CurrentOrder
        {
            get
            {
                if (OrderQueue != null && OrderQueue.Count > 0)
                {
                    return OrderQueue[0];
                }
                else
                {
                    return new Order(OrderType.None, Location);
                }
            }
        }
        public bool CanShoot { get; set; }

        public bool Selected { get; set; }

        public float ShootRate { get; set; }

        public int ShotCount { get; set; }

        public string Team { get; set; }

        public double Vision { get; set; }

        public override Texture2D GetSprite()
        {
            return Selected ? _selectedSprite : _sprite;
        }

        public void ReloadGun(float elapsedMiliSeconds)
        {
            if (!CanShoot)
            {
                _timer += elapsedMiliSeconds;

                if (_timer > ShootRate)
                {
                    _timer = 0;
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

        internal void Shoot(ref List<Projectile> projectiles)
        {
            if (CanShoot)
            {
                Projectile p = new Projectile(_bulletSprite);

                p.TargetAngle = TargetAngle + (float)Helper.Rand.Next(-5, 5) / 100;

                p.Momentum = new Vector2((float)Math.Cos(p.TargetAngle) * -5, (float)Math.Sin(p.TargetAngle) * -5);
                
                p.Location = Location;
                p.Owner = this;

                CanShoot = false;
                projectiles.Add(p);

                ShotCount++;
            }
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

    }
}