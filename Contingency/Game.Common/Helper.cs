using System;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Contingency
{
    public static class Helper
    {
        public static Random Rand = new Random();

        public static float NextFloat()
        {
            var buffer = new byte[4];
            Rand.NextBytes(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static void AddAttribute(XmlDocument doc, XmlNode node, string att, string value)
        {
            if (string.IsNullOrEmpty(value) || value == "0")
                return;

            node.Attributes.Append(doc.CreateAttribute(att));
            node.Attributes[node.Attributes.Count - 1].Value = value;
        }

        public static void AddAttribute(XmlDocument doc, XmlNode node, string att, Vector2 value)
        {
            string concatVar = value.X + "|" + value.Y;
            AddAttribute(doc, node, att, concatVar);
        }

        public static void AddAttribute(XmlDocument doc, XmlNode node, string att, Rectangle value)
        {
            string concatVar = value.X + "|" + value.Y + "|" + value.Width + "|" + value.Height;
            AddAttribute(doc, node, att, concatVar);
        }

        internal static string GetAttribute(XmlNode node, string name)
        {
            XmlAttribute x = node.Attributes[name];

            if (x == null)
            {
                return "0";
            }

            return x.Value;
        }

        internal static Vector2 GetVectorAttribute(XmlNode node, string p)
        {
            if (node.Attributes[p] != null)
            {
                string[] x = node.Attributes[p].Value.Split('|');
                return new Vector2(float.Parse(x[0]), float.Parse(x[1]));
            }
            return new Vector2(0, 0);
        }

        internal static Rectangle GetRectangleAttribute(XmlNode node, string p)
        {
            if (node.Attributes[p] != null)
            {
                string[] x = node.Attributes[p].Value.Split('|');
                return new Rectangle(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2]), int.Parse(x[3]));
            }
            return new Rectangle(0, 0, 0, 0);
        }
    }
}