using System.Collections.Generic;
using Difficult_circumstances.Model.Entities.Properties;
using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities
{
    public interface IEntity
    {
        string Name { get; set; }

        Tile CurrentTile { get; set; }

        int Width { get; set; }

        int Height { get; set; }

        bool Illuminated { get; set; }

        PropertyList Properties { get; set; }
    }

    public interface IProperty
    {
        void Update();
    }

    public class PropertyList : List<IProperty>
    {
        
    }


}