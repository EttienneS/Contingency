using System.Collections.Generic;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Properties
{
    public interface IIlluminator
    {
        float IlluminationRange { get; set; }

        List<Tile> IlluminatedTiles { get; set; }

        void Illuminate();
    }

    public static class IlluminationHelper
    {
        public static void Illuminate(IIlluminator i, List<Tile> adjacentTiles, List<Tile> illuminatedTiles, bool preserveLight = false)
        {
            if (adjacentTiles != null)
            {
                if (!preserveLight)
                {
                    foreach (Tile t in illuminatedTiles)
                    {
                        if (!adjacentTiles.Contains(t))
                        {
                            t.Illuminators.Remove(i);
                        }
                    }
                    illuminatedTiles.Clear();
                }

                foreach (var tile in adjacentTiles)
                {
                    illuminatedTiles.Add(tile);
                    if (!tile.Illuminators.Contains(i))
                        tile.Illuminators.Add(i);
                }
            }
        }

        public static void Deluminate(IIlluminator i)
        {
            foreach (Tile t in i.IlluminatedTiles)
            {
                if (t.Illuminators.Contains(i))
                    t.Illuminators.Remove(i);
            }
        }

    }

}