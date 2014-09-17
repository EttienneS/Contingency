using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Objects
{
    internal class Rock : IEntity, IItem
    {
        public Rock()
        {
            Name = GetType().Name;
            Height = Width = 3;
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


        public PropertyList Properties { get; set; }
    }
}