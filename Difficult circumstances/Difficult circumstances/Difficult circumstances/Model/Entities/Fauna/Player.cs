using Difficult_circumstances.Model.Entities.Objects;
using Difficult_circumstances.Model.Entities.Properties;
using System.Collections.Generic;
using System.Linq;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Fauna
{
    public class Player : Creature, IIlluminator
    {
        public override void Update()
        {
        }

        public readonly List<IEntity> Inventory = new List<IEntity>();

        public Player()
        {
            Width = 15;
            Height = 15;
            VisionRadius = 4;

            Health = MaxHealth = 10;
            ProvidesFoodType = Food.Meat;
            Damage = 1;
            DesiredFood = Food.Meat | Food.Fruit;
            IlluminationRange = 1;
            IlluminatedTiles = new List<Tile>();
        }

        public void Attack(object context)
        {
            IAnimate target = (context as IAnimate);
            target.Health -= Damage;
            Health -= target.Damage;
        }

        public void Stay(object context)
        {
            if (Health < MaxHealth)
            {
                Health++;
            }
        }

        internal void Eat(object context)
        {
            IEdible edible = (context as IEdible);
            CurrentHunger -= edible.GetEaten();

            if (Inventory.Contains(context as IEntity))
            {
                Inventory.Remove(context as IEntity);
            }
        }

        internal void PickUp(object context)
        {
            IEntity item = context as IEntity;

            Inventory.Add(item);
            CurrentTile.TileContents.Remove(item);
        }

        internal void Build(object context)
        {
            CurrentTile.AddContent(context as IEntity);
            Inventory.Remove(Inventory.First(t => t is Rock));
        }

        internal void Use(object context)
        {
            IUseable usable = context as IUseable;
            usable.Use(CurrentTile);
        }

        public float IlluminationRange { get; set; }
        public List<Tile> IlluminatedTiles { get; set; }

        public void Illuminate()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.AddRange(AdjacentTiles);
            tiles.Add(CurrentTile);

            IlluminationHelper.Illuminate(this, tiles, IlluminatedTiles);
        }
    }
}