using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public static class ControlResources
    {
        public static SpriteFont DefaultFont;
        public static Dictionary<string, Rectangle> AtlasRegions;
        public static Dictionary<string, SpriteFont> Fonts;
        public static Texture2D Atlas;
        public static Texture2D Brush;
        public static Color DisabledColor, DefaultColor;
    }
}
