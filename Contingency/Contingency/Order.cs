using Microsoft.Xna.Framework;

namespace Contingency
{
    public class Order
    {
        public OrderType Type { get; set; }

        public Vector2 Target { get; set; }

        public Order(OrderType type, Vector2 target)
        {
            Type = type;
            Target = target;
        }
    }

    public enum OrderType
    {
        None, Move, Attack
    }
}