using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class Picture : Control
    {
        private int _lang;
        private Rectangle _rectSource, _rectDestDisable;
        private Texture2D _texture;
        private Vector2 _vectorDestBegin, _vectorDestCenter, _vectorDifferent, _vectorDestUsed;
        private float _step, _curentIteration;
        private bool _isShown;
        private Direction _direction;
        public event Action OnMaximized;
        public event Action OnMinimized;

        public Picture(Texture2D texture, int iterationCount)
        {
            _lang = 0;
            _rectSource = new Rectangle(0, 0, texture.Width, texture.Height);
            _vectorDestCenter = new Vector2((GraphicInstance.Window.Width - texture.Width) / 2, (GraphicInstance.Window.Height - texture.Height) / 2);
            _texture = texture;
            _step = 1.57f / iterationCount;
            _rectDestDisable = new Rectangle(0, 0, GraphicInstance.Window.Width, GraphicInstance.Window.Height);
        }

        public void Show()
        {
            _direction = Direction.Grow;
            _vectorDestBegin = _vectorDestCenter;
            _isShown = true;
        }

        public void Hide()
        {
            _direction = Direction.Reduce;
        }

        public void Maximize(int x, int y)
        {
            _direction = Direction.Grow;
            _curentIteration = 0;
            _vectorDestBegin = new Vector2(x, y);
            _vectorDifferent = _vectorDestBegin - _vectorDestCenter;
            _isShown = true;
            _curentIteration = 0;
        }

        public void Minimize(int x, int y)
        {
            _direction = Direction.Reduce;
            _vectorDestBegin = new Vector2(x, y);
            _vectorDifferent = _vectorDestBegin - _vectorDestCenter;
        }

        public override bool ClickInControl()
        {
            return false;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isShown)
            {
                float scale = MathHelper.Lerp(0, 1f, (float)Math.Abs(Math.Sin(_curentIteration)));
                switch (_direction)
                {
                    case Direction.Grow:
                        if (_curentIteration < 1.57f)
                        {
                            _curentIteration += _step;
                            _vectorDestUsed = _vectorDestBegin - _vectorDifferent * scale;
                        }
                        else
                        {
                            _direction = Direction.None;
                            if (OnMaximized != null) OnMaximized();
                        }
                        break;
                    case Direction.Reduce:
                        _curentIteration -= _step;
                        _vectorDestUsed = _vectorDestBegin - _vectorDifferent * scale;
                        if (_curentIteration < 0)
                        {
                            _curentIteration = 0;
                            _direction = Direction.None;
                            _isShown = false;
                            if (OnMinimized != null) OnMinimized();
                        }
                        break;
                    case Direction.None:
                        break;
                }
                spriteBatch.Draw(ControlResources.Brush, _rectDestDisable, Color.White);
                spriteBatch.Draw(_texture, _vectorDestUsed, _rectSource, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        public override void Offset(int x, int y)
        {

        }

        public override void SetPosition(int x, int y)
        {

        }
    }

    public enum Direction
    {
        None, Grow, Reduce
    }
}