using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public class Panel : Control
    {
        Action<SpriteBatch> _drawing;
        Rectangle[] _rectDest, _rectSource;
        public Color Backcolor;
        public bool FixedSize { get; private set; }
        public object Tag { get; set; }
        bool _pressed;
        /// <summary>
        /// Creates transparent panel/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Panel(int x, int y, int width, int height) : base()
        {
            Rect = new Rectangle(x, y, width, height);
            _pressed = false;
            _drawing = TransparentPanelDrawing;
        }
        /// <summary>
        /// Creates filled panel by region from atlas/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        /// <param name="stretched">If set to <c>true</c> that source rectangle was splitted on 3x3 parts. If set to <b>false</b> that source rectangle was zoomed up to width and height</param>
        /// <param name="cornerRadius">Corner radius. It must be setted when stretched = <c>true</c></param>
        public Panel(int x, int y, int width, int height, string atlasRegion, bool stretched, int cornerRadius = 0) : base()
        {
            Rect = new Rectangle(x, y, width, height);
            if (stretched)
            {
                if (cornerRadius == 0) throw new ArgumentException("cornerRadius must be set when stretched = true");
                _rectSource = SpliteRect(ControlResources.AtlasRegions[atlasRegion], cornerRadius);
                _rectDest = SpliteRect(Rect, cornerRadius);
            }
            else
            {
                _rectSource = new Rectangle[1];
                _rectSource[0] = ControlResources.AtlasRegions[atlasRegion];
                _rectDest = new Rectangle[1];
                _rectDest[0] = Rect;
            }
            _pressed = false;
            _drawing = FromAtlasPanelDrawing;
            Backcolor = Color.White;
        }

        public override bool ClickInControl()
        {
            bool flag = false;
            if (Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
            {
                if (ClickInternal()) return true;
                if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed) _pressed = true;
                else
                {
                    if (_pressed) flag = true;
                    _pressed = false;
                }
            }
            else _pressed = false;
            return flag;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _drawing(spriteBatch);
        }

        void TransparentPanelDrawing(SpriteBatch spriteBatch)
        {
            DrawInternal(spriteBatch);
        }

        void FromAtlasPanelDrawing(SpriteBatch spriteBatch)
        {
            for (byte i = 0; i < _rectDest.Length; i++)
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[i], _rectSource[i], Backcolor);
            DrawInternal(spriteBatch);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            if (_rectDest != null)
                for (int i = 0; i < _rectDest.Length; i++)
                {
                    _rectDest[i].Offset(x, y);
                }
            for (byte i = 0; i < Controls.Count; i++)
                Controls[i].Offset(x, y);
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }
        /// <summary>
        /// Adds the control. Note that will be right to create Panel with x=0, y=0 then add all controls on Panel and then Offset Panel to neccessary place
        /// </summary>
        /// <param name="control">Control.</param>
        public void AddControl(Control control)
        {
            Controls.Add(control);
        }

        public void RemoveControl(Control control)
        {
            Controls.Remove(control);
        }
    }
}