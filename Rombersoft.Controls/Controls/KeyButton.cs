using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Rombersoft.Controls
{
    public class KeyButton : Control
    {
        Vector2 _posText;
        public Color TextColor, BackColor, BorderColor;
        public int BorderWidth;
        string _text;
        static Texture2D _body;
        Rectangle _rectSourceImg, _rectDestImage, _rectInternal;
        public event Action OnClicked, OnPressed, OnReleased;
        bool _isPressed, _mouseInside, _mouseButtonPressed;
        double _lastPressed;

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
        public KeyButton(int x, int y, int width, int height)
        {
            if (_body == null) throw new InvalidOperationException("Texture for button is null. You have to call static method <CreateTexture> before");
            _text = string.Empty;
            Rect = new Rectangle(x, y, width, height);
            int borderWidth = width < height ? width / 30 : height / 30;
            _rectInternal = new Rectangle(x + borderWidth, y + borderWidth, width - 2 * borderWidth, height - 2 * borderWidth);
            _lastPressed = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
            _isPressed = _mouseInside = _mouseButtonPressed = false;
            TextColor = BorderColor = Color.Black;
            BackColor = Color.White;
        }

        public Rectangle Icon
        {
            get { return _rectSourceImg; }
            set
            {
                float allowedX = Rect.X + Rect.Width / 6;
                float allowedY = Rect.Y + Rect.Height / 6;
                float allowedWidth = Rect.Width / 1.5f;
                float allowedHeight = Rect.Height / 1.5f;
                float koefHeight = value.Height / allowedHeight;
                float koefWidth = value.Width / allowedWidth;
                int height, width;
                if (koefWidth > 1 || koefHeight > 1) //it need decrease size
                {
                    if (koefWidth < koefHeight)
                    {
                        height = (int)allowedHeight;
                        width = (int)(value.Width / koefHeight);
                    }
                    else
                    {
                        width = (int)allowedWidth;
                        height = (int)(value.Height / koefWidth);
                    }
                }
                else
                {
                    if (koefWidth < koefHeight)
                    {
                        height = (int)allowedHeight;
                        width = (int)(value.Width / koefHeight);
                    }
                    else
                    {
                        width = (int)allowedWidth;
                        height = (int)(value.Height / koefWidth);
                    }
                }
                _rectDestImage = new Rectangle(Rect.X + (Rect.Width - width) / 2, Rect.Y + (Rect.Height - height) / 2, width, height);
                _rectSourceImg = value; 
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
            _rectInternal.Offset(x, y);
            _rectDestImage.Offset(x, y);
            _posText.X += x;
            _posText.Y += y;
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            SetPosText();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isPressed)
            {
                GraphicInstance.SpriteBatch.Draw(_body, Rect, GetInverse(BorderColor));
                GraphicInstance.SpriteBatch.Draw(_body, _rectInternal, GetInverse(BackColor));
                GraphicInstance.SpriteBatch.DrawString(Font, _text, _posText, GetInverse(TextColor));
                GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestImage, _rectSourceImg, GetInverse(TextColor));
            }
            else
            {
                GraphicInstance.SpriteBatch.Draw(_body, Rect, BorderColor);
                GraphicInstance.SpriteBatch.Draw(_body, _rectInternal, BackColor);
                GraphicInstance.SpriteBatch.DrawString(Font, _text, _posText, TextColor);
                GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestImage, _rectSourceImg, TextColor);
            }
        }

        public override bool ClickInControl()
        {
            _mouseInside = false;
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (_mouseButtonPressed) return true;
                if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                {
                    if (!_isPressed)
                    {
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
                if (_isPressed)
                {
                    _isPressed = false;
                    if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                    {
                        double newSubtract = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
                        if (Math.Abs(newSubtract - _lastPressed) > 100)
                        {
                            if (OnReleased != null) OnReleased();
                            if (OnClicked != null) OnClicked.Invoke();
                            _lastPressed = newSubtract;
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
            _posText = new Vector2((Rect.Width - pos.X) / 2 + Rect.X, (Rect.Height - pos.Y) / 2 + Rect.Y);
        }

        /// <summary>
        /// Creates the texture. Attention!!! This method have to executed at main Thread
        /// </summary>
        /// <param name="border">Border color.</param>
        /// <param name="body">Body color.</param>
        /// <param name="borderWidth">Border width.</param>
        public static void CreateTexture(int width, int height)
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicInstance.Device);
            RenderTarget2D map = new RenderTarget2D(GraphicInstance.Device, width, height);
            GraphicInstance.Device.SetRenderTarget(map);
            GraphicInstance.Device.Clear(Color.Transparent);
            spriteBatch.Begin();
            Rectangle[] rectSources = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            Rectangle[] rectDests = SpliteRect(new Rectangle(0, 0, width, height),12);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[0], rectSources[0], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[1], rectSources[1], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[2], rectSources[2], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[3], rectSources[3], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[4], rectSources[4], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[5], rectSources[5], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[6], rectSources[6], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[7], rectSources[7], Color.White);
            spriteBatch.Draw(ControlResources.Atlas, rectDests[8], rectSources[8], Color.White);
            spriteBatch.End();
            GraphicInstance.Device.SetRenderTarget(null);
            _body = new Texture2D(GraphicInstance.Device, map.Bounds.Width, map.Bounds.Height);
            Color[] color = new Color[map.Bounds.Width * map.Bounds.Height];
            map.GetData<Color>(color);
            _body.SetData<Color>(color);
        }
    }
}
