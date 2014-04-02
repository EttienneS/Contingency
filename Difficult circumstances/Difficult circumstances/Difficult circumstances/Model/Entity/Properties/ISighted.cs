using System.Collections.Generic;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entity.Properties
{
    interface ISighted
    {
        short VisionRadius { get; set; }

        List<Tile> VisibleTiles { get; set; } 
    }
}
