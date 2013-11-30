using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    public class Projectile : Sprite
    {
        private Texture2D Sprite { get; set; }

        public Projectile(Texture2D sprite)
        {
            Sprite = sprite;
            Damage = 3;
            CollisionRadius = Width / 2;
        }

        public override Texture2D GetSprite()
        {
            return Sprite;
        }

        public int Damage { get; set; }

        internal Unit Owner
        {
            get;
            set;
        }
    }
}