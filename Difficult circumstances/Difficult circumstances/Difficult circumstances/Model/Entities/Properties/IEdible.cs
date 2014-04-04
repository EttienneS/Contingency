using System;

namespace Difficult_circumstances.Model.Entities.Properties
{
    public interface IEdible
    {
        Food ProvidesFoodType { get; set; }

        short NutritionalValue { get; set; }

        short GetEaten();

        string FoodName { get; set; }

        bool CanBeEaten();
    }

    [Flags]
    public enum Food : int
    {
        Meat = 1, Fruit = 2, Grass = 4
    }
}