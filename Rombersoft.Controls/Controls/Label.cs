using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class Label:Control
    {
        string _text;
        Vector2 _vectorPosition, _vectorShadow;
        Color _color, _shadowColor;

        public Color Color
        {
            get { return _color; }
            set 
            {
                _color = value; 
                _shadowColor = GetInverse(value);
            }
        }

        public bool WithShadow { get; set; }

        public Label(int x, int y)
        {
            _color = ControlResources.DefaultColor;
            _shadowColor = GetInverse(_color);
            _vectorPosition = new Vector2(x, y);
            _text = "";
            Rect = new Rectangle(x, y, 0, 0);
            SetBounds();
        }

        public string Text
        {
            set
            {
                _text = value;
                SetBounds();
            }
            get { return _text; }
        }

        private void SetBounds()
        {
            try
            {
                Rect.Width = (int)Font.MeasureString(_text).X;
                Rect.Height = (int)Font.MeasureString(_text).Y;
                float textOffset = Font.MeasureString("A").Y / 25f;
                _vectorShadow = _vectorPosition + new Vector2(textOffset, textOffset);
            }
            catch (Exception)
            {
                string message = string.Format("Строка <{0}> содержит недопустимые символы.", _text);
                throw new Exception(message);
            }
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
            if (!Visibled) return;
            try
            {
                if(WithShadow) spriteBatch.DrawString(Font, _text, _vectorShadow, _shadowColor);
                spriteBatch.DrawString(Font, _text, _vectorPosition, _color);
            }
            catch(ArgumentException ex)
            {
                throw new Exception("Ошибка при отображении текста <" + _text + "> " +  ex.ToString());
            }
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _vectorPosition.X += x;
            _vectorPosition.Y += y;
            _vectorShadow.X += x;
            _vectorShadow.Y += y;
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            SetBounds();
        }
    }
}