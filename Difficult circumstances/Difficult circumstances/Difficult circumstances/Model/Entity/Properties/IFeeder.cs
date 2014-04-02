namespace Difficult_circumstances.Model.Entity.Properties
{
    public interface IFeeder
    {
        short CurrentHunger { get; set; }

        short HungerRate { get; set; }

        Food DesiredFood { get; set; }
    }
}