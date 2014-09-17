using Difficult_circumstances.Model.Entities.Properties;

namespace Difficult_circumstances.Model.Entities.Flora
{
    internal class Grass : LivingEntity, ILiving, IEdible
    {
        public Grass()
        {
            Name = GetType().Name;
            NutritionalValue = 5;
            ProvidesFoodType = Food.Grass;
            Health = MaxHealth = 10;

            Lenght = (short)MathHelper.Random.Next(1, 15);
            Width = Height = Lenght;

            FoodName = Name;
        }

        public short Lenght { get; set; }

        public override void Update()
        {
            if (Lenght < 15)
            {
                if (MathHelper.Random.Next(1, 100) > 95)
                {
                    Lenght++;
                }
                Width = Height = Lenght;
            }
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

        public string FoodName { get; set; }

        public bool CanBeEaten()
        {
            return Lenght > 0;
        }

        public short Health { get; set; }

        public short MaxHealth { get; set; }
    }
}