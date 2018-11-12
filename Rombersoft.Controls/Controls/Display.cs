using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Rombersoft.Controls
{
    public class Display : Control
    {
        private Rectangle _rectSouceBegin, _rectSourceMiddle, _rectSourceEnd, _rectDestBegin, _rectDestMiddle, _rectDestEnd;
        private Vector2  _vectorText, _vectorCursor, _vectorDestShadow;
        private int _yText, _xText;
        private string _text;
        private int  _lastPressed, _maxQuantity;
        private long _clock;
        private bool _isFocused, _mouseButtonPressed, _thisPressed, _isPressed, _mouseInside;
        private Color _textColor, _shadowColor;
        public bool WithShadow { get; set; }
        public event Action OnClick;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="x">Позиция координаты Х</param>
        /// <param name="y">Позиция координаты Y</param>
        /// <param name="width">Ширина табло</param>
        /// <param name="heigt">Высота тобло</param>
        /// <param name="posTextV">Позиция координаты Y относительно верха табло для текста. Если установить в "0",
        /// позиция рассчитается автоматически</param>
        /// <param name="maxQuantity">Максимальное количество символов</param>
        public Display(int x, int y, int width, int height, int maxQuantity)
        {
            Rect = new Rectangle(x, y, width, height);
            _rectSouceBegin = ControlResources.AtlasRegions["GlassButtonBegin"];
            _rectSourceMiddle = ControlResources.AtlasRegions["GlassButtonMiddle"];
            _rectSourceEnd = ControlResources.AtlasRegions["GlassButtonEnd"];

            float scale = (float)height / (float)160;
            int widthBegin = (int)(80 * scale);
            int widthMiddle = (int)(Rect.Width - 160 * scale);
            int widthEnd = (int)(80 * scale);
            int posMiddle = Rect.X + widthBegin;
            int posEnd = posMiddle + widthMiddle;

            _rectDestBegin = new Rectangle(Rect.X, Rect.Y, widthBegin, (int)(160 * scale));
            _rectDestMiddle = new Rectangle(posMiddle, y, widthMiddle, (int)(160 * scale));
            _rectDestEnd = new Rectangle(posEnd, Rect.Y, widthEnd, (int)(160 * scale));

            float textOffset = Font.MeasureString("A").Y / 30f;
            _vectorDestShadow = new Vector2(textOffset, textOffset);
            _maxQuantity = maxQuantity > 100 ? 100 : maxQuantity;
            Text = "";
            _textColor = ControlResources.DefaultColor;
            _shadowColor = GetInverse(_textColor);
            Enabled = true;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if(value.Length > _maxQuantity) _text = value.Substring(0, _maxQuantity).Replace('\n', ' ').Replace('\r', ' ');
                else _text = value.Replace('\n', ' ').Replace('\r', ' ');
                UpdateAlign();
            }
        }

        void UpdateAlign()
        {
            _yText = (int)((Rect.Height - Font.MeasureString(_text + "R").Y) / 2);
            _xText = (int)((Rect.Width - Font.MeasureString(_text).X) / 2);
            _vectorCursor = new Vector2(Rect.X + _xText + (int)Font.MeasureString(_text).X + 2, Rect.Y + _yText);
            _vectorText = new Vector2(Rect.X + _xText, Rect.Y + _yText);
        }

        public bool AllowAnimation { get; set; }

        public bool Focused { get; set; }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (AllowAnimation && _isFocused)
            {
                if (_clock % 30 > 15)
                {
                    if (WithShadow) spriteBatch.DrawString(Font, "_", _vectorCursor + _vectorDestShadow, _shadowColor);
                    spriteBatch.DrawString(Font, "_", _vectorCursor, _textColor);
                }
            }
            spriteBatch.Draw(ControlResources.Atlas, _rectDestBegin, _rectSouceBegin, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestMiddle, _rectSourceMiddle, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestEnd, _rectSourceEnd, Color.White);
            if (WithShadow) spriteBatch.DrawString(Font, _text, _vectorText + _vectorDestShadow, _shadowColor);
            spriteBatch.DrawString(Font, _text, _vectorText, _textColor);
        }

        public override bool ClickInControl()
        {
            _clock++;
            _mouseInside = false;
            if (!Enabled) return false;
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (_mouseButtonPressed) return _thisPressed;
                if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                {
                    if (!_isPressed)
                    {
                        _thisPressed = true;
                        _isPressed = true;
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
                        if (_lastPressed > 0)
                        {
                            if(DateTime.Now.TimeOfDay.TotalMilliseconds > _lastPressed + 300)
                            {
                                _isFocused = true;
                                new Thread(new ThreadStart(Click)) { IsBackground = true }.Start();
                                _lastPressed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
                            }
                        }
                        else
                        {
                            _isFocused = true;
                            new Thread(new ThreadStart(Click)) { IsBackground = true }.Start();
                            _lastPressed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
                        }
                        _mouseInside = true;
                    }
                }
                else _isPressed = false;
            }
            return _mouseInside;
        }

        private void Click()
        {
            if (OnClick != null) OnClick.Invoke();
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            _vectorCursor.X += x;
            _vectorCursor.Y += y;
            Rect.Offset(x, y);
            _rectDestMiddle.Offset(x, y);
            _rectDestBegin.Offset(x,y);
            _rectDestEnd.Offset(x,y);
            _vectorText.X += x;
            _vectorText.Y += y;
        }

        public void Backspace()
        {
            if (_text.Length > 0) Text = _text.Substring(0, _text.Length - 1);
        }

        public Color ForeColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                _shadowColor = GetInverse(_textColor);
            }
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            UpdateAlign();
            float textOffset = Font.MeasureString("A").Y / 30f;
            _vectorDestShadow = new Vector2(textOffset, textOffset);
        }
    }
}