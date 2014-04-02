using Difficult_circumstances.Model.Entity.Properties;

namespace Difficult_circumstances.Model.Entity.Creatures
{
    public class Player : Creature
    {
        public override void Update()
        {
        }

        public Player()
        {
            Width = 25;
            Height = 25;
            VisionRadius = 4;

            Health = MaxHealth = 10;

            Damage = 5;
            DesiredFood = Food.Meat | Food.Fruit;
        }

        public void Attack(object context)
        {
            IAnimate target = (context as IAnimate);
            target.Health -= Damage;
            Health -= target.Damage;
        }

        public void Stay(object context)
        {
            if (Health < MaxHealth)
            {
                Health++;
            }
        }

        internal void Eat(object context)
        {
            IEdible edible = (context as IEdible);
            CurrentHunger -= edible.GetEaten();
        }
    }
}