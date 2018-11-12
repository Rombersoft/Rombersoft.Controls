using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    /// <summary>
    /// Control. Basic class for all controls
    /// </summary>
    public abstract class Control
    {
        static Keys _lastPressedKey = Keys.None;
        public string Name;
        public bool Visibled;
        internal static ITextEditor _textEditable;
        /// <summary>
        /// Задает или получает размер занимаемой прямоугольной области
        /// </summary>
        public Rectangle Rect;
        /// <summary>
        /// Возвращает позицию контрола
        /// </summary>
        /// <summary>
        /// Получает или задает родителя
        /// </summary>
        /// <value>Родитель - экземпляр класса Control, который является контейнером для этого контрола</value>
        public Control Parent;
        /// <summary>
        /// Получает или задает доступность к контролу
        /// </summary>
        /// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
        public bool Enabled;
        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        /// <value>The controls.</value>
        protected List<Control> Controls;
        /// <summary>
        /// Получает или задает позицию вставки при перемещении
        /// </summary>
        /// <value>The insert position.</value>
        public Control()
        {
            Controls = new List<Control>(5);
            Enabled = true;
            Visibled = true;
            Font = ControlResources.DefaultFont;
        }

        protected SpriteFont Font;
        public abstract void Draw();
        public void DrawInternal(SpriteBatch spriteBatch)
        {
            byte internalCount = (byte)Controls.Count;
            for (byte i = 0; i < internalCount; i++)
                Controls[i].Draw(spriteBatch);
        }

        /// <summary>
        /// Рисует контролы
        /// </summary>
        public abstract void Draw(SpriteBatch spriteBatch);
        /// <summary>
        /// Обрабатывает нажатия и положение курсора мыши
        /// </summary>
        /// <returns><c>true</c>, if in control was clicked, <c>false</c> otherwise.</returns>
        public abstract bool ClickInControl();
        public bool ClickInternal()
        {
            byte internalCount = (byte)Controls.Count;
            for (byte i = 0; i < internalCount; i++)
                if(Controls[i].ClickInControl()) return true;
            return false;
        }
        /*{
            if(InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if(InputManager.Instance.MouseState.X > Rect.X && InputManager.Instance.MouseState.X > Rect.Width + Rect.X)
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].Rect.Y + Controls[i].Rect.Height < 0 || Controls[i].Rect.Y > GraphicInstance.Window.Height
                        || Controls[i].Rect.X + Controls[i].Rect.Width < 0 || Controls[i].Rect.X > GraphicInstance.Window.Width)
                        continue;
                    Controls[i].ClickInControl();
                }
            }
            return true;
        }*/
        /// <summary>
        /// Устанавливает позицию контрола
        /// </summary>
        /// <param nameRU="x">новая позиция Х</param>
        /// <param nameRU="y">новая позиция У</param>
        public abstract void SetPosition(int x, int y);
        /// <summary>
        /// Смещает позицию котрола на указаное количество пикселей
        /// </summary>
        /// <param nameRU="x">шаг смешения по Х</param>
        /// <param nameRU="y">шаг смещения по Y</param>
        /// <returns>новую позицию контрола</returns>
        public abstract void Offset(int x, int y);
       
        protected static Color GetInverse(Color color)
        {
            //if (color == Color.White) return Color.DarkSlateGray;
            byte r, g, b;
            r = color.R;
            g = color.G;
            b = color.B;
            return new Color(255 - r, 255 - g, 255 - b, color.A);
        }

        public static string FormatString(string source, int maxLength, SpriteFont font)
        {
            StringBuilder builder = new StringBuilder(1000);
            foreach(char c in source)
            {
                if ((font.Characters.Contains(c)) || (c == '\n') || (c == '\r'))
                    builder.Append(c);
            }
            string[] str = builder.ToString().Split(' ');
            string text = str[0];
            builder.Clear();
            for (int j = 1; j < str.Length; j++)
            {
                if(font.MeasureString(text + " " + str[j]).X < maxLength) text += (" " + str[j]);
                else
                {
                    builder.Append(text);
                    text = string.Empty;
                    if (j <= str.Length - 1)
                    {
                        builder.Append("\n");
                        text = str[j];
                    }
                }
            }
            if(!string.IsNullOrEmpty(text)) builder.Append(text);
            return builder.ToString();
        }

        public static string RemoveUnsupportedSymbols(string source, SpriteFont font)
        {
            StringBuilder builder = new StringBuilder();
            for (short j = 0; j < source.Length; j++)
            {
                if (font.Characters.Contains(source[j]) || source[j] == '\n' || source[j] == '\r') builder.Append(source[j]);
                else builder.Append(' ');
            }
            return builder.ToString();
        }

        public static Rectangle[] SpliteRect(Rectangle rectSource, int cornerSize)
        {
            Rectangle[] rectResult = new Rectangle[9];
            rectResult[0] = new Rectangle(rectSource.X,rectSource.Y,cornerSize,cornerSize);
            rectResult[1] = new Rectangle(rectSource.X + cornerSize, rectSource.Y, rectSource.Width - cornerSize*2, cornerSize);
            rectResult[2] = new Rectangle(rectSource.X + rectSource.Width - cornerSize,rectSource.Y,cornerSize,cornerSize);
            rectResult[3] = new Rectangle(rectSource.X, rectSource.Y + cornerSize, cornerSize, rectSource.Height - cornerSize * 2);
            rectResult[4] = new Rectangle(rectSource.X + cornerSize, rectSource.Y + cornerSize, rectSource.Width - cornerSize * 2, rectSource.Height - cornerSize * 2);
            rectResult[5] = new Rectangle(rectSource.X + rectSource.Width - cornerSize, rectSource.Y + cornerSize, cornerSize, rectSource.Height - cornerSize * 2);
            rectResult[6] = new Rectangle(rectSource.X, rectSource.Y + rectSource.Height - cornerSize, cornerSize, cornerSize);
            rectResult[7] = new Rectangle(rectSource.X + cornerSize, rectSource.Y + rectSource.Height - cornerSize, rectSource.Width - cornerSize * 2, cornerSize);
            rectResult[8] = new Rectangle(rectSource.X + rectSource.Width - cornerSize, rectSource.Y + rectSource.Height - cornerSize, cornerSize, cornerSize);
            return rectResult;
        }

        protected SpriteFont GetBestFont(int buttonHeight, string fontFamily)
        {
            buttonHeight -= buttonHeight / 5;
            int maxHeight = 0;
            SpriteFont font = null;
            fontFamily = fontFamily.ToLower();
            foreach (var key in ControlResources.Fonts.Keys)
            {
                if (key.ToLower().StartsWith(fontFamily))
                {
                    int height = (int)ControlResources.Fonts[key].MeasureString("A").Y;
                    if (height < buttonHeight && height > maxHeight)
                    {
                        maxHeight = height;
                        font = ControlResources.Fonts[key];
                    }
                }
            }
            return font;
        }

        public static void Window_TextInput(object sender, TextInputEventArgs e)
        {
             if (_textEditable != null)
            {
                switch (e.Key)
                {
                    case Keys.Enter:
                    case Keys.Back:
                    case Keys.Delete:
                        break;
                    default:
                        _textEditable.Append(e.Character);
                        break;
                }
            }
        }

        public static void UpdateKeyboard()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] keys = keyboardState.GetPressedKeys();
            if (keys.Length > 0)
            {
                if (keys[0] != _lastPressedKey)
                {
                    if (_textEditable != null)
                    {
                        _lastPressedKey = keys[0];
                        switch (keys[0])
                        {
                            case Keys.End:
                                _textEditable.MoveEnd();
                                break;
                            case Keys.Home:
                                _textEditable.MoveHome();
                                break;
                            case Keys.Left:
                                _textEditable.MoveLeft();
                                break;
                            case Keys.Up:
                                _textEditable.MoveUp();
                                break;
                            case Keys.Right:
                                _textEditable.MoveRight();
                                break;
                            case Keys.Down:
                                _textEditable.MoveDown();
                                break;
                            case Keys.Back:
                                _textEditable.BackSpaceOneSymbol();
                                break;
                            case Keys.Delete:
                                _textEditable.DeleteOneSymbol();
                                break;
                            case Keys.Enter:
                                _textEditable.Return();
                                break;
                            case Keys.D0:
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                            case Keys.D9:
                            case Keys.NumPad0:
                            case Keys.NumPad1:
                            case Keys.NumPad2:
                            case Keys.NumPad3:
                            case Keys.NumPad4:
                            case Keys.NumPad5:
                            case Keys.NumPad6:
                            case Keys.NumPad7:
                            case Keys.NumPad8:
                            case Keys.NumPad9:
                            case Keys.A:
                            case Keys.B:
                            case Keys.C:
                            case Keys.D:
                            case Keys.E:
                            case Keys.F:
                            case Keys.G:
                            case Keys.H:
                            case Keys.I:
                            case Keys.J:
                            case Keys.K:
                            case Keys.L:
                            case Keys.M:
                            case Keys.N:
                            case Keys.O:
                            case Keys.P:
                            case Keys.Q:
                            case Keys.R:
                            case Keys.S:
                            case Keys.T:
                            case Keys.U:
                            case Keys.V:
                            case Keys.W:
                            case Keys.X:
                            case Keys.Y:
                            case Keys.Z:
                            case Keys.F1:
                            case Keys.F2:
                            case Keys.F3:
                            case Keys.F4:
                            case Keys.F5:
                            case Keys.F6:
                            case Keys.F7:
                            case Keys.F8:
                            case Keys.F9:
                            case Keys.F10:
                            case Keys.F11:
                            case Keys.F12:
                            case Keys.Tab:
                            case Keys.Pause:
                            case Keys.CapsLock:
                            case Keys.Kana:
                            case Keys.Kanji:
                            case Keys.Escape:
                            case Keys.ImeConvert:
                            case Keys.ImeNoConvert:
                            case Keys.Space:
                            case Keys.PageUp:
                            case Keys.PageDown:
                            case Keys.Select:
                            case Keys.Print:
                            case Keys.Execute:
                            case Keys.PrintScreen:
                            case Keys.Insert:
                            case Keys.Help:
                            case Keys.LeftWindows:
                            case Keys.RightWindows:
                            case Keys.Apps:
                            case Keys.Sleep:
                            case Keys.Multiply:
                            case Keys.Add:
                            case Keys.Separator:
                            case Keys.Subtract:
                            case Keys.Decimal:
                            case Keys.Divide:
                            case Keys.NumLock:
                            case Keys.Scroll:
                            case Keys.LeftShift:
                            case Keys.RightShift:
                            case Keys.LeftControl:
                            case Keys.RightControl:
                            case Keys.LeftAlt:
                            case Keys.RightAlt:
                            case Keys.BrowserBack:
                            case Keys.BrowserForward:
                            case Keys.BrowserRefresh:
                            case Keys.BrowserStop:
                            case Keys.BrowserSearch:
                            case Keys.BrowserFavorites:
                            case Keys.BrowserHome:
                            case Keys.VolumeMute:
                            case Keys.VolumeDown:
                            case Keys.VolumeUp:
                            case Keys.MediaNextTrack:
                            case Keys.MediaPreviousTrack:
                            case Keys.MediaStop:
                            case Keys.MediaPlayPause:
                            case Keys.LaunchMail:
                            case Keys.SelectMedia:
                            case Keys.LaunchApplication1:
                            case Keys.LaunchApplication2:
                            case Keys.OemSemicolon:
                            case Keys.OemPlus:
                            case Keys.OemComma:
                            case Keys.OemMinus:
                            case Keys.OemPipe:// \
                            case Keys.OemOpenBrackets: // [
                            case Keys.OemCloseBrackets: // ]
                            case Keys.OemPeriod:
                            case Keys.OemQuestion:
                            case Keys.OemTilde:
                                break;
                            default:
                                Console.WriteLine(keys[0]);
                                break;
                        }
                    }
                }
            }
            else
            {
                _lastPressedKey = Keys.None;
            }
        }
    }
}