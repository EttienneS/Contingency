using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Contingency.Units
{
    interface ISprite
    {
        Texture2D GetSprite(GraphicsDevice graphicsDevice);
    }
}
