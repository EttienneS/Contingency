using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace Contingency.Units
{
    [Serializable]
    public class Special : ISerializable
    {
        public string Type { get; set; }

        public float CoolDown { get; set; }

        public float Elapsed { get; set; }

        public float Power { get; set; }

        public bool Complete
        {
            get;
            set;
        }

        internal void Execute(float elapsed, ref Unit owner)
        {
            Elapsed += elapsed;
            switch (Type.ToLower())
            {
                case "blink":
                    if (Elapsed > CoolDown)
                    {
                        owner.TargetAngle = (float)Math.Atan2(owner.Location.Y - owner.CurrentOrder.Target.Y, owner.Location.X - owner.CurrentOrder.Target.X);
                        owner.CurrentAngle = owner.TargetAngle;
                        owner.Location = owner.CurrentOrder.Target;
                        Complete = true;

                        owner.Hit(new Projectile(5));

                        bool landedInSomething = false;
                        foreach (Block b in GameState.Blocks)
                        {
                            if (owner.Touches(b))
                            {
                                b.Hit(new Projectile(int.MaxValue));
                                landedInSomething = true;
                            }
                        }

                        if (landedInSomething)
                        {
                            owner.Hit(new Projectile(owner.MaxHP / 3));
                        }
                    }
                    break;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type);
        }

        public Special(SerializationInfo information, StreamingContext context)
        {
            Type = (string)information.GetValue("Type", typeof(string));
        }

        public Special(string type, float coolDown, float power)
        {
            Type = type;
            CoolDown = coolDown;
            Power = power;
        }
    }
}
