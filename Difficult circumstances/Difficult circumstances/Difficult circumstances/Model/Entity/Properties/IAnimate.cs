namespace Difficult_circumstances.Model.Entity.Properties
{
    public interface IAnimate : ILiving, IAttacker
    {
    }

    public interface ILiving
    {
        short Health { get; set; }

        short MaxHealth { get; set; }
    }
}