using System;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public class TextBox:Control, ITextEditor
    {
        static object _locking = new object();
        ButtonState _lastState;
        public Color TextColor, FrameColor, BackColor;
        Vector2 _vectorText, _vectorCursor;
        Rectangle[] _rectsSourceFrame, _rectsDestFrame, _rectsSourceBG, _rectsDestsBG;
        string _text;
        ushort _clock, _cursorPos;
        float _offsetCursor;

        bool _isFocused, _fixedSize;
        TextAlign _align;

        public event Action<TextBox> OnClick;

        public bool WithShadow { get; set; }

        public TextBox(int x, int y, int width)
        {
            Vector2 vector = Font.MeasureString("|");
            int height = (int)vector.Y;
            _offsetCursor = vector.X / 4;
            height += height / 3;
            Rect = new Rectangle(x, y, width, height);
            _rectsDestFrame = SpliteRect(Rect, 12);
            _rectsSourceFrame = SpliteRect(ControlResources.AtlasRegions["Frame"], 12);
            _rectsDestsBG = SpliteRect(new Rectangle(x+1, y+1, width -2, height-2), 12);
            _rectsSourceBG = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            _text = "";
            _cursorPos = 0;
            Align = TextAlign.Left;
            AlignText();
            TextColor = ControlResources.DefaultColor;
            FrameColor = Color.Black;
            BackColor = Color.White;
        }

        public TextAlign Align
        {
            get { return _align; }
            set
            {
                _align = value;
                AlignText();
            }
        }

        public bool FixedSize
        {
            get { return _fixedSize; }
            set
            {
                _fixedSize = value;
            }
        }

        void AlignText()
        {
            Vector2 temp;
            if (string.IsNullOrEmpty(_text)) temp = Font.MeasureString("A");
            else temp = Font.MeasureString(_text);
            switch (Align)
            {
                case TextAlign.Left:
                    _vectorText = new Vector2(Rect.X + Rect.Height / 4, Rect.Y + (Rect.Height - (int)temp.Y) / 2);
                    break;
                case TextAlign.Center:
                    _vectorText = new Vector2(Rect.X + (Rect.Width - (int)temp.X) / 2, Rect.Y + (Rect.Height - (int)temp.Y) / 2);
                    break;
                case TextAlign.Right:
                    _vectorText = new Vector2(Rect.X + Rect.Width - (int)temp.X - Rect.Height / 4, Rect.Y + (Rect.Height - (int)temp.Y) / 2);
                    break;
            }
            SetCursorPosition();
        }

        void SetCursorPosition()
        {
            Vector2 temp = _vectorText;
            string substring = _text.Substring(0, _cursorPos);
            temp.X += Font.MeasureString(substring).X - _offsetCursor;
            _vectorCursor = temp;
        }

        public bool Focused
        {
            get { return _isFocused; }
            set 
            {
                if (value)
                {
                    if(_textEditable != null) _textEditable.Focused = false;
                    _textEditable = this;
                }
                _isFocused = value; 
            }
        }
		
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if ((int)Font.MeasureString(_text).X > Rect.Width)
                {
                    Rect.Width += Rect.Height;
                    _rectsDestFrame = SpliteRect(Rect, 12);
                    _rectsDestsBG = SpliteRect(new Rectangle(Rect.X + 1, Rect.Y + 1, Rect.Width - 2, Rect.Height - 2), 12);
                }
                AlignText();
            }
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            Vector2 vector = Font.MeasureString("|");
            int height = (int)vector.Y;
            _offsetCursor = vector.X / 4;
            Rect.Height = height;
            _rectsDestFrame = SpliteRect(Rect, 12);
            _rectsDestsBG = SpliteRect(new Rectangle(Rect.X + 1, Rect.Y + 1, Rect.Width - 2, height - 2), 12);
            AlignText();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override bool ClickInControl()
        {
            bool flag = false;
            if (!Enabled) return false;
            if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
            {
                if (InputManager.Instance.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released
                   && _lastState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    Focused = true;
                    _clock = 0;
                    StringBuilder builder = new StringBuilder(_text.Length);
                    int x = InputManager.Instance.GetMousePositionToVector2().X;
                    if (x < (int)_vectorText.X) _cursorPos = 0;
                    else if (x > (int)(_vectorText.X + Font.MeasureString(_text).X)) _cursorPos = (ushort)_text.Length;
                    else
                    {
                        int destination = 10000;
                        foreach (char c in _text)
                        {
                            builder.Append(c);
                            int currentDestination = Math.Abs((int)(Font.MeasureString(builder.ToString()).X + _vectorText.X) - x);
                            if (currentDestination < destination)
                            {
                                destination = currentDestination;
                            }
                            else
                            {
                                _cursorPos = (ushort)(builder.Length - 1);
                                break;
                            }
                        }
                    }
                    SetCursorPosition();
                    if (OnClick != null) OnClick(this);
                    Thread.Sleep(300);
                    flag = true;
                }
                _lastState = InputManager.Instance.MouseState.LeftButton;
            }
            return flag;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[0], _rectsSourceBG[0], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[1], _rectsSourceBG[1], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[2], _rectsSourceBG[2], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[3], _rectsSourceBG[3], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[4], _rectsSourceBG[4], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[5], _rectsSourceBG[5], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[6], _rectsSourceBG[6], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[7], _rectsSourceBG[7], BackColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestsBG[8], _rectsSourceBG[8], BackColor);

            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[0], _rectsSourceFrame[0], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[1], _rectsSourceFrame[1], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[2], _rectsSourceFrame[2], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[3], _rectsSourceFrame[3], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[4], _rectsSourceFrame[4], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[5], _rectsSourceFrame[5], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[6], _rectsSourceFrame[6], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[7], _rectsSourceFrame[7], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectsDestFrame[8], _rectsSourceFrame[8], FrameColor);

            if (Focused && Enabled)
            {
                spriteBatch.DrawString(Font, _text, _vectorText, TextColor);
                if (_clock % 60 < 30)
                {
                    spriteBatch.DrawString(Font, "|", _vectorCursor, TextColor);
                }
            }
            else if(!string.IsNullOrEmpty(_text))
            {
                spriteBatch.DrawString(Font, _text, _vectorText, TextColor);
            }
            _clock++;
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x- Rect.X, y- Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _rectsDestFrame[0].Offset(x, y);
            _rectsDestFrame[1].Offset(x, y);
            _rectsDestFrame[2].Offset(x, y);
            _rectsDestFrame[3].Offset(x, y);
            _rectsDestFrame[4].Offset(x, y);
            _rectsDestFrame[5].Offset(x, y);
            _rectsDestFrame[6].Offset(x, y);
            _rectsDestFrame[7].Offset(x, y);
            _rectsDestFrame[8].Offset(x, y);
            _rectsDestsBG[0].Offset(x, y);
            _rectsDestsBG[1].Offset(x, y);
            _rectsDestsBG[2].Offset(x, y);
            _rectsDestsBG[3].Offset(x, y);
            _rectsDestsBG[4].Offset(x, y);
            _rectsDestsBG[5].Offset(x, y);
            _rectsDestsBG[6].Offset(x, y);
            _rectsDestsBG[7].Offset(x, y);
            _rectsDestsBG[8].Offset(x, y);
            _vectorText.X += x;
            _vectorText.Y += y;
        }

        public void Append(char symbol)
        {
            lock (_locking)
            {
                if (Font.Characters.Contains(symbol))
                {
                    Text = _text.Insert(_cursorPos, symbol.ToString());
                    _cursorPos++;
                    AlignText();
                }
            }
        }

        public void DeleteOneSymbol()
        {
            lock (_locking)
            {
                if (_cursorPos < _text.Length)
                {
                    _text = _text.Remove(_cursorPos, 1);
                    AlignText();
                }
            }
        }

        public void BackSpaceOneSymbol()
        {
            lock (_locking)
            {
                if (_cursorPos > 0)
                {
                    _cursorPos--;
                    _text = _text.Remove(_cursorPos, 1);
                    AlignText();
                }
            }
        }

        public void Return()
        {

        }

        public void MoveUp()
        {
            
        }

        public void MoveDown()
        {
            
        }

        public void MoveLeft()
        {
            lock (_locking)
            {
                if (_cursorPos > 0)
                {
                    _clock = 0;
                    _cursorPos--;
                    SetCursorPosition();
                }
            }
        }

        public void MoveRight()
        {
            lock (_locking)
            {
                if (_cursorPos < _text.Length)
                {
                    _clock = 0;
                    _cursorPos++;
                    SetCursorPosition();
                }
            }
        }

        public void MoveHome()
        {
            lock (_locking)
            {
                _cursorPos = 0;
                SetCursorPosition();
            }
        }

        public void MoveEnd()
        {
            lock (_locking)
            {
                _cursorPos = (ushort)_text.Length;
                SetCursorPosition();
            }
        }
    }
}