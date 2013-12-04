using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Contingency
{
    [Serializable]
    public class Order : ISerializable
    {
        public OrderType Type { get; set; }

        public Vector2 Target { get; set; }

        public Order(OrderType type, Vector2 target)
        {
            Type = type;
            Target = target;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type);
            info.AddValue("Target", Target);
        }

        public Order(SerializationInfo information, StreamingContext context)
        {
            Type = (OrderType)information.GetValue("Type", typeof(OrderType));
            Target = (Vector2)information.GetValue("Target", typeof(Vector2));
        }
    }

    public enum OrderType
    {
        None, Move, Attack
    }
}