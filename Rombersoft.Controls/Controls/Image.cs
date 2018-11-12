using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class Image : Control
    {
        private Rectangle _rectSource;
        private Vector2 _position;
        bool _scaled;

        public Image(int x, int y, string atlasRegion)
        {
            _position = new Vector2(x, y);
            _rectSource = ControlResources.AtlasRegions[atlasRegion];
            Rect = new Rectangle(x, y, _rectSource.Width, _rectSource.Height);
            _scaled = false;
        }

        public Image(int x, int y, int width, int height, string atlasRegion)
        {
            Rect = new Rectangle(x, y, width, height);
            _rectSource = ControlResources.AtlasRegions[atlasRegion];
            _position = new Vector2(x, y);
            _scaled = true;
        }

        public override bool ClickInControl()
        {
            return false;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_scaled) spriteBatch.Draw(ControlResources.Atlas, Rect, _rectSource, Color.White);
            else spriteBatch.Draw(ControlResources.Atlas, _position, _rectSource, Color.White);
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _position.X += x;
            _position.Y += y;
        }
    }
}