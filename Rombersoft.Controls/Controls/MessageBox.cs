using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class MessageBox : Control
    {
        private Rectangle[] _rectSourceCanvas, _rectDestCanvas;
        private Rectangle _rectSourceWarning, _rectDestIcon;
        private EventWaitHandle _ewh;
        private ButtonRound[] _buttons;
        private Label _labelTitle, _labelMessage;
        public Color IconColor, BackColor;
        private Action _syncAction;

        /// <summary>
        /// Creates window aspect ratio 4:3 with width in 0.8 * (Window application width)
        /// </summary>
        public MessageBox() : base()
        {
            if(GraphicInstance.Window.Height == 0 || GraphicInstance.Window.Width == 0) throw new Exception("Invalid a Window aspect ratio");
            _rectSourceCanvas = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            _rectSourceWarning = ControlResources.AtlasRegions["IconWarning"];
            int minWindowSize = GraphicInstance.Window.Height < GraphicInstance.Window.Width ? GraphicInstance.Window.Height : GraphicInstance.Window.Width;
            int width = (int)(minWindowSize * 0.8f);
            int x = (GraphicInstance.Window.Width - width) / 2;
            int height = (int)(width * 0.66);
            int y = (GraphicInstance.Window.Height - height) / 2;
            Rect = new Rectangle(x, y, width, height);
            _rectDestCanvas = SpliteRect(Rect, 12);
            _rectDestIcon = new Rectangle(x + (int)(width * 0.8), y + (int)(width * 0.05), (int)(width * 0.15), (int)(width * 0.15 / 1.16));
            IconColor = Color.Chocolate;
            BackColor = Color.White;
            Visibled = false;
            _ewh = new AutoResetEvent(false);
            _syncAction = null;
            _buttons = new ButtonRound[0];
            _labelTitle = new Label(0, 0);
            _labelMessage = new Label(0, 0);
        }

        public void AddControl(Control control)
        {
            control.Offset(Rect.X, Rect.Y);
            Controls.Add(control);
        }

        public void SetTwoButtons(string[] text, Action[] actions, Color backColor, Color textColor)
        {
            _syncAction += () =>
            {
                _buttons = new ButtonRound[2];
                _buttons[0] = new ButtonRound(Rect.X + Rect.Width / 10, Rect.Y + (int)(Rect.Height * 0.9), (int)(Rect.Width * 0.3), Rect.Height / 5, "ButtonSimple",
                                              "MB_first", backColor);
                _buttons[1] = new ButtonRound(Rect.X + (int)(Rect.Width * 0.6), Rect.Y + (int)(Rect.Height * 0.9), (int)(Rect.Width * 0.3), Rect.Height / 5, "ButtonSimple",
                                              "MB_first", backColor);
                _buttons[0].PropertyText(Align.Center, GetBestFont(Rect.Height / 5, "roboto"), textColor);
                _buttons[1].PropertyText(Align.Center, GetBestFont(Rect.Height / 5, "roboto"), textColor);
                _buttons[0].Text(text[0]);
                _buttons[1].Text(text[1]);
                _buttons[0].OnClicked += () =>
                {
                    _ewh.Set();
                    Visibled = false;
                    actions[0]();
                };
                _buttons[1].OnClicked += () =>
                {
                    _ewh.Set();
                    Visibled = false;
                    actions[1]();
                };
            };
        }

        public void SetOneButton(string text, Action actions, Color backColor, Color textColor)
        {
            _syncAction += () =>
            {
                _buttons = new ButtonRound[1];
                _buttons[0] = new ButtonRound(Rect.X + (int)(Rect.Width * 0.35), Rect.Y + (int)(Rect.Height * 0.9), (int)(Rect.Width * 0.3), Rect.Height / 5, "ButtonSimple",
                                              "MB_first", backColor);
                _buttons[0].PropertyText(Align.Center, GetBestFont(Rect.Height / 5, "roboto"), textColor);
                _buttons[0].Text(text);
                _buttons[0].OnClicked += () =>
                {
                    _ewh.Set();
                    Visibled = false;
                    actions();
                };
            };
        }

        public void SetTitle(string text, Color textColor)
        {
            _syncAction += () =>
            {
                SpriteFont font = GetBestFont(_rectDestIcon.Height, "ubuntu");
                _labelTitle.SetFont(font);
                _labelTitle.Text = text;
                _labelTitle.Color = textColor;
                Vector2 vector = font.MeasureString(text);
                _labelTitle.SetPosition((Rect.Width - (int)vector.X) / 2 + Rect.X, (int)(vector.Y * 0.25) + _rectDestIcon.Y);
            };
        }
        /// <summary>
        /// Sets the text. It makes aligh on center
        /// </summary>
        /// <param name="x">Desired X coordinate.</param>
        /// <param name="text">Text.</param>
        /// <param name="font">Font.</param>
        /// <param name="color">Color.</param>
        public void SetText(int x, string text, SpriteFont font, Color color)
        {
            text = FormatString(text, Rect.Width - 2 * x, font);
            int height = (int)font.MeasureString(text).Y;
            int y = (Rect.Y + (int)(Rect.Height * 0.9) - (_rectDestIcon.Y + _rectDestIcon.Height) - height) / 2 + _rectDestIcon.Y + _rectDestIcon.Height;
            string[] lines = text.Split('\n');
            for (byte i = 0; i < lines.Length; i++)
            {
                Vector2 pos = font.MeasureString(lines[i]);
                Label label = new Label((Rect.Width - (int)pos.X) / 2 + Rect.X, y);
                label.Color = color;
                label.SetFont(font);
                label.Text = lines[i];
                Controls.Add(label);
                y += (int)pos.Y;
            }
        }

        /// <summary>
        /// Show MessageBox during timeout.
        /// </summary>
        /// <param name="timeout">Timeout for showing in milliseconds</param>
        public void Show(int timeout=0)
        {
            Visibled = true;
            if (timeout > 0)
            {
                new Thread(() =>
                    {
                        _ewh.WaitOne(timeout);
                        Visibled = false;
                    }){ IsBackground = true }.Start();
            }
        }

        public bool ShowModal(int timeout = 0)
        {
            Visibled = true;
            bool flag;
            if (timeout > 0) flag = _ewh.WaitOne(timeout);
            else flag = _ewh.WaitOne();
            Visibled = false;
            return flag;
        }

        public void Close()
        {
            Visibled = false;
        }

        public void Clear()
        {
            if(Controls.Count == 0) return;
            Visibled = false;
            _ewh.Reset();
            for (byte i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] != null)
                {
                    Controls[i].Offset((int)-Rect.X, (int)-Rect.Y);
                }
            }
            Controls.Clear();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visibled)
            {
                spriteBatch.Draw(ControlResources.Brush, GraphicInstance.Window, Color.FromNonPremultiplied(0,0,0,200));
                for (byte i = 0; i < 9; i++)
                    spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[i], _rectSourceCanvas[i], BackColor);
                _labelTitle.Draw(spriteBatch);
                _labelMessage.Draw(spriteBatch);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceWarning, IconColor);
                byte internalCount = (byte)Controls.Count;
                for (byte i = 0; i < internalCount; i++)
                {
                    Controls[i].Draw(spriteBatch);
                }
                for (byte i = 0; i < _buttons.Length; i++)
                {
                    _buttons[i].Draw();
                }
            }
            if (_syncAction != null)
            {
                _syncAction();
                _syncAction = null;
            }
        }

        public override bool ClickInControl()
        {
            if (Visibled)
            {
                for (byte i = 0; i < _buttons.Length; i++)
                {
                    if(_buttons[i] != null) _buttons[i].ClickInControl();
                }
            }
            return false;
        }

        public override void SetPosition(int x, int y)
        {
            //Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            /*Rect.Offset(x, y);
            _rectDestCanvas[0].Offset(x, y);
            _rectDestCanvas[1].Offset(x, y);
            _rectDestCanvas[2].Offset(x, y);
            _rectDestCanvas[3].Offset(x, y);
            _rectDestCanvas[4].Offset(x, y);
            _rectDestCanvas[5].Offset(x, y);
            _rectDestCanvas[6].Offset(x, y);
            _rectDestCanvas[7].Offset(x, y);
            _rectDestCanvas[8].Offset(x, y);
            _rectDestIcon.Offset(x, y);
            for (byte i = 0; i < Controls.Count; i++)
                Controls[i].Offset(x, y);*/
        }
    }
}
