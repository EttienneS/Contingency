using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entities
{
    public abstract class LivingEntity : IEntity
    {
        public string Name { get; set; }

        public Tile CurrentTile { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public abstract void Update();
    }
}