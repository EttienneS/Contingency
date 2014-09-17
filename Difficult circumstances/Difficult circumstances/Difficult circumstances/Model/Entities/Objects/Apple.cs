using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Objects
{
    internal class Apple : IEntity, IEdible, IItem
    {
        public Apple()
        {
            Name = GetType().Name;
            NutritionalValue = 10;
            ProvidesFoodType = Food.Fruit;
            FoodName = Name;

            Height = Width = 3;
        }

        public PropertyList Properties { get; set; }

        public Food ProvidesFoodType { get; set; }

        public short NutritionalValue { get; set; }

        public short GetEaten()
        {
            CurrentTile.TileContents.Remove(this);
            return NutritionalValue;
        }

        public string FoodName { get; set; }

        public bool CanBeEaten()
        {
            return true;
        }

        public bool CanLoot()
        {
            return true;
        }

        public string Name { get; set; }

        public Tile CurrentTile { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool Illuminated { get; set; }
    }
}