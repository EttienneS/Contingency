using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public class Projectile : Sprite, ISerializable
    {
        public Projectile(SerializationInfo information, StreamingContext context)
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
            Damage = (int)information.GetValue("Damage", typeof(int));
        }

        public Projectile()
        {
            Damage = 3;
            CollisionRadius = Width / 2;
        }

        public override Texture2D GetSprite()
        {
            return Owner.BulletSprite;
        }

        public int Damage { get; set; }

        public Unit Owner
        {
            get;
            set;
        }

        public void GetObjectData(SerializationInfo information, StreamingContext context)
        {
            information.AddValue("Location", Location);
            information.AddValue("CurrentAngle", CurrentAngle);
            information.AddValue("TargetAngle", TargetAngle);
            information.AddValue("Momentum", Momentum);
            information.AddValue("MaxHP", MaxHP);
            information.AddValue("CurrentHP", CurrentHP);
            information.AddValue("Team", Team);
            information.AddValue("CollisionRadius", CollisionRadius);
            information.AddValue("Height", Height);
            information.AddValue("Width", Width);
            information.AddValue("Owner", Owner);
            information.AddValue("Damage", Damage);
        }
    }
}