using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities
{
    public interface IEntity
    {
        string Name { get; set; }

        Tile CurrentTile { get; set; }

        int Width { get; set; }

        int Height { get; set; }
    }
}