using System;
using System.Collections.Generic;
using System.Xml;
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

        internal XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Order/>");

            Helper.AddAttribute(doc, doc.DocumentElement, "Type", Type.ToString());
            Helper.AddAttribute(doc, doc.DocumentElement, "Target", Target);

            return doc.DocumentElement;
        }

        public Order()
        { }

        internal static Order Deserialize(XmlNode node)
        {
            Order p = new Order
            {
                Type = (OrderType)Enum.Parse(typeof(OrderType), Helper.GetAttribute(node, "Type")),
                Target = Helper.GetVectorAttribute(node, "Target"),
            };

            return p;
        }
    }

    public class OrderList : List<Order>
    {
        public XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<OrderList/>");

            foreach (Order o in this)
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(o.Serialize(), true));
            }

            return doc.DocumentElement;
        }

        internal static OrderList Deserialize(XmlNode node)
        {
            OrderList o = new OrderList();

            foreach (XmlNode child in node.ChildNodes)
            {
                o.Add(Order.Deserialize(child));
            }

            return o;
        }
    }

    public enum OrderType
    {
        None,
        Move,
        Attack,
        Special
    }
}