using System.Collections.Generic;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities.Properties
{
    public interface IUseable
    {
        void Use(Tile currentTile);
    }

}