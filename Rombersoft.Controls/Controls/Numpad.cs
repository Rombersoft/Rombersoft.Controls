using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Rombersoft.Controls
{
    public class NumPad:Control
    {
        KeyButton[] _buttons;
        Rectangle[] _rectSourceCanvas, _rectDestCanvas;
        public event Action<string> OnClick;
        public Color BackColor;
        static NumPad _instance = new NumPad();

        public static NumPad Instance
        {
            get
            {
                return _instance;
            }
        }

        private NumPad()
        {
            int minWindowSize = GraphicInstance.Window.Height < GraphicInstance.Window.Width ? GraphicInstance.Window.Height : GraphicInstance.Window.Width;
            int width = minWindowSize/2;
            int x = GraphicInstance.Window.Width - width;
            int height = (int)(width/4f*3f);
            int y = GraphicInstance.Window.Height - height;
            Rect = new Rectangle(x, y, width, height);
            _rectSourceCanvas = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            _rectDestCanvas = SpliteRect(Rect, 12);
            _buttons = new  KeyButton[12];
            int buttonWidth = (int)(width * 0.21);
            int buttonMargin = (int)(width * 0.035);
            for (int i = 0; i < 12; i++)
            {
                if (i < 9)
                {
                    string command = (i + 1).ToString();
                    _buttons[i] = new KeyButton(i % 3 * (buttonWidth + buttonMargin) + x + buttonMargin, i / 3 * (buttonWidth + buttonMargin) + y + buttonMargin, 
                                                buttonWidth, buttonWidth);
                    _buttons[i].OnClicked += () => { Button_Click(command); };
                    _buttons[i].Text((i + 1).ToString());
                }

                if (i == 9)
                {
                    _buttons[i] = new KeyButton(3 * (buttonWidth + buttonMargin) + x + buttonMargin, y + buttonMargin, buttonWidth, buttonWidth);
                    _buttons[i].OnClicked += () => Button_Click("del");
                    _buttons[i].Icon = ControlResources.AtlasRegions["IconBackspace"];
                }

                if (i == 10)
                {
                    _buttons[i] = new KeyButton(3 * (buttonWidth + buttonMargin) + x + buttonMargin, buttonWidth + buttonMargin + y + buttonMargin, buttonWidth, buttonWidth);
                    _buttons[i].OnClicked += () => Button_Click("0");
                    _buttons[i].Text("0");
                }

                if (i == 11)
                {
                    _buttons[i] = new KeyButton(3 * (buttonWidth + buttonMargin) + x + buttonMargin, 2*(buttonWidth + buttonMargin) + y + buttonMargin, buttonWidth, buttonWidth);
                    _buttons[i].OnClicked += () => { Button_Click("reset"); };
                    _buttons[i].Text("СE");          
                }
            }
            BackColor = Color.White;
        }

        public void SetButtonColor(Color borderColor, Color bodyColor, Color textColor)
        {
            for (byte i = 0; i < 12; i++)
            {
                _buttons[i].BackColor = bodyColor;
                _buttons[i].BorderColor = borderColor;
                _buttons[i].TextColor = textColor;
            }
        }

        public void SetFont(SpriteFont font)
        {
            for (byte i = 0; i < 12; i++)
            {
                _buttons[i].SetFont(font);
            }
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[0], _rectSourceCanvas[0], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[1], _rectSourceCanvas[1], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[2], _rectSourceCanvas[2], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[3], _rectSourceCanvas[3], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[4], _rectSourceCanvas[4], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[5], _rectSourceCanvas[5], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[6], _rectSourceCanvas[6], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[7], _rectSourceCanvas[7], BackColor);
            GraphicInstance.SpriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[8], _rectSourceCanvas[8], BackColor);
            for (byte i = 0; i < 12; i++)
            {
                _buttons[i].Draw(spriteBatch);
            }
        }

        public override bool ClickInControl()
        {
            for (byte i = 0; i < 12; i++)
            {
                _buttons[i].ClickInControl();
            }
            return false;
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            for (byte i = 0; i < 12; i++)
            {
                _buttons[i].Offset(x, y);
            }
        }

        private void Button_Click(string command)
        {
            if (OnClick != null) OnClick(command);
        }
    }
}
