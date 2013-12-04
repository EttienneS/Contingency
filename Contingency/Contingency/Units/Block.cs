using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Block : Sprite
    {
       
        public string Team { get; set; }

        public Unit Owner { get; set; }

        internal void Hit(Projectile p)
        {
            if (Team == p.Owner.Team)
                p.Damage = 0;

            CurrentHP -= p.Damage;
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
            MaxHP = 10;
            CurrentHP = MaxHP;
        }

        public Block(SerializationInfo information, StreamingContext context)
        {
            Deserialize(information,context);
        }
    }
}