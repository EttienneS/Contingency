using System.Collections.Generic;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public class Menu : Sprite
    {
        public Menu()
        {
            Width = 60;
            Height = 60;
            CollisionRadius = 30;
            Visible = false;
        }

        public bool Visible { get; set; }

        private Dictionary<string, Rectangle> Quads
        {
            get
            {
                Dictionary<string, Rectangle> quads = new Dictionary<string, Rectangle>();
                int menuX = (int)Location.X;
                int menuY = (int)Location.Y;
                int quadWidth = Width / 2;
                int quadHeight = Height / 2;

                quads.Add("Attack", new Rectangle(menuX, menuY, quadWidth, quadHeight));                               // topleft
                quads.Add("Move", new Rectangle(menuX + quadWidth, menuY, quadWidth, quadHeight));                  // topRight
                quads.Add("Stop", new Rectangle(menuX, menuY + quadHeight, quadWidth, quadHeight));               // bottomLeft
                quads.Add("Special", new Rectangle(menuX + quadWidth, menuY + quadHeight, quadWidth, quadHeight));  // bottomRight

                return quads;
            }
        }

        public string GetMenuSelection(Vector2 mouseVector)
        {
            Rectangle mouse = new Rectangle((int)mouseVector.X, (int)mouseVector.Y, 1, 1);
            foreach (var kvpQuad in Quads)
            {
                if (kvpQuad.Value.Intersects(mouse))
                {
                    Visible = false;
                    return kvpQuad.Key;
                }
            }

            return string.Empty;
        }

        public Texture2D GetSprite()
        {
            return SpriteList.ContentSprites["menu"];
        }
    }
}