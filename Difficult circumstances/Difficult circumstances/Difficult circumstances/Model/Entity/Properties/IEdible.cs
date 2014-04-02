using System;

namespace Difficult_circumstances.Model.Entity.Properties
{
    public interface IEdible
    {
        Food ProvidesFoodType { get; set; }

        short NutritionalValue { get; set; }

        short GetEaten();
    }

    [Flags]
    public enum Food : int
    {
        Meat = 1, Fruit = 2, Grass = 4
    }
}