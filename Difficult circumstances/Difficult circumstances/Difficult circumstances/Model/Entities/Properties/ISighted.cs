using System.Collections.Generic;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Properties
{
    internal interface ISighted
    {
        short VisionRadius { get; set; }

        List<Tile> VisibleTiles { get; set; }
    }
}