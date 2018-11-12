using System;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Rombersoft.Controls
{
    public class Button:Control
    {
        private Vector2 _posText1, _posShadowText;
        private string _text;
        private Texture2D _image;
        private SpriteEffects _spriteEffects;
        public Color BackColor;
        Color _textColor, _textShadowColor, _imageColor;
        private Rectangle RectImg, _mainView;
        private Rectangle ? _imageR;
        public event Action OnClicked, OnPressed, OnReleased;

        private bool _isPressed, _thisPressed, _scaled, _mouseButtonPressed, _mouseInside;
        private double _lastPressed;
        private float _scale;
        private Align _align = Align.Center;
        public bool WithShadow { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param nameRU="x">координата Х</param>
        /// <param nameRU="y">координата Y</param>
        /// <param nameRU="width">ширина</param>
        /// <param nameRU="height">высота</param>
        /// <param nameRU="mainView">рисунок в режиме ожидания</param>
        /// <param nameRU="isInFocus">рисунок в фокусе</param>
        /// <param nameRU="pressed">рисунок нажатой кнопки</param>
        /// <param nameRU="disabled">рисунок недоступной кнопки</param>
        /// <param nameRU="nameRU">название кнопки</param>
        /// <param nameRU="action">действие при нажатии</param>
        public Button(int x, int y, int width, int height, string mainView, string name)
        {
            _text = "";
            width = width == 0 ? ControlResources.AtlasRegions[mainView].Width : width;
            height = height == 0 ? ControlResources.AtlasRegions[mainView].Height : height;
            _mainView = ControlResources.AtlasRegions[mainView];
            BackColor = Color.White;
            _textColor = Color.Black;
            _textShadowColor = GetInverse(_textColor);
            Rect = new Rectangle(x, y, width, height);
            Name = name;
            _posShadowText = new Vector2(2, 2);
            _lastPressed = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
            _scale = 1;
        }

        public Button(int x, int y, int width, int height, string mainView, string name, Color background, SpriteEffects effect):
            this(x, y, width, height, mainView, name)
        {
            _spriteEffects = effect;
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                if (Math.Abs(value - Math.Truncate(value)) <= 0.01)
                    _scaled = false;
                else
                    _scaled = true;
            }
        }

        public string Unit { get; set; }

        public bool HoldTimeBeforeNext { get; set; }

        public Texture2D Image
        {
            get { return _image; }
            set
            {
                if(value == null) throw new Exception("Не возможно присвоить картинке нулевое положение");
                _image = value;
                RectImg = new Rectangle(Rect.X + (Rect.Width - value.Width)/2, Rect.Y + (Rect.Height - value.Height)/2, value.Width, value.Height);
            }
        }

        public void ImageR(Rectangle? img, Color? color = null)
        {
            _imageR = img;
            if (!_imageR.HasValue) throw new Exception("Не возможно присвоить картинке нулевую ссылку");
            RectImg = new Rectangle(Rect.X + (Rect.Width - _imageR.Value.Width) / 2, Rect.Y + (Rect.Height - _imageR.Value.Height) / 2, _imageR.Value.Width, _imageR.Value.Height);
            _imageColor = color.HasValue ? color.Value : Color.White;
        }

        public void Text(string text)
        {
            _text = text;
            SetPosText();
        }

        public string GetText()
        {
            return _text;
        }

        public void PropertyText(Align align, SpriteFont font, Color color)
        {
            Font = font;
            _align = align;
            _textColor = color;
            _textShadowColor = GetInverse(_textColor);
            SetPosText();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override bool ClickInControl()
        {
            if (!Enabled) return false;
            _mouseInside = false;
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (_mouseButtonPressed) return _thisPressed;
                if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                {
                    if (!_isPressed)
                    {
                        _thisPressed = true;
                        _isPressed = true;
                        if (OnPressed != null) OnPressed();
                    }
                    _mouseInside = true;
                }
                else _mouseButtonPressed = true;
            }
            else
            {
                _mouseButtonPressed = false;
                if (_thisPressed)
                {
                    _isPressed = false;
                    _thisPressed = false;
                    if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                    {
                        if (HoldTimeBeforeNext)
                        {
                            double newSubtract = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
                            if (Math.Abs(newSubtract - _lastPressed) > 300)
                            {
                                if (OnReleased != null) OnReleased();
                                if (OnClicked != null) OnClicked();
                                _lastPressed = newSubtract;
                            }
                        }
                        else
                        {
                            if (OnReleased != null) OnReleased();
                            if (OnClicked != null) OnClicked();
                        }
                        _mouseInside = true;
                    }
                }
                else _isPressed = false;
            }
            return _mouseInside;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_scaled)
            {
                if (_thisPressed) spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, BackColor, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                else spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, BackColor, 0, Vector2.Zero, _spriteEffects, 1);
                if (Image != null) spriteBatch.Draw(Image, RectImg, Color.White);
                else if (_imageR.HasValue) spriteBatch.Draw(ControlResources.Atlas, RectImg, _imageR, Color.White);
                if (!String.IsNullOrEmpty(_text))
                {
                    if (WithShadow) spriteBatch.DrawString(Font, _text, _posText1 + _posShadowText, _textShadowColor);
                    spriteBatch.DrawString(Font, _text, _posText1, _textColor);
                }
                if (!Enabled) spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, ControlResources.DisabledColor);
            }
            else
            {
                if (_thisPressed) spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, BackColor, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                else
                {
                    float newWidth = Rect.Width * _scale;
                    float newHeight = Rect.Height * _scale;
                    float x = Rect.X - (Rect.Width * _scale - Rect.Width) / 2;
                    float y = Rect.Y - (Rect.Height * _scale - Rect.Height) / 2;
                    Rectangle newRect = new Rectangle((int)x, (int)y, (int)newWidth, (int)newHeight);
                    spriteBatch.Draw(ControlResources.Atlas, newRect, _mainView, BackColor, 0, Vector2.Zero, _spriteEffects, 1);
                }
                if (Image != null) spriteBatch.Draw(Image, RectImg, Color.White);
                else if (_imageR.HasValue) spriteBatch.Draw(ControlResources.Atlas, RectImg, _imageR, Color.White);
                if (!String.IsNullOrEmpty(_text))
                {
                    if (WithShadow)
                        spriteBatch.DrawString(Font, _text, _posText1 + _posShadowText, _textShadowColor);
                    spriteBatch.DrawString(Font, _text, _posText1, _textColor);
                }
                if (!Enabled) spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, ControlResources.DisabledColor);
            }
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            if ((_image != null) || (_imageR.HasValue))
            {
                RectImg.Offset(x, y);
            }
            _posText1.X += x;
            _posText1.Y += y;
        }

        public override void SetPosition(int x, int y)
        {
            _posText1.X += x - Rect.X;
            _posText1.Y += y - Rect.Y;
            RectImg.Offset(x-Rect.X, y-Rect.Y);
            Rect.X = x;
            Rect.Y = y;
        }

        private void SetPosText()
        {
            if (!String.IsNullOrEmpty(_text))
            {
                Vector2 pos = Font.MeasureString(_text);
                switch (_align)
                {
                    case Align.Left:
                        _posText1 = new Vector2(Rect.Width / 7 + Rect.X, (Rect.Height - pos.Y) / 2 + Rect.Y);
                        break;
                    case Align.Right:
                        _posText1 = new Vector2(Rect.Width - pos.X + Rect.X - Rect.Width / 7, (Rect.Height - pos.Y) / 2 + Rect.Y);
                        break;
                    case Align.Center:
                        _posText1 = new Vector2((Rect.Width - pos.X) / 2 + Rect.X, (Rect.Height - pos.Y)/2 + Rect.Y);
                        break;
                }
            }
        }
    }

    public enum Align:byte
    {
        Left,
        Right,
        Center
    }
}