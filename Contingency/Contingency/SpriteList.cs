using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency
{
    public static class SpriteList
    {
        public static Dictionary<string, Texture2D> ContentSprites = new Dictionary<string, Texture2D>();

        public static SpriteFont Font { get; set; }

        public static void LoadSprites(ContentManager Content)
        {
            SpriteList.ContentSprites.Add("unitRed", Content.Load<Texture2D>("unitRed"));
            SpriteList.ContentSprites.Add("unitBlue", Content.Load<Texture2D>("unitBlue"));
            SpriteList.ContentSprites.Add("unitRedSelected", Content.Load<Texture2D>("unitRedSelected"));
            SpriteList.ContentSprites.Add("unitBlueSelected", Content.Load<Texture2D>("unitBlueSelected"));
            SpriteList.ContentSprites.Add("projectileRed", Content.Load<Texture2D>("projectileRed"));
            SpriteList.ContentSprites.Add("projectileBlue", Content.Load<Texture2D>("projectileBlue"));
            SpriteList.ContentSprites.Add("explosion", Content.Load<Texture2D>("explosion"));
            SpriteList.ContentSprites.Add("targetMove", Content.Load<Texture2D>("targetMove"));
            SpriteList.ContentSprites.Add("targetAttack", Content.Load<Texture2D>("targetAttack"));
            SpriteList.ContentSprites.Add("block", Content.Load<Texture2D>("block"));

            SpriteList.Font = Content.Load<SpriteFont>("Font");
        }
    }
}