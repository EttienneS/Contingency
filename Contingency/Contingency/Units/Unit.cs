using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    public class Unit : Sprite
    {
        public Order CurrentOrder;
        private readonly Texture2D _bulletSprite;
        private readonly Texture2D _selectedSprite;
        private readonly Texture2D _sprite;
        private float _timer;

        public Unit(int width, int height, int x, int y, Texture2D sprite, Texture2D selectedSprite, int hp, string teamName, Texture2D bulletSprite)
        {
            Width = width;
            Height = height;
            Location = new Vector2(x, y);
            HP = hp;

            CollisionRadius = width / 2;
            _bulletSprite = bulletSprite;
            _sprite = sprite;
            _selectedSprite = selectedSprite;
            Team = teamName;
            Vision = 50;

            CurrentOrder = new Order(OrderType.None, Location);

            ShootRate = 250;
            CanShoot = true;
        }

        public bool CanShoot { get; set; }

        public int HP { get; set; }

        public bool Selected { get; set; }

        public float ShootRate { get; set; }

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

            HP -= p.Damage;
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
            }
        }
    }
}