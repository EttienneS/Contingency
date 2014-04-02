using System;
using System.Collections.Generic;
using Difficult_circumstances.Model.Entity.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entity.Creatures
{
    public abstract class Creature : EntityBase, ISighted, IMobile, IAnimate, IFeeder, IEdible
    {
        protected Creature()
        {
            Name = GetType().Name + ":" + NameGenerator.GenerateName();
            NutritionalValue = 50;
            ProvidesFoodType = Food.Meat;
            HungerRate = 1;

            Damage = 1;
        }

        public List<Tile> AdjacentTiles { get; set; }

        public short CurrentHunger { get; set; }

        public Food DesiredFood { get; set; }

        public short Health { get; set; }

        public short Height { get; set; }

        public short HungerRate { get; set; }

        public short MaxHealth { get; set; }

        public List<Tile> VisibleTiles { get; set; }

        public short VisionRadius { get; set; }

        public short Width { get; set; }

        public bool Move(Tile targetTile)
        {
            // always 0, 1 or 2
            int distance = Math.Abs((CurrentTile.X - targetTile.X)) + Math.Abs((CurrentTile.Y - targetTile.Y));

            if (distance <= 2)
            {
                CurrentTile.Move(this, targetTile);

                return true;
            }

            return false;
        }

        public override void TurnComplete()
        {
          
        }

        public Food ProvidesFoodType { get; set; }

        public short NutritionalValue { get; set; }

        public short GetEaten()
        {
            CurrentTile.TileContents.Remove(this);
            return NutritionalValue;
        }

        public short Damage { get; set; }

    }
}