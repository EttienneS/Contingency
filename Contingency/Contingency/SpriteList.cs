using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public static class SpriteList
    {
        public static readonly Dictionary<string, Texture2D> ContentSprites = new Dictionary<string, Texture2D>();

        public static SpriteFont Font { get; set; }

        public static void LoadSprites(ContentManager content)
        {
            ContentSprites.Add("blinkerRed", content.Load<Texture2D>("blinkerRed"));
            ContentSprites.Add("blinkerBlue", content.Load<Texture2D>("blinkerBlue"));
            ContentSprites.Add("blinkerRedSelected", content.Load<Texture2D>("blinkerRedSelected"));
            ContentSprites.Add("blinkerBlueSelected", content.Load<Texture2D>("blinkerBlueSelected"));

            ContentSprites.Add("builderRed", content.Load<Texture2D>("builderRed"));
            ContentSprites.Add("builderBlue", content.Load<Texture2D>("builderBlue"));
            ContentSprites.Add("builderRedSelected", content.Load<Texture2D>("builderRedSelected"));
            ContentSprites.Add("builderBlueSelected", content.Load<Texture2D>("builderBlueSelected"));

            ContentSprites.Add("projectileRed", content.Load<Texture2D>("projectileRed"));
            ContentSprites.Add("projectileBlue", content.Load<Texture2D>("projectileBlue"));
            ContentSprites.Add("explosion", content.Load<Texture2D>("explosion"));
            ContentSprites.Add("targetMove", content.Load<Texture2D>("targetMove"));
            ContentSprites.Add("targetAttack", content.Load<Texture2D>("targetAttack"));
            ContentSprites.Add("block", content.Load<Texture2D>("block"));
            ContentSprites.Add("menu", content.Load<Texture2D>("menu"));
            ContentSprites.Add("start", content.Load<Texture2D>("start"));
            ContentSprites.Add("join", content.Load<Texture2D>("join"));
            ContentSprites.Add("background", content.Load<Texture2D>("background"));

            Font = content.Load<SpriteFont>("Font");
        }
    }
}