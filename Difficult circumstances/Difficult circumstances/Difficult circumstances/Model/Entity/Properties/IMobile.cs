using System.Collections.Generic;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entity.Properties
{
    public interface IMobile
    {
       
        List<Tile> AdjacentTiles { get; set; }

        bool Move(Tile t);
    }
}
