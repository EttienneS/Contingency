using System;
using System.Runtime.Serialization;

namespace Contingency.Units
{
    [Serializable]
    public class Special : ISerializable
    {
        protected Special(SerializationInfo information, StreamingContext context)
        {
            Type = (string)information.GetValue("Type", typeof(string));
            CoolDown = (float)information.GetValue("CoolDown", typeof(float));
            Elapsed = (float)information.GetValue("Elapsed", typeof(float));
            Power = (float)information.GetValue("Power", typeof(float));
        }

        public Special(string type, float coolDown, float power)
        {
            Type = type;
            CoolDown = coolDown;
            Power = power;
        }

        public bool Complete
        {
            get;
            set;
        }

        public float CoolDown { get; set; }

        public float Elapsed { get; set; }

        public float Power { get; set; }

        public string Type { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type);
            info.AddValue("CoolDown", CoolDown);
            info.AddValue("Elapsed", Elapsed);
            info.AddValue("Power", Power);
        }

        internal void Execute(float elapsed, ref Unit owner, ref GameState gameState)
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
                        foreach (Block b in gameState.Blocks)
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
    }
}