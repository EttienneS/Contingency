using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Objects
{
    internal class Rock : IEntity, IItem
    {
        public Rock()
        {
            Name = GetType().Name;
            Width = 5;
            Height = 5;

        }


        public bool CanLoot()
        {
            return true;
        }

        public string Name { get; set; }

        public Tile CurrentTile { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}