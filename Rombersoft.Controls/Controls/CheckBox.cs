using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public class CheckBox:Control
    {
        private string _text;
        private Color _textColor, _controlColor;
        private Rectangle _rectSourceContainer, _rectSourcePoint, _rectSourceGlass, _rectDestContainer, _rectDestPoint;
        private Vector2 _vectorDestText;
        private bool _isPressed = false;
        public bool Checked, WithGlass;
        private Action _action;
        private float _scale;
        int _widthAnimation;

        public CheckBox(int x, int y, SpriteFont font, Color controlColor, Action action)
        {
            _text = string.Empty;
            Font = font;
            _controlColor = controlColor;
            int size = (int)Font.MeasureString("A").Y;
            _scale =  size / 80f;
            _rectSourceContainer = ControlResources.AtlasRegions["CheckBoxContainer"];
            _rectSourcePoint = ControlResources.AtlasRegions["CheckPoxPoint"];
            _rectSourceGlass = ControlResources.AtlasRegions["RadioButtonContainer"];
            _rectDestContainer = new Rectangle(x, y, size, size);
            _rectDestPoint = new Rectangle(x + (int)(10 * _scale), y + (int)(18 * _scale), (int)(_rectSourcePoint.Width * _scale), (int)(_rectSourcePoint.Height * _scale));
            _vectorDestText = new Vector2(x + size + (int)(size * 0.3) , y);
            _action = action;
            Rect = new Rectangle(x, y, size, size);
            Checked = WithGlass = false;
        }

        public void Text(string text, Color color)
        {
            _text = text;
            _textColor = color;
            Rect = new Rectangle(_rectDestContainer.X, _rectDestContainer.Y, _rectDestContainer.Width + (int)(_rectDestContainer.Width * 0.3 + Font.MeasureString(_text).X),
                                 _rectDestContainer.Width);
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, _rectDestContainer, _rectSourceContainer, _controlColor);
            if (Checked)
            {
                if (_widthAnimation < _rectSourcePoint.Width)
                {
                    spriteBatch.Draw(ControlResources.Atlas, new Rectangle(_rectDestPoint.X, _rectDestPoint.Y, (int)(_widthAnimation * _scale), _rectDestPoint.Height), 
                                     new Rectangle(_rectSourcePoint.X, _rectSourcePoint.Y, _widthAnimation, _rectSourcePoint.Height), _controlColor);
                    _widthAnimation += 4;
                }
                else spriteBatch.Draw(ControlResources.Atlas, _rectDestPoint, _rectSourcePoint, _controlColor);
            }
            else if (_widthAnimation > 0)
            {
                spriteBatch.Draw(ControlResources.Atlas, new Rectangle(_rectDestPoint.X, _rectDestPoint.Y, (int)(_widthAnimation * _scale), _rectDestPoint.Height),
                                 new Rectangle(_rectSourcePoint.X, _rectSourcePoint.Y, _widthAnimation, _rectSourcePoint.Height), _controlColor);
                _widthAnimation -= 4;
            }
            spriteBatch.DrawString(Font, _text, _vectorDestText, _textColor);
            if (!Enabled)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectDestContainer, _rectSourceContainer, ControlResources.DisabledColor);
                if (Checked) spriteBatch.Draw(ControlResources.Atlas, _rectDestPoint, _rectSourcePoint, ControlResources.DisabledColor);
                spriteBatch.DrawString(Font, _text, _vectorDestText, ControlResources.DisabledColor);
            }
            if(WithGlass) spriteBatch.Draw(ControlResources.Atlas, _rectDestContainer, _rectSourceGlass, Color.White);
        }

        public override bool ClickInControl()
        {
            if (!Enabled) return false;
            if(Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
            {
                if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!_isPressed) _isPressed = true;
                }
                else if(_isPressed)
                {
                    if (Checked)
                    {
                        Checked = false;
                        _widthAnimation = _rectSourcePoint.Width;
                    }
                    else
                    {
                        Checked = true;
                        _widthAnimation = 0;
                    }
                    _isPressed = false;
                    if(_action != null) _action.Invoke();
                }
                return true;
            }
            else return false;
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _rectDestPoint.Offset(x,y);
            _rectDestContainer.Offset(x, y);
            _vectorDestText.X += x;
            _vectorDestText.Y += y;
        }
    }
}
