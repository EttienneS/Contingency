using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public static class SpriteList
    {
        public static Dictionary<string, Texture2D> ContentSprites = new Dictionary<string, Texture2D>();

        public static SpriteFont Font { get; set; }

        public static void LoadSprites(ContentManager content)
        {
            ContentSprites.Add("unitRed", content.Load<Texture2D>("unitRed"));
            ContentSprites.Add("unitBlue", content.Load<Texture2D>("unitBlue"));
            ContentSprites.Add("unitRedSelected", content.Load<Texture2D>("unitRedSelected"));
            ContentSprites.Add("unitBlueSelected", content.Load<Texture2D>("unitBlueSelected"));
            ContentSprites.Add("projectileRed", content.Load<Texture2D>("projectileRed"));
            ContentSprites.Add("projectileBlue", content.Load<Texture2D>("projectileBlue"));
            ContentSprites.Add("explosion", content.Load<Texture2D>("explosion"));
            ContentSprites.Add("targetMove", content.Load<Texture2D>("targetMove"));
            ContentSprites.Add("targetAttack", content.Load<Texture2D>("targetAttack"));
            ContentSprites.Add("block", content.Load<Texture2D>("block"));
            ContentSprites.Add("menu", content.Load<Texture2D>("menu"));
            ContentSprites.Add("start", content.Load<Texture2D>("start"));

            Font = content.Load<SpriteFont>("Font");
        }
    }
}