namespace Difficult_circumstances.Model.Entity.Creatures
{
    internal class Bluerior : Creature
    {
        public override void Update()
        {
        }

        public Bluerior()
        {
            Width = 15;
            Height = 25;
            VisionRadius = 3;
            MaxHealth = Health = 6;
        }
    }
}