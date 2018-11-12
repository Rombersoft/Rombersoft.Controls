using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public class Popup:Control
    {
        Rectangle[] _rectSourceCanvas, _rectDestCanvas;
        Rectangle _rectSourceIcon, _rectDestIcon;
        Vector2 _textPosition;
        Color TextColor, CanvasColor, IconColor;
        int _step, _shownY, _closedY;
        string _text;
        AutoResetEvent _resetEvent;
        static Popup _instance = new Popup();
        public static Popup Instance { get { return _instance; } }
        /// <summary>
        /// Creates popup message window with size 600450 пикселей
        /// </summary>
        Popup()
        {
            int minWindowSize = GraphicInstance.Window.Height < GraphicInstance.Window.Width ? GraphicInstance.Window.Height : GraphicInstance.Window.Width;
            int width = (int)(minWindowSize * 0.95f);
            int x = GraphicInstance.Window.Width - width - minWindowSize/40;
            int height = width/10;
            _shownY = GraphicInstance.Window.Height - height - minWindowSize/40;
            Rect = new Rectangle(x, _shownY, width, height);
            _rectSourceCanvas = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            _rectDestCanvas = SpliteRect(Rect, 12);
            _rectSourceIcon = ControlResources.AtlasRegions["IconWarning"];
            _rectDestIcon = new Rectangle(Rect.X + Rect.Width - (int)(Rect.Height * 0.9f), Rect.Y + (Rect.Height - (int)(Rect.Height * 0.6 / 1.16))/2, 
                                          (int)(Rect.Height*0.6), (int)(Rect.Height * 0.6 / 1.16));
            _textPosition = new Vector2(Rect.X + Rect.Width/40, Rect.Y);
            Font = ControlResources.Fonts["Ubuntu12"];
            TextColor = Color.Gray;
            CanvasColor = Color.Lavender;
            IconColor = Color.Chocolate;
            _text = string.Empty;
            _step = (GraphicInstance.Window.Height - _shownY) / 40;
            int residue = (GraphicInstance.Window.Height - _shownY) % 40;
            if (residue > 0) _closedY = _step * (40 + residue) + _shownY;
            else _closedY = GraphicInstance.Window.Height;
            _resetEvent = new AutoResetEvent(false);
        }
        /// <summary>
        /// Show popup window
        /// </summary>
        /// <param name="message">Message text.</param>
        public void Show(string message)
        {
            Task.Run(() => 
            {
                _resetEvent.WaitOne(15000);
                Close();
            });
            if (message == null) return;
            _step = -Math.Abs(_step);
            SetPosition(Rect.X, _closedY);
            _text = FormatString(message, _rectDestIcon.X - (int)_textPosition.X - Rect.Width / 20, Font);            
            Offset(0, _step);
        }
        /// <summary>
        /// Закриває спливаюче вікно
        /// </summary>
        public void Close()
        {
            _step = Math.Abs(_step);
            Offset(0, _step);
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override bool ClickInControl()
        {
            if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()) && InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed && _step < 0)
                _resetEvent.Set();
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[0], _rectSourceCanvas[0], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[1], _rectSourceCanvas[1], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[2], _rectSourceCanvas[2], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[3], _rectSourceCanvas[3], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[4], _rectSourceCanvas[4], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[5], _rectSourceCanvas[5], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[6], _rectSourceCanvas[6], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[7], _rectSourceCanvas[7], CanvasColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestCanvas[8], _rectSourceCanvas[8], CanvasColor);
            spriteBatch.DrawString(Font, _text, _textPosition, TextColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestIcon, _rectSourceIcon, IconColor);
            if (Rect.Y < _closedY && Rect.Y > _shownY) Offset(0, _step);
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            _rectDestCanvas[0].Offset(x, y);
            _rectDestCanvas[1].Offset(x, y);
            _rectDestCanvas[2].Offset(x, y);
            _rectDestCanvas[3].Offset(x, y);
            _rectDestCanvas[4].Offset(x, y);
            _rectDestCanvas[5].Offset(x, y);
            _rectDestCanvas[6].Offset(x, y);
            _rectDestCanvas[7].Offset(x, y);
            _rectDestCanvas[8].Offset(x, y);
            Rect.Offset(x, y);
            _rectDestIcon.Offset(x, y);
            _textPosition.X += x;
            _textPosition.Y += y;
        }
    }
}
