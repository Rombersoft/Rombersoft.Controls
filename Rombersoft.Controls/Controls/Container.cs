using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rombersoft.Controls
{
    public class Container:Control
    {
        private Rectangle[] _rectsSource, _rectsDests;
        private bool _transparent, _isShown;
        public Color BackColor;

        public Container(int x, int y, int width, int height, bool transparent)
        {
            _transparent = transparent;
            Rect = new Rectangle(x, y, width, height);
            _rectsSource = SpliteRect(ControlResources.AtlasRegions["BackgroundTransrarentRadial"], 12);
            _rectsDests = SpliteRect(Rect, 12);
            BackColor = Color.White;
            _isShown = false;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public void AddControl(Control control)
        {
            control.Offset(Rect.X, Rect.Y);
            Controls.Add(control);
        }

        public void RemoveControl(Control control)
        {
            control.Offset(-Rect.X, -Rect.Y);
            Controls.Remove(control);
        }

        public void Clear()
        {
            for (byte i = 0; i < Controls.Count; i++)
                RemoveControl(Controls[i]);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isShown)
            {
                if (!_transparent)
                {
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[0], _rectsSource[0], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[1], _rectsSource[1], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[2], _rectsSource[2], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[3], _rectsSource[3], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[4], _rectsSource[4], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[5], _rectsSource[5], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[6], _rectsSource[6], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[7], _rectsSource[7], BackColor);
                    spriteBatch.Draw(ControlResources.Atlas, _rectsDests[8], _rectsSource[8], BackColor);
                }
                DrawInternal(spriteBatch);
            }
        }

        public override bool ClickInControl()
        {
            if (!_isShown) return false;
            return ClickInternal();
        }
        
        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _rectsDests[0].Offset(x, y);
            _rectsDests[1].Offset(x, y);
            _rectsDests[2].Offset(x, y);
            _rectsDests[3].Offset(x, y);
            _rectsDests[4].Offset(x, y);
            _rectsDests[5].Offset(x, y);
            _rectsDests[6].Offset(x, y);
            _rectsDests[7].Offset(x, y);
            _rectsDests[8].Offset(x, y);
            for (int i = 0; i < Controls.Count; i++)
                Controls[i].Offset(x, y);
        }

        public void Show()
        {
            _isShown = true;
        }

        public void Close()
        {
            _isShown = false;
        }
    }
}
