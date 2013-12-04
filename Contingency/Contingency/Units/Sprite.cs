using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    [Serializable]
    public abstract class Sprite 
    {
        public Sprite(){}

        public Vector2 Momentum = new Vector2(0f);
        public float TargetAngle;
        public const float TurnSpeed = 0.05f;

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public string Team { get; set; }

        
        private double _collisionRadius;

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

        public float CurrentAngle { get; set; }

        public int Height { get; set; }

        public Vector2 Location { get; set; }

        public int Width { get; set; }

        public static bool AlmostEquals(float f1, float f2, double precision)
        {
            return (Math.Abs(f1 - f2) <= precision);
        }

        public abstract Texture2D GetSprite();

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
            Vector2 Distance = this.Location - location;
            if (Distance.Length() < this.CollisionRadius + radius) // if the distance is less than the diameter
                return true;
            return false;
        }

        public bool TouchesWithOffset(Vector2 location, double radius, float offset)
        {
            location = new Vector2(location.X - offset, location.Y - offset);

            Vector2 Distance = this.Location - location;
            if (Distance.Length() < this.CollisionRadius + radius) // if the distance is less than the diameter
                return true;
            return false;
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
                if (increasingAngle == true)
                    CurrentAngle += TurnSpeed;
                else
                    CurrentAngle -= TurnSpeed;
            }

            Location += Momentum;
        }

        
    }
}