using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    internal class Unit : Sprite
    {
        private Texture2D _selectedSprite;
        private Texture2D _sprite;
        private Texture2D _bulletSprite;
        private float _timer;

        public Order CurrentOrder;

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

            ShootRate = 500;
            CanShoot = true;
        }

        public double Vision { get; set; }

        public int HP { get; set; }

        public bool Selected { get; set; }

        public Vector2 Facing { get; set; }

        public string Team { get; set; }

        public float ShootRate { get; set; }

        public bool CanShoot { get; set; }

        public override Texture2D GetSprite()
        {
            return Selected ? _selectedSprite : _sprite;
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

                p.Momentum = new Vector2((float)Math.Cos(p.TargetAngle) * -1, (float)Math.Sin(p.TargetAngle) * -1);
                p.Location = Location;
                p.Owner = this;

                CanShoot = false;
                projectiles.Add(p);
            }
        }

        internal void UpdateTarget(List<Unit> units)
        {
            foreach (Unit u in units)
            {
                if (u.Team != Team && u.Touches(u.CurrentOrder.Target, Vision))
                {
                    Target(u.Location);
                    break;
                }
            }
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
    }
}