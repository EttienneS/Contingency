using System;
using System.Xml;

namespace Contingency.Units
{
    public class Special
    {
        public Special(string type, float cooldown, float power)
        {
            Type = type;
            Cooldown = cooldown;
            Power = power;
        }

        public bool Complete { get; set; }

        public float Cooldown { get; set; }

        public float Elapsed { get; set; }

        public float Power { get; set; }

        public string Type { get; set; }

        public void Execute(float elapsed, ref Unit owner, ref GameState gameState)
        {
            Elapsed += elapsed;
            switch (Type.ToLower())
            {
                case "blink":
                    if (Elapsed > Cooldown)
                    {
                        owner.TargetAngle = (float)Math.Atan2(owner.Location.Y - owner.CurrentOrder.Target.Y, owner.Location.X - owner.CurrentOrder.Target.X);
                        owner.CurrentAngle = owner.TargetAngle;
                        owner.Location = owner.CurrentOrder.Target;
                        Complete = true;

                        owner.Hit(new Projectile(5), null);

                        bool landedInSomething = false;
                        foreach (Block b in gameState.Blocks)
                        {
                            if (owner.Touches(b))
                            {
                                b.Hit(new Projectile(int.MaxValue), null);
                                landedInSomething = true;
                            }
                        }

                        if (landedInSomething)
                        {
                            owner.Hit(new Projectile(owner.MaxHP / 3), null);
                        }
                    }
                    break;
            }
        }

        internal XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Special/>");

            Helper.AddAttribute(doc, doc.DocumentElement, "Complete", Complete.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Cooldown", Cooldown.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Elapsed", Elapsed.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Power", Power.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Type", Type);

            return doc.DocumentElement;
        }

        public Special()
        { }

        internal static Special Deserialize(XmlNode node)
        {
            Special p = new Special
            {
                Complete = bool.Parse(Helper.GetAttribute(node, "Complete")),
                Cooldown = float.Parse(Helper.GetAttribute(node, "Cooldown")),
                Elapsed = float.Parse(Helper.GetAttribute(node, "Elapsed")),
                Power = float.Parse(Helper.GetAttribute(node, "Power")),
                Type = Helper.GetAttribute(node, "Type"),
            };

            return p;
        }
    }
}