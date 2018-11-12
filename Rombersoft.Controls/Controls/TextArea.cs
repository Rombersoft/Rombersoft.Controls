using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rombersoft.Controls
{
    public class TextArea : Control
    {
        private Rectangle _rectSourceTop, _rectSourceMiddle, _rectSourceBottom, _rectDestMiddle, _rectDestTop, _rectDestBottom;
        private Vector2 _vectorTopRight, _vectorLeftTop, _vectorRightBottom, _vectorBottomLeft, _vectorScale, _vectorCursor;
        private long _clock;
        private int _lastPressed, _order, _widthText, _currentLine;
        private string _text;
        private bool _isFocused, _mouseButtonPressed, _thisPressed, _isPressed;
        private Action Click;
        private SpriteFont _font;
        private StringBuilder _stringBuilder;
        private EventWaitHandle wh1;

        public TextArea(int x, int y, int width, int height, Action click)
        {
            wh1 = new AutoResetEvent(true);
            _widthText = width - 30;
            Rect = new Rectangle(x, y, width, height);
            _rectSourceTop = new Rectangle(1115, 196, 110, 20);
            _rectSourceMiddle = new Rectangle(1116, 216, 110, 34);
            _rectSourceBottom = new Rectangle(1115, 250, 110, 20);
            _rectDestMiddle = new Rectangle(x, y+20, width, height - 40);
            _rectDestBottom = new Rectangle(x + 110, y+height-20, width-220, 20);
            _rectDestTop = new Rectangle(x + 110, y, width - 220, 20);

            _vectorScale = new Vector2(1, 1);
            _vectorLeftTop = new Vector2(x, y);
            _vectorTopRight = new Vector2(x + width - 110, y);
            _vectorBottomLeft = new Vector2(x, y + height - 20);
            _vectorRightBottom = new Vector2(x + width - 110, y + height - 20);
            _vectorCursor = new Vector2(15, 15) + _vectorLeftTop;
            Click = click;
            _font = ControlResources.Fonts["UbuntuBold16"];
            _stringBuilder = new StringBuilder(1000);
            _text = "";
            Enabled = true;
        }

        public bool Focused 
        {
            get { return _isFocused; }
            set { _isFocused = value; }
        }

        public void AddSymbol(string symbol)
        {
            wh1.WaitOne();
            _stringBuilder.Insert(_order, symbol);
            _order++;
            string text = _stringBuilder.ToString();
            StringBuilder builder = new StringBuilder(300);
            for(short i=0; i<text.Length; i++)
            {
                if (_font.MeasureString(builder.ToString() + text[i]).X > _widthText)
                {
                    builder.Append('\n');
                    builder.Append(text[i]);
                }
                else builder.Append(text[i]);
            }
            _text = builder.ToString();
            _vectorCursor.X += _font.MeasureString(symbol).X;
            if (_vectorCursor.X - _vectorLeftTop.X - 15f > _widthText)
            {
                _vectorCursor.X = _vectorLeftTop.X + 13f + _font.MeasureString(symbol).X;
                _vectorCursor.Y += _font.LineSpacing;
                _currentLine++;
            }
            wh1.Set();
        }

        public void BackSpace()
        {
            if (_order > 0)
            {
                wh1.WaitOne();
                char[] symbols = new char[1];
                _stringBuilder.CopyTo(_order-1, symbols, 0, 1);
                _stringBuilder.Remove(_order-1, 1);
                _order--;
                string text = _stringBuilder.ToString();
                StringBuilder builder = new StringBuilder(300);
                for(short i=0; i<text.Length; i++)
                {
                    if (_font.MeasureString(builder.ToString() + text[i]).X > _widthText)
                    {
                        builder.Append('\n');
                        builder.Append(text[i]);
                    }
                    else builder.Append(text[i]);
                }
                _text = builder.ToString();
                _vectorCursor.X -= _font.MeasureString(symbols[0].ToString()).X;
                if((!String.IsNullOrEmpty(_text))&&(_vectorCursor.X - _vectorLeftTop.X - 15f < 0))
                {
                    if(--_currentLine>=0)_vectorCursor.X = _vectorLeftTop.X + 13f + _font.MeasureString(_text.Split('\n')[_currentLine]).X;
                    _vectorCursor.Y -= _font.LineSpacing;
                }
                wh1.Set();
            }
        }

        public string GetOnlyText()
        {
            return _stringBuilder.ToString();
        }

        public void Reset()
        {
            _order = 0;
            _stringBuilder.Clear();
            _text = "";
            _vectorCursor = new Vector2(15, 15) + _vectorLeftTop;
        }

        public SpriteFont FontP
        {
            get { return _font; }
            set 
            { 
                _font = value; 
            }
        }

        private void SetCursor(int x, int y)
        {
            if (String.IsNullOrEmpty(_text)) return;
            string[] lines = _text.Split('\n');
            y -= (int)(_vectorLeftTop.Y + 15f);
            int nextX = x;
            if (y < 0) 
                _vectorCursor = new Vector2(15, 15) + _vectorLeftTop;
            else
            {
                if(y>lines.Length*_font.LineSpacing)
                {
                    _vectorCursor = new Vector2(15, 15) + _vectorLeftTop;
                    _vectorCursor.Y += (lines.Length - 1) * _font.LineSpacing;
                    _vectorCursor.X += _font.MeasureString(lines.Last()).X;
                }
                else
                {
                    for (byte i = 0; i < lines.Length; i++)
                    {
                        if (y >= i * _font.LineSpacing && y <= (i + 1) * _font.LineSpacing)
                        {
                            nextX = x - (int)(_vectorLeftTop.X + 15f);
                            float distance = _widthText + 10;
                            y = i * _font.LineSpacing;
                            Vector2 point = new Vector2(nextX, y);
                            StringBuilder builder = new StringBuilder(100);
                            for (byte j = 0; j < lines[i].Length; j++)
                            {
                                builder.Append(lines[i][j]);
                                float newDestance = Vector2.Distance(point, new Vector2(_font.MeasureString(builder.ToString()).X, y));
                                if (newDestance < distance)
                                {
                                    _order = j+1;
                                    distance = newDestance;
                                    nextX = (int)_font.MeasureString(builder.ToString()).X;
                                    if (j == lines[i].Length - 1)
                                    {
                                        _vectorCursor = _vectorLeftTop + new Vector2(15, 15) + new Vector2(nextX - 2, y);
                                        _currentLine = i;
                                    }
                                }
                                else if (newDestance > distance)
                                {
                                    _vectorCursor = _vectorLeftTop + new Vector2(15, 15) + new Vector2(nextX-2,y);
                                    _currentLine = i;
                                    break;
                                }
                            }
                            for (byte j = 0; j < i; j++)
                            {
                                _order += lines[j].Length;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, _vectorLeftTop, _rectSourceTop, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestTop, _rectSourceMiddle, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _vectorTopRight, _rectSourceTop, Color.White, 0, Vector2.Zero, _vectorScale, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestMiddle, _rectSourceMiddle, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _vectorBottomLeft, _rectSourceBottom, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestBottom, _rectSourceMiddle, Color.White);
            spriteBatch.Draw(ControlResources.Atlas, _vectorRightBottom, _rectSourceBottom, Color.White, 0, Vector2.Zero, _vectorScale, SpriteEffects.FlipHorizontally, 0);
            if (_text != null) spriteBatch.DrawString(_font, _text, _vectorLeftTop + new Vector2(15, 15), Color.Black); 
            if (_isFocused)
            {
                if (_clock == 4294967294) _clock = 0;
                if (_clock % 30 > 15)
                {
                    spriteBatch.DrawString(Font, "|", _vectorCursor, Color.Blue);
                }
            }
        }

        public override bool ClickInControl()
        {
            _clock++;
            bool flag = false;
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
                    SetCursor(InputManager.Instance.MouseState.X, InputManager.Instance.MouseState.Y);
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
                        flag = true;
                        if (_lastPressed > 0)
                        {
                            if (DateTime.Now.TimeOfDay.TotalMilliseconds > _lastPressed + 300)
                            {
                                _isFocused = true;
                                if(Click != null) new Thread(new ThreadStart(Click)) { IsBackground = true }.Start();
                                _lastPressed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
                            }
                        }
                        else
                        {
                            _isFocused = true;
                            if(Click != null) new Thread(new ThreadStart(Click)) { IsBackground = true }.Start();
                            _lastPressed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
                        }
                    }
                }
                else _isPressed = false;
            }
            return flag;
        }

        public override void Offset(int x, int y)
        {
            throw new NotImplementedException("Метод TextArea.Offset не реализован");
        }

        public override void SetPosition(int x, int y)
        {
            throw new NotImplementedException();
        }

        private class Symbol
        {
            public char Char { get; set; }
            public Vector2 Position { get; set; }
            public int Order { get; set; }
            public float Width { get; set; }

            public Symbol(char symbol, int order, Vector2 position, float width)
            {
                Char = symbol;
                Order = order;
                Width = width;
                Position = position;
            }
        }
    }
}