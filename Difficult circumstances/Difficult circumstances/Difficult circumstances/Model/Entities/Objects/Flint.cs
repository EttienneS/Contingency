using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;
using System.Collections.Generic;

namespace Difficult_circumstances.Model.Entities.Objects
{
    internal class Flint : IEntity, IItem, IUseable, IIlluminator
    {

        public PropertyList Properties { get; set; }

        public Flint()
        {
            Name = GetType().Name;
            Height = Width = 3;
            IlluminatedTiles = new List<Tile>();
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

        public void Use(Tile currentTile)
        {
            List<Tile> tiles = new List<Tile> { currentTile };
            IlluminationHelper.Illuminate(this, tiles, IlluminatedTiles, true);
        }

        public float IlluminationRange { get; set; }

        public List<Tile> IlluminatedTiles { get; set; }

        public void Illuminate()
        {
        }
    }
}