using Difficult_circumstances.Model.Map;

namespace Difficult_circumstances.Model.Entity
{
    public abstract class EntityBase
    {
        public string Name { get; set; }

        public Tile CurrentTile { get; set; }

        public abstract void Update();

        // anything that needs to be reset or changed once a turn has ended goes here
        public abstract void TurnComplete();
    }
}