using Difficult_circumstances.Model.Entity.Properties;

namespace Difficult_circumstances.Model.Entity
{
    internal class Grass : EntityBase, ILiving, IEdible
    {
        public Grass(short lenght)
        {
            Name = GetType().Name;
            NutritionalValue = 5;
            ProvidesFoodType = Food.Grass;
            Health = MaxHealth = 10;

            Lenght = lenght;
        }

        public short Lenght { get; set; }

        public override void Update()
        {
            if (Lenght < 30)
            {
                Lenght++;
            }
        }

        public override void TurnComplete()
        {
        }

        public Food ProvidesFoodType { get; set; }

        public short NutritionalValue { get; set; }

        public short GetEaten()
        {
            if (Lenght > 5)
            {
                Lenght -= 5;
                return (short)(5 * NutritionalValue);
            }

            short x = (short)(Lenght * NutritionalValue);
            Lenght = 0;
            return x;
        }

        public short Health { get; set; }

        public short MaxHealth { get; set; }
    }
}