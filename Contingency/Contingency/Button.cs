using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public class Button
    {
        public Button(string name, int widht, int height, string text, Vector2 location, Color color, Color textColor, GraphicsDevice device)
        {
            Width = widht;
            Height = height;
            Location = location;
            ButtonColor = color;
            TextColor = textColor;
            Text = text;

            Sprite = new Texture2D(device, Width, height);
            Color[] red = new Color[Width * height];
            for (int i = 0; i < Width * height; i++)
                red[i] = color;

            Sprite.SetData(red);
            Visible = true;
            Name = name;
        }

        public event EventHandler<EventArgs> ButtonClicked;

        public Color ButtonColor { get; set; }

        public int Height { get; set; }

        public Vector2 Location { get; set; }

        public Texture2D Sprite { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; }

        public bool Visible { get; set; }

        public int Width { get; set; }

        public string Name { get; set; }

        public void CheckClicked(Vector2 mouseVector)
        {
            Rectangle mouse = new Rectangle((int)mouseVector.X, (int)mouseVector.Y, 1, 1);
            Rectangle box = new Rectangle((int)Location.X, (int)Location.Y, Width, Height);

            if (mouse.Intersects(box))
            {
                EventHandler<EventArgs> handler = ButtonClicked;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
        }
    }

    public class ButtonList : List<Button>
    {
        public Button this[string buttonName]
        {
            get { return this.First(btn => btn.Name == buttonName); }
        }
    }
}