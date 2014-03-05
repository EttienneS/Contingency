using System;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Contingency.Units
{
    public class Sprite
    {
        public Vector2 Momentum = new Vector2(0f);

        public float TargetAngle;

        public float TurnSpeed = 0.05f;

        private double _collisionRadius;

        public float CurrentAngle { get; set; }

        public int CurrentHP { get; set; }

        public int Height { get; set; }

        public Vector2 Location { get; set; }

        public int MaxHP { get; set; }

        public string Team { get; set; }

        public int Width { get; set; }

        public double CollisionRadius
        {
            get
            {
                if (_collisionRadius == 0)
                {
                    _collisionRadius = Width / 2;
                }
                return _collisionRadius;
            }
            set
            {
                _collisionRadius = value;
            }
        }

        public static bool AlmostEquals(float f1, float f2, double precision)
        {
            return (Math.Abs(f1 - f2) <= precision);
        }

        public void Target(Vector2 target)
        {
            TargetAngle = (float)Math.Atan2(Location.Y - target.Y, Location.X - target.X);
        }

        public bool Touches(Sprite sprite)
        {
            return Touches(sprite.Location, sprite.CollisionRadius);
        }

        public bool Touches(Vector2 location, double radius)
        {
            Vector2 distance = Location - location;
            return distance.Length() < CollisionRadius + radius;
        }

        public bool TouchesWithOffset(Vector2 location, double radius, float offset)
        {
            location = new Vector2(location.X - offset, location.Y - offset);

            Vector2 distance = Location - location;
            return distance.Length() < CollisionRadius + radius;
        }

        public void UpdateState()
        {
            // invert angle if they go over -pi and +pi (circular value)
            if (CurrentAngle > MathHelper.Pi) CurrentAngle -= MathHelper.TwoPi;
            if (TargetAngle > MathHelper.Pi) TargetAngle -= MathHelper.TwoPi;
            if (CurrentAngle < -MathHelper.Pi) CurrentAngle = MathHelper.TwoPi - CurrentAngle;
            if (TargetAngle < -MathHelper.Pi) TargetAngle = MathHelper.TwoPi - TargetAngle;

            bool increasingAngle = false;

            //four cases to consider
            //both angles are positive
            if (CurrentAngle >= 0 && TargetAngle >= 0)
            {
                if (CurrentAngle < TargetAngle)
                    increasingAngle = true;
            }
            //both angles are negative
            else if (CurrentAngle < 0 && TargetAngle < 0)
            {
                if (CurrentAngle < TargetAngle)
                    increasingAngle = true;
            }
            else
            {
                //both angles are on opposite sides of the x-axis
                if (Math.Abs(CurrentAngle - TargetAngle) < MathHelper.Pi)
                {
                    // shorter to go via zero
                    if (CurrentAngle < 0)
                        increasingAngle = true;
                }
                else
                {
                    // need to go via ±pi
                    if (CurrentAngle > 0)
                        increasingAngle = true;
                }
            }

            // we've (almost) reached our target angle
            if (AlmostEquals(CurrentAngle, TargetAngle, TurnSpeed))
            {
                //in case we are not *exactly* at the target angle
                CurrentAngle = TargetAngle;
            }
            else
            {
                //need to move currentAngle towards targetAngle
                //direction to move has been calculated  in the previous section
                if (increasingAngle)
                    CurrentAngle += TurnSpeed;
                else
                    CurrentAngle -= TurnSpeed;
            }

            Location += Momentum;
        }

        public XmlNode SpriteSerialize(XmlDocument doc, XmlNode node)
        {
            Helper.AddAttribute(doc, node, "Momentum", Momentum);
            Helper.AddAttribute(doc, node, "TargetAngle", TargetAngle.ToString());
            Helper.AddAttribute(doc, node, "TurnSpeed", TurnSpeed.ToString());
            Helper.AddAttribute(doc, node, "CollisionRadius", CollisionRadius.ToString());
            Helper.AddAttribute(doc, node, "CurrentAngle", CurrentAngle.ToString());
            Helper.AddAttribute(doc, node, "CurrentHP", CurrentHP.ToString());
            Helper.AddAttribute(doc, node, "Location", Location);
            Helper.AddAttribute(doc, node, "MaxHP", MaxHP.ToString());
            Helper.AddAttribute(doc, node, "Team", Team);
            Helper.AddAttribute(doc, node, "Width", Width.ToString());
            Helper.AddAttribute(doc, node, "Height", Height.ToString());

            return doc.DocumentElement;
        }
    }
}