using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

namespace Rombersoft.Controls
{
    public class KeyBoard : Control
    {
        public enum TypeInput { eng, rus, eng_UpCase }
        private ButtonForKeyboardBig[] _buttons;
        private int _x, _y;
        public event Action<string, string> OnClick;
        private bool _upper = false, _rus = false, _withBorder, _enabled;
        private TypeInput _typeInput;
        private Plate _plate;

        public TypeInput LanguageInput
        {
            get { return _typeInput; }
            set
            {
                _typeInput = value;
                switch (value)
                {
                    case TypeInput.eng:
                        _rus = false;
                        _buttons[53].Enabled = true;
                        _buttons[22].Enabled = true;
                        _buttons[34].Enabled = true;
                        _buttons[44].Enabled = true;
                        _buttons[45].Enabled = true;
                        break;
                    case TypeInput.eng_UpCase:
                        _rus = false;
                        _buttons[53].Enabled = false;
                        _buttons[45].Enabled = false;
                        _buttons[22].Enabled = false;
                        _buttons[34].Enabled = false;
                        _buttons[44].Enabled = false;
                        break;
                    case TypeInput.rus:
                        _rus = true;
                        _buttons[53].Enabled = true;
                        _buttons[22].Enabled = true;
                        _buttons[34].Enabled = true;
                        _buttons[44].Enabled = true;
                        _buttons[45].Enabled = true;
                        break;
                }
                ChangeLanguage();
                if (value == TypeInput.eng_UpCase)
                {
                    CapsLock();
                    _upper = false;
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="x">Координата контрола по X</param>
        /// <param name="y">Координата контрола по Y</param>
        public KeyBoard(int x, int y, bool withBorder, bool blackText)
        {
            Color textColor;
            if (blackText)
                textColor = Color.Black;
            else
                textColor = Color.SteelBlue;
            _withBorder = withBorder;
            _x = x;
            _y = y;
            if (_withBorder)
            {
                _plate = new Plate(0, _y - 15, 1280, 373, true, 1);
            }
            _buttons = new ButtonForKeyboardBig[54];
            for (byte i = 0; i < 10; i++)
            {
                int j = i;
                _buttons[i] = new ButtonForKeyboardBig(126 * i + _x, _y, 115, "", new Action(() => ButtonClick(j)));
                _buttons[i].PropertyText(Align.Center, Font, textColor);
                _buttons[i].Unit = "Keyboard";
                _buttons[i].Text = ((i + 1) % 10).ToString();
                _buttons[i].SetPosText_Y1(10);
            }

            for (byte n = 0, i = 10; i < 21; i++, n++)
            {
                int j = i;
                _buttons[i] = new ButtonForKeyboardBig(103 * n + _x, 70 + _y, 91, "", new Action(() => ButtonClick(j)));
                _buttons[i].PropertyText(Align.Center, Font, textColor);
                _buttons[i].Unit = "Keyboard";
                _buttons[i].SetPosText_Y1(10);
            }

            _buttons[21] = new ButtonForKeyboardBig(103 * 11 + _x, 70 + _y, 115, "", new Action(() => ButtonClick(21)));
            _buttons[21].ImageR = ControlResources.AtlasRegions["Backspace"];
            _buttons[21].PropertyText(Align.Center, ControlResources.DefaultFont, textColor);
            _buttons[22] = new ButtonForKeyboardBig(_x, 140 + _y, 115, "", new Action(() => ButtonClick(22)));
            _buttons[22].Text = "Caps Lock";
            _buttons[22].SetPosText_Y1(15);
            _buttons[34] = new ButtonForKeyboardBig(_x, 210 + _y, 154, "", new Action(() => ButtonClick(34)));
            _buttons[44] = new ButtonForKeyboardBig(1094 + _x, 210 + _y, 154, "", new Action(() => ButtonClick(34)));
            _buttons[45] = new ButtonForKeyboardBig(_x, 280 + _y, 154, "", new Action(() => ButtonClick(45)));
            _buttons[22].PropertyText(Align.Center, ControlResources.DefaultFont, textColor);
            _buttons[22].Unit = "Keyboard";
            _buttons[22].AllowFocus = true;

            for (byte n = 0, i = 23; i < 34; i++, n++)
            {
                int j = i;
                _buttons[i] = new ButtonForKeyboardBig(103 * n + 127 + _x, 140 + _y, 91, "", new Action(() => ButtonClick(j)));
                _buttons[i].PropertyText(Align.Center, Font, textColor);
                _buttons[i].Unit = "Keyboard";
                _buttons[i].SetPosText_Y1(10);
            }

            _buttons[34].ImageR = ControlResources.AtlasRegions["IconShift"];
            _buttons[34].PropertyText(Align.Center, ControlResources.DefaultFont, textColor);
            _buttons[34].AllowFocus = true;

            for (byte n = 0, i = 35; i < 44; i++, n++)
            {
                int j = i;
                _buttons[i] = new ButtonForKeyboardBig(103 * n + 166 + _x, 210 + _y, 91, "", new Action(() => ButtonClick(j)));
                _buttons[i].PropertyText(Align.Center, Font, textColor);
                _buttons[i].Unit = "Keyboard";
                _buttons[i].SetPosText_Y1(10);
            }

            _buttons[44].ImageR = ControlResources.AtlasRegions["IconShift"];
            _buttons[44].PropertyText(Align.Center, ControlResources.DefaultFont, textColor);
            _buttons[44].AllowFocus = true;

            _buttons[45].PropertyText(Align.Center, Font, textColor);
            _buttons[45].Text = "?123";
            _buttons[45].AllowFocus = true;
            _buttons[45].SetPosText_Y1(10);

            _buttons[46] = new ButtonForKeyboardBig(166 + _x, 280 + _y, 91, "", new Action(() => ButtonClick(46)));
            _buttons[46].PropertyText(Align.Center, Font, textColor);
            _buttons[46].Text = "@";
            _buttons[46].SetPosText_Y1(10);

            _buttons[47] = new ButtonForKeyboardBig(269 + _x, 280 + _y, 91, "", new Action(() => ButtonClick(47)));
            _buttons[47].PropertyText(Align.Center, Font, textColor);
            _buttons[47].Text = "-";
            _buttons[47].SetPosText_Y1(10);

            _buttons[48] = new ButtonForKeyboardBig(372 + _x, 280 + _y, 297, "", new Action(() => ButtonClick(48)));
            _buttons[48].SetPosText_Y1(10);

            _buttons[49] = new ButtonForKeyboardBig(681 + _x, 280 + _y, 91, "", new Action(() => ButtonClick(49)));
            _buttons[49].PropertyText(Align.Center, Font, textColor);
            _buttons[49].SetPosText_Y1(10);

            _buttons[50] = new ButtonForKeyboardBig(784 + _x, 280 + _y, 91, "", new Action(() => ButtonClick(50)));
            _buttons[50].PropertyText(Align.Center, Font, textColor);
            _buttons[50].SetPosText_Y1(10);

            _buttons[51] = new ButtonForKeyboardBig(887 + _x, 280 + _y, 91, "", new Action(() => ButtonClick(51)));
            _buttons[51].PropertyText(Align.Center, Font, textColor);
            _buttons[51].Text = "[";
            _buttons[51].SetPosText_Y1(10);

            _buttons[52] = new ButtonForKeyboardBig(990 + _x, 280 + _y, 91, "", new Action(() => ButtonClick(52)));
            _buttons[52].PropertyText(Align.Center, Font, textColor);
            _buttons[52].SetPosText_Y1(10);

            _buttons[53] = new ButtonForKeyboardBig(1094 + _x, 280 + _y, 154, "", new Action(() => ButtonClick(53)));
            _buttons[53].PropertyText(Align.Center, ControlResources.DefaultFont, textColor);
            _buttons[53].SetPosText_Y1(15);

            ChangeLanguage();
        }

        public void EnableKeys()
        {
            if (!_enabled)
            {
                for (int i = 0; i < _buttons.Length; i++)
                {
                    _buttons[i].Enabled = true;
                }
                _enabled = true;
            }
        }

        public void EnableKeys(string[] symbols)
        {
            _enabled = false;
            for (int i = 0; i < _buttons.Length; i++)
            {
                if(!symbols.Contains(_buttons[i].Text)) _buttons[i].Enabled = false;
                else _buttons[i].Enabled = true;
            }
            _buttons[21].Enabled = true;
        }

        public void Reset()
        {
            _buttons[45].InFocus = false;
            _buttons[45].Text = "?123";
            _buttons[22].Enabled = true;
            _buttons[34].Enabled = true;
            _buttons[44].Enabled = true;
            _buttons[53].Enabled = true;
            ChangeLanguage();
            if (_upper == true)
            {
                _upper = false;
                CapsLock();
            }
        }

        private void ButtonClick(int key)
        {
            string symbol = "";
            switch (key)
            {
                case 53:
                    if (_rus) _rus = false;
                    else _rus = true;
                    ChangeLanguage();
                    if (_upper == true)
                    {
                        _upper = false;
                        CapsLock();
                    }
                    if (OnClick != null)
                    {
                        OnClick(_typeInput.ToString(), "redraw");
                    }
                    return;
                case 22:
                    CapsLock();
                    if (OnClick != null)
                    {
                        OnClick(_typeInput.ToString(), "redraw");
                    }
                    return;
                case 34:
                case 44:
                    CapsLock();
                    if (OnClick != null)
                    {
                        OnClick(_typeInput.ToString(), "redraw");
                    }
                    return;
                case 45:
                    if (_buttons[45].Text == "abc")
                    {
                        _buttons[45].InFocus = false;
                        _buttons[45].Text = "?123";
                        _buttons[22].Enabled = true;
                        _buttons[34].Enabled = true;
                        _buttons[44].Enabled = true;
                        _buttons[53].Enabled = true;
                        ChangeLanguage();
                        if (_upper == true)
                        {
                            _upper = false;
                            CapsLock();
                        }
                    }
                    else
                    {
                        _buttons[45].Text = "abc";
                        ShowSymbols();
                    }
                    if (OnClick != null)
                    {
                        OnClick(_typeInput.ToString(), "redraw");
                    }
                    return;
                case 48:
                    symbol = " ";
                    break;
                case 21:
                    symbol = "del";
                    break;
                default:
                    symbol = _buttons[key].Text;
                    break;
            }
            if (OnClick != null)
            {
                OnClick(_typeInput.ToString(), symbol);
            }
            if ((_buttons[34].InFocus == true) || (_buttons[44].InFocus == true)) CapsLock();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_withBorder) _plate.Draw(spriteBatch);
            for (byte i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].Draw(spriteBatch);
            }
        }

        public override bool ClickInControl()
        {
            foreach (ButtonForKeyboardBig button in _buttons)
            {
                button.ClickInControl();
            }
            return false;
        }

        public override void SetPosition(int x, int y)
        {
            int stepX = x - _x;
            int stepY = y - _y;
            _x = x;
            _y = y;
            foreach (ButtonForKeyboardBig button in _buttons)
            {
                button.Offset(stepX, stepY);
            }
            if (_withBorder) _plate.Offset(stepX, stepY);
        }

        public override void Offset(int x, int y)
        {
            _x += x;
            _y += y;
            if (_withBorder) _plate.Offset(x, y);
            foreach (ButtonForKeyboardBig button in _buttons)
            {
                button.Offset(x, y);
            }
        }

        private void ChangeLanguage()
        {
            if (!_rus)
            {
                _buttons[53].Text = "Рус/Укр";
                _buttons[10].Text = "Tab";
                _buttons[10].Enabled = false;
                _buttons[11].Enabled = true;
                _buttons[11].Text = "q";
                _buttons[12].Text = "w";
                _buttons[13].Text = "e";
                _buttons[14].Text = "r";
                _buttons[15].Text = "t";
                _buttons[16].Text = "y";
                _buttons[17].Text = "u";
                _buttons[18].Text = "i";
                _buttons[19].Text = "o";
                _buttons[20].Text = "p";
                _buttons[23].Text = "a";
                _buttons[24].Text = "s";
                _buttons[25].Text = "d";
                _buttons[26].Text = "f";
                _buttons[27].Text = "g";
                _buttons[28].Text = "h";
                _buttons[29].Text = "j";
                _buttons[30].Text = "k";
                _buttons[30].Enabled = true;
                _buttons[31].Text = "l";
                _buttons[32].Text = ";";
                _buttons[33].Text = "`";
                _buttons[33].Name = "1-я кавычка";
                _buttons[35].Text = "z";
                _buttons[36].Text = "x";
                _buttons[37].Text = "c";
                _buttons[38].Text = "v";
                _buttons[39].Text = "b";
                _buttons[40].Text = "n";
                _buttons[41].Text = "m";
                _buttons[41].Enabled = true;
                _buttons[42].Text = ",";
                _buttons[42].Enabled = true;
                _buttons[43].Text = ".";
                _buttons[43].Enabled = true;
                _buttons[47].Text = "-";
                _buttons[49].Text = ":";
                _buttons[50].Text = "_";
                _buttons[51].Text = "[";
                _buttons[52].Text = "]";
            }
            else
            {
                _buttons[53].Text = "English";
                _buttons[50].Text = "ы";
                _buttons[24].Text = "і";
                _buttons[49].Text = "э";
                _buttons[33].Text = "є";
                _buttons[10].Text = "й";
                _buttons[10].Enabled = true;
                _buttons[11].Enabled = true;
                _buttons[11].Text = "ц";
                _buttons[12].Text = "у";
                _buttons[13].Text = "к";
                _buttons[14].Text = "е";
                _buttons[15].Text = "н";
                _buttons[16].Text = "г";
                _buttons[17].Text = "ш";
                _buttons[18].Text = "щ";
                _buttons[19].Text = "з";
                _buttons[20].Text = "х";
                _buttons[23].Text = "ф";
                _buttons[25].Text = "в";
                _buttons[26].Text = "а";
                _buttons[27].Text = "п";
                _buttons[28].Text = "р";
                _buttons[29].Text = "о";
                _buttons[30].Text = "л";
                _buttons[30].Enabled = true;
                _buttons[31].Text = "д";
                _buttons[32].Text = "ж";
                _buttons[35].Text = "я";
                _buttons[36].Text = "ч";
                _buttons[37].Text = "с";
                _buttons[38].Text = "м";
                _buttons[39].Text = "и";
                _buttons[40].Text = "т";
                _buttons[41].Text = "ь";
                _buttons[41].Enabled = true;
                _buttons[42].Text = "б";
                _buttons[42].Enabled = true;
                _buttons[43].Text = "ю";
                _buttons[43].Enabled = true;
                _buttons[47].Text = ".";
                _buttons[51].Text = "ї";
                _buttons[52].Text = "ъ";
            }
        }

        private void CapsLock()
        {
            int i;
            if (_rus) i = 10;
            else i = 11;
            if (!_upper)
            {
                for (; i < 21; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToUpper();
                }
                for (i = 23; i < 34; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToUpper();
                }
                for (i = 35; i < 44; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToUpper();
                }
                for (i = 49; i < 53; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToUpper();
                }
                _buttons[52].Text = _buttons[52].Text.ToUpper();
                _upper = true;
            }
            else
            {
                for (; i < 21; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToLower();
                }
                for (i = 23; i < 34; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToLower();
                }
                for (i = 35; i < 44; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToLower();
                }
                for (i = 49; i < 53; i++)
                {
                    _buttons[i].Text = _buttons[i].Text.ToLower();
                }
                _buttons[52].Text = _buttons[52].Text.ToLower();
                _upper = false;
                _buttons[22].InFocus = false;
                _buttons[34].InFocus = false;
                _buttons[44].InFocus = false;
            }
        }

        private void ShowSymbols()
        {
            _buttons[10].Text = "Tab";
            _buttons[10].Enabled = false;
            _buttons[11].Enabled = false;
            _buttons[22].Enabled = false;
            _buttons[34].Enabled = false;
            _buttons[41].Enabled = false;
            _buttons[42].Enabled = false;
            _buttons[43].Enabled = false;
            _buttons[44].Enabled = false;
            _buttons[53].Enabled = false;
            _buttons[11].Text = "";
            _buttons[12].Text = "!";
            _buttons[13].Text = "~";
            _buttons[14].Text = "#";
            _buttons[15].Text = "№";
            _buttons[16].Text = "$";
            _buttons[17].Text = "%";
            _buttons[18].Text = "^";
            _buttons[19].Text = "?";
            _buttons[20].Text = "&";
            _buttons[23].Text = "*";
            _buttons[24].Text = "(";
            _buttons[25].Text = ")";
            _buttons[26].Text = ".";
            _buttons[27].Text = "/";
            _buttons[28].Text = "+";
            _buttons[29].Text = "=";
            _buttons[30].Text = "";
            _buttons[30].Enabled = false;
            _buttons[31].Text = "|";
            _buttons[32].Text = ";";
            _buttons[33].Text = "`";
            _buttons[35].Text = "-";
            _buttons[36].Text = "{";
            _buttons[37].Text = "}";
            _buttons[38].Text = "<";
            _buttons[39].Text = ">";
            _buttons[40].Text = ",";
            _buttons[41].Text = "";
            _buttons[42].Text = "";
            _buttons[43].Text = "";
            _buttons[49].Text = ":";
            _buttons[50].Text = "_";
            _buttons[51].Text = "[";
            _buttons[52].Text = "]";
        }
    }

    public class ButtonForKeyboardBig : Control
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private int _posTextY1;
        private Vector2 _posText, _vectorDestBegin, _vectorDestEnd, _vectorScale;
        private SpriteFont _spriteFont1;
        private string _text = "";
        private Color _color;
        private Rectangle _rect, _rectImg, _rectSourceMainViewBegin, _rectSourceMainViewMiddle, _rectSourceMainViewEnd, _rectSourceFocusedBegin, 
        _rectSourceFocusedMiddle, _rectSourceFocusedEnd, _rectDestMiddle;
        private Rectangle? _imageR;
        private Action OnClickButton;
        private bool _isPressed = false;
        private bool _thisPressed = false,
        _mouseButtonPressed = false;
        private Align _align = Align.Center;
        private double _lastPressed;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="x">координата Х</param>
        /// <param name="y">координата Y</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        /// <param name="mainView">рисунок в режиме ожидания</param>
        /// <param name="isInFocus">рисунок в фокусе</param>
        /// <param name="pressed">рисунок нажатой кнопки</param>
        /// <param name="disabled">рисунок недоступной кнопки</param>
        /// <param name="name">название кнопки</param>
        /// <param name="action">действие при нажатии</param>
        public ButtonForKeyboardBig(int x, int y, int width, string name, Action action)
        {
            _color = Color.SteelBlue;
            _x = x;
            _y = y;
            _width = width == 0 ? 55 : width;
            _height = 62;// 138,1897,55,65//138,1829,55,65
            _rectSourceMainViewBegin = new Rectangle(138,1899,20,62);
            _rectSourceMainViewMiddle = new Rectangle(158, 1899, 15, 62);
            _rectSourceMainViewEnd = new Rectangle(173,1899,20,62);
            _rectSourceFocusedBegin = new Rectangle(138, 1831, 20, 62);
            _rectSourceFocusedMiddle = new Rectangle(158, 1831, 15, 62);
            _rectSourceFocusedEnd = new Rectangle(173, 1831, 20, 62);
            _vectorDestBegin = new Vector2(_x,_y);
            _rectDestMiddle = new Rectangle(_x+20, _y, _width - 40, 62);
            _vectorDestEnd = new Vector2(_x + width - 20, _y);
            _vectorScale = new Vector2(1, 1);
            _spriteFont1 = ControlResources.DefaultFont;
            _rect = new Rectangle(_x, _y, _width, _height);
            Name = name;
            OnClickButton = action;
            Enabled = true;
            FocusedText = Color.White;
            _lastPressed = -1;
            _posTextY1 = 0;
            _lastPressed = DateTime.Now.Subtract(new DateTime(2010, 1, 1)).TotalMilliseconds;
        }

        public bool InFocus { get; set; }

        public string Unit { get; set; }

        public bool AllowFocus { get; set; }

        public bool HoldTimeBeforeNext { get; set; }

        public Color FocusedText { get; set; }
        /// <summary>
        /// Смещает позицию текста по OY относительно верхнего левого угла кнопки
        /// </summary>
        /// <param name="y">размер смещения в пикселях</param>
        public void SetPosText_Y1(int y)
        {
            _posText.Y = y + _y;
            _posTextY1 = (int)_posText.Y;
        }

        public Rectangle ? ImageR
        {
            get { return _imageR; }
            set
            {
                _imageR = value;
                if (!_imageR.HasValue) throw new Exception("Не возможно присвоить картинке нулевую ссылку");
                _rectImg = new Rectangle(_x + (_width - _imageR.Value.Width) / 2, _y + (_height - _imageR.Value.Height) / 2, _imageR.Value.Width, _imageR.Value.Height);
            }
        }

        public string Text
        {
            set
            {
                _text = value;
                SetPosText();
            }
            get { return _text; }
        }

        public override void Offset(int x, int y)
        {
            _x += x;
            _y += y;
            _rect.Offset(x, y);
            if(_imageR.HasValue)
            {
                _rectImg.Offset(x, y);
            }
            _posText.X += x;
            _posText.Y += y;
            _posTextY1 = (int)_posText.Y;
            _rectDestMiddle.Offset(x, y);
            _vectorDestBegin.X += x;
            _vectorDestBegin.Y += y;
            _vectorDestEnd.X += x;
            _vectorDestEnd.Y += y;
        }

        public void PropertyText(Align align, SpriteFont font, Color color)
        {
            _color = color;
            _spriteFont1 = font;
            _align = align;
            SetPosText();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if((_thisPressed)||(!Enabled))
            {
                spriteBatch.Draw(ControlResources.Atlas, _vectorDestBegin, _rectSourceMainViewBegin, Color.White, 0f, Vector2.Zero,
                    _vectorScale, SpriteEffects.FlipVertically, 0f);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestMiddle, _rectSourceMainViewMiddle, Color.White, 0f, Vector2.Zero, 
                    SpriteEffects.FlipVertically, 0f);
                spriteBatch.Draw(ControlResources.Atlas, _vectorDestEnd, _rectSourceMainViewEnd, Color.White, 0f, Vector2.Zero,
                    _vectorScale, SpriteEffects.FlipVertically, 0f);
                if (!String.IsNullOrEmpty(_text))
                {
                    spriteBatch.DrawString(_spriteFont1, _text, _posText, Color.RosyBrown);
                }
                if (ImageR.HasValue) spriteBatch.Draw(ControlResources.Atlas, _rectImg, _imageR, Color.White);
            }
            else
            {
                if(InFocus)
                {
                    spriteBatch.Draw(ControlResources.Atlas, _vectorDestBegin, _rectSourceFocusedBegin, Color.White);
                    spriteBatch.Draw(ControlResources.Atlas, _rectDestMiddle, _rectSourceFocusedMiddle, Color.White);
                    spriteBatch.Draw(ControlResources.Atlas, _vectorDestEnd, _rectSourceFocusedEnd, Color.White);
                    if (!String.IsNullOrEmpty(_text))
                    {
                        spriteBatch.DrawString(_spriteFont1, _text, _posText, Color.White);
                    }
                }
                else
                {
                    spriteBatch.Draw(ControlResources.Atlas, _vectorDestBegin, _rectSourceMainViewBegin, Color.White);
                    spriteBatch.Draw(ControlResources.Atlas, _rectDestMiddle, _rectSourceMainViewMiddle, Color.White);
                    spriteBatch.Draw(ControlResources.Atlas, _vectorDestEnd, _rectSourceMainViewEnd, Color.White);
                    if (!String.IsNullOrEmpty(_text))
                    {
                        spriteBatch.DrawString(_spriteFont1, _text, _posText, _color);
                    }
                }

                if (ImageR.HasValue)
                {
                    if (_color == Color.Black)
                        spriteBatch.Draw(ControlResources.Atlas, _rectImg, _imageR, Color.FromNonPremultiplied(0, 0, 0, 255));
                    else
                        spriteBatch.Draw(ControlResources.Atlas, _rectImg, _imageR, Color.White);
                }
            }
        }

        public override bool ClickInControl()
        {
            bool flag = false;
            if (!Enabled) return false;
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (_mouseButtonPressed) return flag;
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
                        if (AllowFocus) InFocus = true;
                        flag = true;
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
            return flag;
        }

        private void Click()
        {
            if (OnClickButton != null) OnClickButton.Invoke();
        }

        public override void SetPosition(int x, int y)
        {
            _posText.X += x - _x;
            _posText.Y += y - _y;
            _posTextY1 = (int)_posText.Y;
            _rect.X = x;
            _rect.Y = y;
            _rectImg.Offset(x - _x, y - _y);
            _rectDestMiddle.Offset(x - _x, y - _y);
            _vectorDestBegin.X = x;
            _vectorDestBegin.Y = y;
            _vectorDestEnd.X = x;
            _vectorDestEnd.Y = y;
            _x = x;
            _y = y;
        }

        private void SetPosText()
        {
            float length;
            switch (_align)
            {
                case Align.Left:
                    _posText = new Vector2(_width / 7 + _x, _posTextY1);
                    break;
                case Align.Right:
                    length = _spriteFont1.MeasureString(_text).Length();
                    _posText = new Vector2(_width - length + _x - _width / 7, _posTextY1);
                    break;
                case Align.Center:
                    length = _spriteFont1.MeasureString(_text).X;
                    _posText = new Vector2((_width - length) / 2 + _x, _posTextY1);
                    break;
            }
        }
    }
}