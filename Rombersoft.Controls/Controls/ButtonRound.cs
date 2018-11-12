using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Rombersoft.Controls
{
    public class ButtonRound:Control
    {
        private Vector2 _posText, _posShadowText;
        private Color _currentTextColor, _colorTextShadow, _colorBackground;
        private string _text;
        private Texture2D _image;
        private Rectangle _rectImg, _mainViewBegin, _mainViewEnd, _mainViewMiddle, _rectBegin, _rectMiddle, _rectEnd, _rectEndGlass, _glassBegin,
                _glassMiddle, _glassEnd;
        private Rectangle ? _imageR;
        public event Action OnClicked, OnPressed, OnReleased;
        private bool _isPressed, _mouseInside, _thisPressed, _mouseButtonPressed;
        private double _lastPressed;
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
        public ButtonRound(int x, int y, int width, int height, string mainView, string name, Color color)
        {
            _text = string.Empty;
            float scale;
            if (height != 0) scale = (float)height / (float)160;
            else scale = 1;
            _mainViewBegin = ControlResources.AtlasRegions[mainView + "Begin"];
            _mainViewMiddle = ControlResources.AtlasRegions[mainView + "Middle"];
            _mainViewEnd = ControlResources.AtlasRegions[mainView + "End"];
            Rect = new Rectangle(x, y, width, height);
            _glassBegin = ControlResources.AtlasRegions["GlassButtonBegin"];
            _glassMiddle = ControlResources.AtlasRegions["GlassButtonMiddle"];
            _glassEnd = ControlResources.AtlasRegions["GlassButtonEnd"];
            Name = name;
            _posShadowText = new Vector2(2, 2);
            _lastPressed = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
            int widthBegin = (int)(80 * scale);
            int widthMiddle = (int)(Rect.Width - 160 * scale);
            int widthEnd = (int)(80 * scale);
            int posMiddle = Rect.X + widthBegin;
            int posEnd = posMiddle + widthMiddle;
            _rectBegin = new Rectangle(Rect.X, Rect.Y, widthBegin, (int)(160 * scale));
            _rectMiddle = new Rectangle(posMiddle, y, widthMiddle, (int)(160 * scale));
            _rectEnd = new Rectangle(posEnd, Rect.Y, widthEnd, (int)(160 * scale));
            _rectEndGlass = new Rectangle(posEnd, Rect.Y, widthEnd+1, (int)(160 * scale));
            _colorBackground = color;
            _isPressed = _mouseInside = _thisPressed = _mouseButtonPressed = false;
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
                //if((value.Width + 6 < Rect.Width)&&(value.Height + 6 < Rect.Height))
                    _rectImg = new Rectangle(Rect.X + (Rect.Width - value.Width)/2, Rect.Y + (Rect.Height - value.Height)/2, value.Width, value.Height);
                //else _rectDestIcon = new Rectangle(Rect.X + 3, Rect.Y + 3, Rect.Width-6, Rect.Height-6);
            }
        }

        public Rectangle ? ImageR
        {
            get { return _imageR; }
            set
            {
                _imageR = value;
                if (!_imageR.HasValue) throw new Exception("Не возможно присвоить картинке нулевую ссылку");
                _rectImg = new Rectangle(Rect.X + (Rect.Width - _imageR.Value.Width) / 2, Rect.Y + (Rect.Height - _imageR.Value.Height) / 2, _imageR.Value.Width, _imageR.Value.Height);
            }
        }

        public void Text(string text)
        {
            _text = text;
            SetPosText();
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _rectBegin.Offset(x, y);
            _rectMiddle.Offset(x, y);
            _rectEnd.Offset(x, y);
            _rectEndGlass.Offset(x, y);
            if ((_image != null) || (_imageR.HasValue))
            {
                _rectImg.Offset(x, y);
            }
            _posText.X += x;
            _posText.Y += y;
        }

        public void PropertyText(Align align, SpriteFont font, Color color)
        {
            _currentTextColor = color;
            _colorTextShadow = GetInverse(_currentTextColor);
            Font = font;
            _align = align;
            SetPosText();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, _rectMiddle, _mainViewMiddle, _colorBackground);
            spriteBatch.Draw(ControlResources.Atlas, _rectBegin, _mainViewBegin, _colorBackground);
            spriteBatch.Draw(ControlResources.Atlas, _rectEnd, _mainViewEnd, _colorBackground);
            if (_image != null)
                spriteBatch.Draw(_image, _rectImg, Color.White);
            if (_imageR.HasValue)
                spriteBatch.Draw(ControlResources.Atlas, _rectImg, _imageR.Value, Color.White);
            if (!String.IsNullOrEmpty(_text))
            {
                if (WithShadow) spriteBatch.DrawString(Font, _text, _posText + _posShadowText, _colorTextShadow);
                spriteBatch.DrawString(Font, _text, _posText, _currentTextColor);
            }
            if (!Enabled)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectMiddle, _mainViewMiddle, ControlResources.DisabledColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectBegin, _mainViewBegin, ControlResources.DisabledColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectEnd, _mainViewEnd, ControlResources.DisabledColor);
                if (_image != null)
                    spriteBatch.Draw(_image, _rectImg, Color.White);
                if (_imageR.HasValue)
                    spriteBatch.Draw(ControlResources.Atlas, _rectImg, _imageR.Value, Color.White);
            }
            if (!_thisPressed)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectMiddle, _glassMiddle, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectBegin, _glassBegin, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectEndGlass, _glassEnd, Color.White);
            }
            else
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectMiddle, _glassMiddle, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                spriteBatch.Draw(ControlResources.Atlas, _rectBegin, _glassBegin, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                spriteBatch.Draw(ControlResources.Atlas, _rectEndGlass, _glassEnd, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            }
        }

        public override bool ClickInControl()
        {
            if(!Enabled) return false;
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
                                if (OnClicked != null) OnClicked.Invoke();
                                _lastPressed = newSubtract;
                            }
                        }
                        else
                        {
                            if (OnReleased != null) OnReleased();
                            if (OnClicked != null) OnClicked.Invoke();
                        }
                        _mouseInside = true;
                    }
                }
                else _isPressed = false;
            }
            return _mouseInside;
        }

        private void SetPosText()
        {
            Vector2 pos = Font.MeasureString(_text);
            switch(_align)
            {
                case Align.Left:
                    _posText = new Vector2(Rect.Width/7 + Rect.X, (Rect.Height - pos.Y) / 2 + Rect.Y);
                    break;
                case Align.Right:
                    _posText = new Vector2(Rect.Width - pos.X + Rect.X - Rect.Width / 7, (Rect.Height - pos.Y) / 2 + Rect.Y);
                    break;
                case Align.Center:
                    _posText = new Vector2((Rect.Width - pos.X) / 2 + Rect.X, (Rect.Height - pos.Y) / 2 + Rect.Y);
                    break;
            }
        }
    }
}
