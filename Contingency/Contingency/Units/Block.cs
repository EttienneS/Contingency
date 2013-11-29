using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public class Block : Sprite
    {
        public int HP { get; set; }

        public string Team { get; set; }

        public Unit Owner { get; set; }

        internal void Hit(Projectile p)
        {
            if (Team == p.Owner.Team)
                p.Damage = 0;

            HP -= p.Damage;
            p.Momentum = new Vector2(0f);
        }

        public override Texture2D GetSprite()
        {
            return SpriteList.ContentSprites["block"];
        }

        public Block(Vector2 location)
        {
            Location = location;
            Width = 10;
            Height = 10;
            CollisionRadius = 5;
            HP = 10;
        }
    }
}