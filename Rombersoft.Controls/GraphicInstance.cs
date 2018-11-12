using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Rombersoft.Controls
{
    public class GraphicInstance
    {
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch{ get; set; }
        public static GraphicsDevice Device { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }
        public static Rectangle Window { get; set; }
    }
}
