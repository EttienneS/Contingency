using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Block : Sprite, ISerializable
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
           Location = (Vector2)information.GetValue("Location", typeof(Vector2));
            CurrentAngle = (float)information.GetValue("CurrentAngle", typeof(float));
            TargetAngle = (float)information.GetValue("TargetAngle", typeof(float));
            Momentum = (Vector2)information.GetValue("Momentum", typeof(Vector2));
            MaxHP = (int)information.GetValue("MaxHP", typeof(int));
            CurrentHP = (int)information.GetValue("CurrentHP", typeof(int));
            Height = (int)information.GetValue("Height", typeof(int));
            Width = (int)information.GetValue("Width", typeof(int));
            Team = (string)information.GetValue("Team", typeof(string));
            Owner = (Unit)information.GetValue("Owner", typeof(Unit));
            CollisionRadius = (double)information.GetValue("CollisionRadius", typeof(double));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
            info.AddValue("CurrentAngle", CurrentAngle);
            info.AddValue("TargetAngle", TargetAngle);
            info.AddValue("Momentum", Momentum);
            info.AddValue("MaxHP", MaxHP);
            info.AddValue("CurrentHP", CurrentHP);
            info.AddValue("Team", Team);
            info.AddValue("Owner", Owner);
            info.AddValue("CollisionRadius", CollisionRadius);
            info.AddValue("Height", Height);
            info.AddValue("Width", Width);
        }
    }
}