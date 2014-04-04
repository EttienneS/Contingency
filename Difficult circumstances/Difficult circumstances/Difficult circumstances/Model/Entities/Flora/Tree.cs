using System.Linq;
using Difficult_circumstances.Model.Entities.Objects;
using Difficult_circumstances.Model.Entities.Properties;

namespace Difficult_circumstances.Model.Entities.Flora
{
    internal class Tree : LivingEntity, ILiving
    {
        public Tree()
        {
            Name = GetType().Name;
            Health = MaxHealth = 10;

            Width = 10;
            Height = 30;

        }

        public override void Update()
        {
            if (CurrentTile.TileContents.Count(c => c is Apple) < 5)
            {
                if (MathHelper.Random.Next(1, 10) > 8)
                {
                    CurrentTile.AddContent(new Apple());
                }
            }
           
        }

        public short Health { get; set; }

        public short MaxHealth { get; set; }
    }
}