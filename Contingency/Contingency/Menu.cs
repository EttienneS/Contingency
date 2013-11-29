using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contingency.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public class Menu : Sprite
    {
        public readonly Dictionary<string, string> OptionsMapping = new Dictionary<string, string>();
        public bool Visible { get; set; }

        public Menu()
        {
            Width = 60;
            Height = 60;
            CollisionRadius = 30;
            Visible = false;
        }

        public override Texture2D GetSprite()
        {
            return SpriteList.ContentSprites["menu"];
        }

        public string GetMenuSelection(Vector2 mouseVector)
        {
            Dictionary<string, Rectangle> quads = new Dictionary<string, Rectangle>();

            int menuX = (int)Location.X;
            int menuY = (int)Location.Y;
            int quadWidth = Width / 2;
            int quadHeight = Height / 2;

            quads.Add("topleft", new Rectangle(menuX, menuY, quadWidth, quadHeight));
            quads.Add("topRight", new Rectangle(menuX + quadWidth, menuY, quadWidth, quadHeight));
            quads.Add("bottomLeft", new Rectangle(menuX, menuY + quadHeight, quadWidth, quadHeight));
            quads.Add("bottomRight", new Rectangle(menuX + quadWidth, menuY + quadHeight, quadWidth, quadHeight));

            Rectangle mouse = new Rectangle((int)mouseVector.X, (int)mouseVector.Y, 1, 1);
            foreach (var kvpQuad in quads)
            {
                if (kvpQuad.Value.Intersects(mouse))
                {
                    Visible = false;
                    return OptionsMapping[kvpQuad.Key];
                }
            }

            return string.Empty;
        }
    }
}
