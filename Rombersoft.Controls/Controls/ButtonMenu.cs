using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rombersoft.Controls
{
    public class ButtonMenu : Control
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private int _posTextY1, _posTextY2;
        private Vector2 _posText1, _posText2, _vectorDestPuck, _vectorDestLine;
        private SpriteFont _spriteFont1;
        private SpriteFont _spriteFont2;
        private Color _mainColor, _disabledColor, _textColor;
        private string _text;
        private Rectangle _rect, _mainView, _focusedView, _disabledView, _rectSourceLine, _rectDestGlass, _rectDestArrowForSection, _rectSourcePuck, _rectDestIcon, _rectSourceArrow;
        private Rectangle? _rectSourceIcon, _glass;
        private Action OnClickButton;
        private bool _isPressed = false, _rightSide;
        private double _lastPressed;
        private bool _thisPressed = false, _mouseButtonPressed = false;
        private Align _align = Align.Center;
        private ButtonType _type;

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
        public ButtonMenu(int x, int y, string mainView, string focusedView, string disabled, string glass, string name, string icon, bool rightSide, ButtonType type, Action action)
        {
            _x = x;
            _y = y;
            _rightSide = rightSide;
            if (type == ButtonType.Numpad)
            {
                _width = ControlResources.AtlasRegions[glass].Width;
                _height = ControlResources.AtlasRegions[glass].Height;
            }
            else
            {
                if(!string.IsNullOrEmpty(mainView) && mainView != "Clear")
                {
                    _width = ControlResources.AtlasRegions[mainView].Width;
                    _height = ControlResources.AtlasRegions[mainView].Height;
                }
                else
                {
                    if(!string.IsNullOrEmpty(glass))
                    {
                        _width = ControlResources.AtlasRegions[glass].Width;
                        _height = ControlResources.AtlasRegions[glass].Height;
                    }
                    else if (!string.IsNullOrEmpty(focusedView))
                    {
                        _width = ControlResources.AtlasRegions[focusedView].Width;
                                                       _height = ControlResources.AtlasRegions[focusedView].Height;
                    }
                }
            }
            _focusedView = ControlResources.AtlasRegions[focusedView];
            _rectSourceIcon = ControlResources.AtlasRegions[icon];
            _disabledView = ControlResources.AtlasRegions[disabled];
            if(ControlResources.AtlasRegions.ContainsKey("ButtonSeparator")) _rectSourceLine = ControlResources.AtlasRegions["ButtonSeparator"];
            else _rectSourceLine = ControlResources.AtlasRegions["LineGradientForMenuButton"];
            if (!string.IsNullOrEmpty(glass)) _glass = ControlResources.AtlasRegions[glass];
            switch (type)
            {
                case ButtonType.Section:
                    _rectDestGlass = new Rectangle(_x, _y, _width, _height);
                    _vectorDestLine = new Vector2(_x + 59, _y);
                    _rectDestIcon = new Rectangle(_x + 30 - _rectSourceIcon.Value.Width / 2, _y + (80 - _rectSourceIcon.Value.Height) / 2, _rectSourceIcon.Value.Width,
                        _rectSourceIcon.Value.Height);
                    _rectSourceArrow = ControlResources.AtlasRegions["ArrowWhite"];
                    _rectDestArrowForSection = new Rectangle(_x + 300, _y + (80 - _rectSourceArrow.Height) / 2, _rectSourceArrow.Width, _rectSourceArrow.Height);
                    break;
                case ButtonType.Additional:
                    _rectDestIcon = new Rectangle(_x + 40 - _rectSourceIcon.Value.Width / 2, _y + (80 - _rectSourceIcon.Value.Height) / 2, _rectSourceIcon.Value.Width,
                        _rectSourceIcon.Value.Height);
                    _vectorDestPuck = new Vector2(_x + 2, _y + 2);
                    _rectSourcePuck = ControlResources.AtlasRegions["RoundBlue"];
                    break;
                case ButtonType.MenuWithArrow:
                case ButtonType.MenuWithIcon:
                    _rectDestGlass = new Rectangle(_x, _y, _width, _height);
                    if (!_rightSide)
                    {
                        _rectDestIcon = new Rectangle(_x + (60 - _rectSourceIcon.Value.Width) / 2, _y + (80 - _rectSourceIcon.Value.Height) / 2, _rectSourceIcon.Value.Width,
                            _rectSourceIcon.Value.Height);
                        _vectorDestLine = new Vector2(_x + 59, _y);
                    }
                    else
                    {
                        _rectDestIcon = new Rectangle(_x + _width - 60 + (60 - _rectSourceIcon.Value.Width) / 2, _y + (80 - _rectSourceIcon.Value.Height) / 2, 
                            _rectSourceIcon.Value.Width,
                            _rectSourceIcon.Value.Height);
                        _vectorDestLine = new Vector2(_x + _width - 59, _y);
                    }
                    break;
                case ButtonType.Simple:
                    _rectDestGlass = new Rectangle(_x, _y, _width, _height);
                    break;
                case ButtonType.Numpad:
                    _rectDestGlass = new Rectangle(_x, _y - 1, _width + 1, _height + 1);
                    break;
            }   
            _mainView = ControlResources.AtlasRegions[mainView];
            _spriteFont1 = ControlResources.Fonts["Roboto18"];
            _spriteFont2 = ControlResources.Fonts["Roboto11"];
            _rect = new Rectangle(_x, _y, _width, _height);
            _type = type;
            Name = name;
            OnClickButton = action;
            //Enable = true;
            _posTextY1 = 0;
            _posTextY2 = 0;
            _lastPressed = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
            _disabledColor = Color.DarkGray;
            _textColor = ControlResources.DefaultColor;
        }

        public void SetPuck(string puck)
        {
            _rectSourcePuck = ControlResources.AtlasRegions[puck];
        }

        public string Unit { get; set; }

        public bool AllowFocus { get; set; }

        public bool HoldTimeBeforeNext { get; set; }

        public bool WithShadow { get; set; }

        /// <summary>
        /// Смещает позицию текста по OY относительно верхнего левого угла кнопки
        /// </summary>
        /// <param nameRU="y">размер смещения в пикселях</param>
        public void SetPosText_Y1(int y)
        {
            _posText1.Y = y + _y;
            _posTextY1 = (int)_posText1.Y;
        }

        public void SetPosText_Y2(int y)
        {
            _posText2.Y = y + _y;
            _posTextY2 = (int)_posText2.Y;
        }

        public Rectangle? IconR
        {
            get { return _rectSourceIcon; }
            set
            {
                _rectSourceIcon = value;
                if (!_rectSourceIcon.HasValue) throw new Exception("Не возможно присвоить картинке нулевую ссылку");
                if(!_rightSide)
                    _rectDestIcon = new Rectangle(_x + (90 - _rectSourceIcon.Value.Width) / 2, _y + (80 - _rectSourceIcon.Value.Height) / 2, _rectSourceIcon.Value.Width, _rectSourceIcon.Value.Height);
                else _rectDestIcon = new Rectangle(_x + _width - 90 + (90 - _rectSourceIcon.Value.Width) / 2, _y + (80 - _rectSourceIcon.Value.Height) / 2, _rectSourceIcon.Value.Width,
                        _rectSourceIcon.Value.Height);
            }
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

        public void PropertyText(Align align, Color mainColor, Color disabledColor)
        {
            _mainColor = mainColor;
            _disabledColor = disabledColor;
            _align = align;
            SetPosText();
        }

        public void PropertyText(Align align, Color mainColor)
        {
            PropertyText(align, mainColor, mainColor);
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visibled) return;
            if (Enabled)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rect, _mainView, Color.White);
                if(_glass.HasValue)
                {
                    if (!_thisPressed)
                    {
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestGlass, _glass.Value, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestGlass, _glass.Value, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                    }
                }
                if (!String.IsNullOrEmpty(_text))
                {
                    if (WithShadow) spriteBatch.DrawString(_spriteFont1, _text, _posText1 + new Vector2(1,1), GetInverse(_textColor));
                    spriteBatch.DrawString(_spriteFont1, _text, _posText1, _textColor);
                }

                switch(_type)
                {
                    case ButtonType.MenuWithIcon:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine, _rectSourceLine, _textColor);
                        if (ControlResources.AtlasRegions.ContainsKey("ButtonSeparator"))
                            spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine - new Vector2(1, 0), _rectSourceLine, GetDarkness(_textColor));
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, Color.White);
                        break;
                    case ButtonType.MenuWithArrow:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine, _rectSourceLine, _textColor);
                        if (ControlResources.AtlasRegions.ContainsKey("ButtonSeparator"))
                            spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine - new Vector2(1,0), _rectSourceLine, GetDarkness(_textColor));
                        if (_rightSide)
                        {
                            spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, _textColor);
                        }
                        else
                        {
                            spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, _textColor, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        }
                        break;
                    case ButtonType.Additional:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestPuck, _rectSourcePuck, Color.White);
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, Color.White);
                        break;
                    case ButtonType.Section:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine, _rectSourceLine, _textColor);
                        if (ControlResources.AtlasRegions.ContainsKey("ButtonSeparator"))
                            spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine - new Vector2(1, 0), _rectSourceLine, GetDarkness(_textColor));
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon.Value, _textColor);
                        break;
                }
            }
            else
            {
                spriteBatch.Draw(ControlResources.Atlas, _rect, _disabledView, Color.White);
                switch (_type)
                {
                    case ButtonType.MenuWithIcon:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine, _rectSourceLine, _disabledColor);
                        if (ControlResources.AtlasRegions.ContainsKey("ButtonSeparator"))
                            spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine - new Vector2(1, 0), _rectSourceLine, GetDarkness(_disabledColor));
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, Color.White);
                        break;
                    case ButtonType.MenuWithArrow:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine, _rectSourceLine, _disabledColor);
                        if (ControlResources.AtlasRegions.ContainsKey("ButtonSeparator"))
                            spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine - new Vector2(1, 0), _rectSourceLine, GetDarkness(_disabledColor));
                        if (_rightSide)
                            spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, _disabledColor);
                        else
                            spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon,_disabledColor, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    case ButtonType.Additional:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestPuck, _rectSourcePuck, Color.White);
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, Color.White);
                        break;
                    case ButtonType.Section:
                        spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine, _rectSourceLine, _disabledColor);
                        if (ControlResources.AtlasRegions.ContainsKey("ButtonSeparator"))
                            spriteBatch.Draw(ControlResources.Atlas, _vectorDestLine - new Vector2(1, 0), _rectSourceLine, GetDarkness(_disabledColor));
                        spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, _disabledColor);
                        break;
                }
                if (!String.IsNullOrEmpty(_text))
                {
                    if (WithShadow) spriteBatch.DrawString(_spriteFont1, _text, _posText1 + new Vector2(1,1), GetInverse(_disabledColor));
                    spriteBatch.DrawString(_spriteFont1, _text, _posText1, _disabledColor);
                }
                if (_glass.HasValue)
                {
                    spriteBatch.Draw(ControlResources.Atlas, _rectDestGlass, _glass.Value, Color.White);
                }
            }
        }

        public override bool ClickInControl()
        {
            if (!Visibled) return false;
            if (!Enabled) return false;
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (_mouseButtonPressed) return _thisPressed;
                if (_rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                {
                    if (!_isPressed)
                    {
                        _thisPressed = true;
                        _isPressed = true;
                    }
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
                    if (_rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                    {
                        if (HoldTimeBeforeNext)
                        {
                            double newSubtract = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
                            if (Math.Abs(newSubtract - _lastPressed) > 300)
                            {
                                new Thread(Click) { IsBackground = true }.Start();
                                _lastPressed = newSubtract;
                            }
                        }
                        else
                        {
                            new Thread(new ThreadStart(Click)) { IsBackground = true }.Start();
                        }
                    }
                }
                else _isPressed = false;
            }
            return _thisPressed;
        }

        private void Click()
        {
            if (OnClickButton != null) OnClickButton.Invoke();
        }
        
        public override void Offset(int x, int y)
        {
            _x += x;
            _y += y;
            _rect.Offset(x, y);
            if (_rectSourceIcon.HasValue)
            {
                _rectDestIcon.Offset(x, y);
            }
            switch (_type)
            {
                case ButtonType.Section:
                    _rectDestArrowForSection.Offset(x, y);
                    break;
                case ButtonType.Additional:
                    _vectorDestPuck.X += x;
                    _vectorDestPuck.Y += y;
                    break;
            }
            _rectDestGlass.Offset(x, y);
            _vectorDestLine.X += x;
            _vectorDestLine.Y += y;
            _posText1.X += x;
            _posText1.Y += y;
            _posTextY1 = (int)_posText1.Y;
            _posText2.X += x;
            _posText2.Y += y;
            _posTextY2 = (int)_posText2.Y;
        }

        public override void SetPosition(int x, int y)
        {
            _posText1.X += x - _x;
            _posText1.Y += y - _y;
            _posTextY1 = (int)_posText1.Y;
            _posText2.X += x - _x;
            _posText2.Y += y - _y;
            _posTextY2 = (int)_posText2.Y;
            _rect.X = x;
            _rect.Y = y;
            if (_rectSourceIcon.HasValue)
            {
                _rectDestIcon.Offset(x - _x, y - _y);
            }
            switch(_type)
            {
                case ButtonType.Section:
                    _rectDestArrowForSection.Offset(x - _x, y - _y);
                    break;
                case ButtonType.Additional:
                    _vectorDestPuck.X += (x - _x);
                    _vectorDestPuck.Y += (y - _y);
                    break;
            }
            _vectorDestLine.X += (x - _x);
            _vectorDestLine.Y += (y - _y);
            _rectDestGlass.Offset(x - _x, y - _y);
            _x = x;
            _y = y;
        }

        private void SetPosText()
        {
            float length;
            switch (_align)
            {
                case Align.Left:
                    if (!_rightSide)
                    {
                        switch (_type)
                        {
                            case ButtonType.Additional:
                                _posText1 = new Vector2(_x + 90, _posTextY1);
                                break;
                            case ButtonType.Section:
                                _posText1 = new Vector2(_x + 75, _posTextY1);
                                break;
                            default:
                                _posText1 = new Vector2(_x + 80, _posTextY1);
                                break;
                        }
                    }
                    else
                    {
                        switch (_type)
                        {
                            case ButtonType.Additional:
                                _posText1 = new Vector2(_x + 90, _posTextY1);
                                break;
                            case ButtonType.Section:
                                _posText1 = new Vector2(_x + 75, _posTextY1);
                                break;
                            default:
                                _posText1 = new Vector2(_x + 80, _posTextY1);
                                break;
                        }
                    }
                    break;
                case Align.Right:
                    length = _spriteFont1.MeasureString(_text).Length();
                    if (!_rightSide) _posText1 = new Vector2(_width - length + _x - _width / 7, _posTextY1);
                    else _posText1 = new Vector2(_width - length + _x - _width / 7 - 80, _posTextY1);
                    break;
                case Align.Center:
                    length = _spriteFont1.MeasureString(_text).X;
                    if (_rightSide)
                    {
                        switch (_type)
                        {
                            case ButtonType.Additional:
                                _posText1 = new Vector2((_width - length) / 2 + _x - 80, _posTextY1);
                                break;
                            case ButtonType.Section:
                                _posText1 = new Vector2((_width - length) / 2 + _x - 80, _posTextY1);
                                break;
                            case ButtonType.MenuWithArrow:
                            case ButtonType.MenuWithIcon:
                                _posText1 = new Vector2((_width - 60 - length) / 2 + _x, _posTextY1);
                                break;
                            case ButtonType.Simple:
                            case ButtonType.Numpad:
                                _posText1 = new Vector2((_width - length) / 2 + _x, _posTextY1);
                                break;
                        }
                    }
                    else
                    {
                        switch (_type)
                        {
                            case ButtonType.Additional:
                                _posText1 = new Vector2((_width - length) / 2 + _x + 80, _posTextY1);
                                break;
                            case ButtonType.Section:
                                _posText1 = new Vector2((_width - length) / 2 + _x + 80, _posTextY1);
                                break;
                            case ButtonType.MenuWithArrow:
                            case ButtonType.MenuWithIcon:
                                _posText1 = new Vector2((_width - 60 - length) / 2 + _x + 60, _posTextY1);
                                break;
                            case ButtonType.Simple:
                            case ButtonType.Numpad:
                                _posText1 = new Vector2((_width - length) / 2 + _x, _posTextY1);
                                break;
                        }
                    }
                    break;
            }
        }

        private Color GetDarkness(Color color)
        {
            float k = 0.4f;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;
            return Color.FromNonPremultiplied((int)(r * k), (int)(g * k), (int)(b * k), a);
        }
    }

    public enum ButtonType
    {
        MenuWithArrow, MenuWithIcon, Section, Additional, Numpad, Simple
    }
}
