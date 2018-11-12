using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class ScrollBar : Control
    {
        private ScrollButton _buttonUp, _buttonDown;
        private ScrollThumb _buttonThumb;
        private Rectangle[] _rectsDestGuideMiddle;
        private int _stepForUpDown, _stepForUpDownMemory;
        public Color GuideColor;
        private Timer _timer;
        public event Action<int> OnValueChanged;

        public int Max
        {
            get { return _buttonThumb.MaxValue; }
            set { _buttonThumb.Value = value; }
        }

        public int Value
        {
            get { return _buttonThumb.Value; }
            set 
            {
                _buttonThumb.Value = value; 
                if (OnValueChanged != null) OnValueChanged(value);
            }
        }

        public Color ButtonsColor
        {
            get { return _buttonThumb.BackgroundColor; }
            set
            {
                _buttonUp.BackgroundColor = _buttonUp.PressedColor = _buttonDown.BackgroundColor = _buttonDown.PressedColor = _buttonThumb.BackgroundColor =
                    _buttonThumb.PressedColor = value;
            }
        }

        public ScrollBar(int x, int y, int width, int height)
        {
            GuideColor = Color.Black;
            Rect = new Rectangle(x, y, width, height);
            _rectsDestGuideMiddle = new Rectangle[2];
            _rectsDestGuideMiddle[0] = new Rectangle(Rect.X + width / 5, Rect.Y + width/2, width / 4, Rect.Height - width);
            _rectsDestGuideMiddle[1] = new Rectangle(Rect.X + width * 3 / 5, Rect.Y + width/2, width / 4, Rect.Height - width);
            _buttonUp = new ScrollButton(Rect.X, Rect.Y, width, (int)(width / 1.22f), "ScrollButton", "ListBoxUp", SpriteEffects.None);
            _buttonUp.OnPressed += Up_OnPressed;
            _buttonUp.OnReleased += Up_OnReleased;
            _buttonDown = new ScrollButton(Rect.X, Rect.Y + height - (int)(width / 1.22f), width, (int)(width / 1.22f), "ScrollButton", "ListBoxUp", SpriteEffects.FlipVertically);
            _buttonDown.OnPressed += Down_OnPressed;
            _buttonDown.OnReleased += Down_OnReleased;
            _buttonThumb = new ScrollThumb(Rect.X, Rect.Y + (int)(width / 1.65f), width, width * 7, height - width * 7 - (int)(width / 0.845f),"Thumb", "Thumb");
            _buttonThumb.OnMoved += Thumb_OnMoved;
            ButtonsColor = Color.White;
            _stepForUpDown = 0;
            _timer = new Timer(500);
            _timer.AutoReset = false;
            _timer.Elapsed += Timer_Elapsed;
        }

        void Up_OnPressed()
        {
            _stepForUpDownMemory = -1;
            _timer.Start();
            Up(_stepForUpDownMemory);
        }

        void Up_OnReleased()
        {
            _stepForUpDown = 0;
            _stepForUpDownMemory = 0;
            _timer.Stop();
        }

        void Down_OnPressed()
        {
            _stepForUpDownMemory = 1;
            _timer.Start();
            Down(_stepForUpDownMemory);
        }

        void Down_OnReleased()
        {
            _stepForUpDown = 0;
            _stepForUpDownMemory = 0;
            _timer.Stop();
        }

        void Thumb_OnMoved(int val)
        {
            if (OnValueChanged != null) OnValueChanged(val);
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _stepForUpDown = _stepForUpDownMemory;
            _stepForUpDownMemory = 0;
        }

        void Down(int step)
        {
            if (_buttonThumb.Value + step < _buttonThumb.MaxValue) Value += step;
            else
            {
                Value = _buttonThumb.MaxValue;
                _stepForUpDown = 0;
            }
        }

        void Up(int step)
        {
            if (_buttonThumb.Value + step > 0) Value += step;
            else
            {
                Value = 0;
                _stepForUpDown = 0;
            }
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Brush, _rectsDestGuideMiddle[0], GuideColor);
            spriteBatch.Draw(ControlResources.Brush, _rectsDestGuideMiddle[1], GuideColor);
            _buttonUp.Draw(spriteBatch);
            _buttonDown.Draw(spriteBatch);
            _buttonThumb.Draw(spriteBatch);
        }

        public override bool ClickInControl()
        {
            if (_stepForUpDown > 0)
            {
                Down(_stepForUpDown);
            }
            else if (_stepForUpDown < 0)
            {
                Up(_stepForUpDown);
            }
            if (_buttonThumb.ClickInControl())
            {
                return true;
            }
            if (_buttonUp.ClickInControl())
            {
                return true;
            }
            if (_buttonDown.ClickInControl())
            {
                return true;
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
            _buttonUp.Offset(x, y);
            _buttonDown.Offset(x, y);
            _buttonThumb.Offset(x, y);
            _rectsDestGuideMiddle[0].Offset(x, y);
            _rectsDestGuideMiddle[1].Offset(x, y);
        }

        private class ScrollButton : Control
        {
            private SpriteEffects _spriteEffects;
            public Color BackgroundColor, PressedColor;
            private Rectangle _mainView, _rectForPress;
            public event Action OnPressed, OnReleased;
            private bool _isPressed, _mouseInside;
            public Image Image;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Rombersoft.Controls.ScrollBar.ScrollButton"/> class.
            /// </summary>
            /// <param name="x">The x coordinate.</param>
            /// <param name="y">The y coordinate.</param>
            /// <param name="width">Width.</param>
            /// <param name="height">Height.</param>
            /// <param name="mainView">Main view.</param>
            /// <param name="name">Name.</param>
            /// <param name="effect">Effect.</param>
            public ScrollButton(int x, int y, int width, int height, string mainView, string name, SpriteEffects effect)
            {
                width = width == 0 ? ControlResources.AtlasRegions[mainView].Width : width;
                height = height == 0 ? ControlResources.AtlasRegions[mainView].Height : height;
                _mainView = ControlResources.AtlasRegions[mainView];
                BackgroundColor = Color.White;
                PressedColor = Color.FromNonPremultiplied(0, 0, 0, 150);
                Rect = new Rectangle(x, y, width, height);
                _rectForPress = new Rectangle(x, y, width * 2, height);
                Name = name;
                _spriteEffects = effect;
                this.Image = null;
            }

            public override void Draw()
            {
                Draw(GraphicInstance.SpriteBatch);
            }

            public override void Draw(SpriteBatch spriteBatch)
            {

                spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, BackgroundColor, 0, Vector2.Zero, _spriteEffects, 0);
                if (_isPressed) spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, PressedColor, 0, Vector2.Zero, _spriteEffects, 0);
                if (this.Image != null) Image.Draw(spriteBatch);
            }

            public override bool ClickInControl()
            {
                _mouseInside = false;
                if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_isPressed) return true;
                    if (_rectForPress.Contains(InputManager.Instance.GetMousePositionToVector2()))
                    {
                        if (!_isPressed)
                        {
                            _isPressed = true;
                            if (OnPressed != null) OnPressed();
                        }
                        _mouseInside = true;
                    }
                    else _isPressed = true;
                }
                else
                {
                    if (_isPressed)
                    {
                        _isPressed = false;
                        if (OnReleased != null) OnReleased();
                        _mouseInside = true;
                    }
                    else _isPressed = false;
                }
                return _mouseInside;
            }

            public override void Offset(int x, int y)
            {
                Rect.Offset(x, y);
                _rectForPress.Offset(x, y);
            }

            public override void SetPosition(int x, int y)
            {
                Rect.X = x;
                Rect.Y = y;
                _rectForPress.X = x;
                _rectForPress.Y = y;
            }
        }

        private class ScrollThumb : Control
        {
            public Color BackgroundColor, PressedColor;
            private Rectangle _mainView, _rectForPress;
            private int _mouseY, _minPos, _maxPos, _value;
            public event Action<int> OnMoved;
            private bool _isPressed, _mouseInside;
            public int MaxValue { get { return _maxPos - _minPos; } }
            public int Value
            {
                get { return _value; }
                set
                {
                    if (value > _maxPos) throw new InvalidOperationException("Value can not be more than MaxValue");
                    _value = value;
                    SetPositionThumb(_minPos + _value);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Rombersoft.Controls.ScrollBar.ScrollButton"/> class.
            /// </summary>
            /// <param name="x">The x coordinate.</param>
            /// <param name="y">The y coordinate.</param>
            /// <param name="width">Width.</param>
            /// <param name="height">Height.</param>
            /// <param name="mainView">Main view.</param>
            /// <param name="name">Name.</param>
            public ScrollThumb(int x, int y, int width, int height, int delta, string mainView, string name)
            {
                width = width == 0 ? ControlResources.AtlasRegions[mainView].Width : width;
                height = height == 0 ? ControlResources.AtlasRegions[mainView].Height : height;
                _mainView = ControlResources.AtlasRegions[mainView];
                BackgroundColor = Color.White;
                PressedColor = Color.FromNonPremultiplied(0, 0, 0, 50);
                Rect = new Rectangle(x, y, width, height);
                _rectForPress = new Rectangle(x, y, width * 2, height);
                _minPos = y;
                _maxPos = y + delta;
                Name = name;
                _value = 0;
            }

            public override void Draw()
            {
                Draw(GraphicInstance.SpriteBatch);
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                if (_isPressed) spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, PressedColor, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                else spriteBatch.Draw(ControlResources.Atlas, Rect, _mainView, BackgroundColor);
            }

            public override bool ClickInControl()
            {
                _mouseInside = false;
                if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_isPressed)
                    {
                        if (InputManager.Instance.MouseState.Y != _mouseY)
                        {
                            int delta = InputManager.Instance.MouseState.Y - _mouseY;
                            if (delta < 0 && Rect.Y != _minPos)
                            {
                                if (Rect.Y + delta >= _minPos) OffsetThumb(delta);
                                else SetPositionThumb(_minPos);
                                if (OnMoved != null) OnMoved(Rect.Y - _minPos);
                            }
                            else if (delta > 0 && Rect.Y != _maxPos)
                            {
                                if (Rect.Y + delta <= _maxPos) OffsetThumb(delta);
                                else SetPositionThumb(_maxPos);
                                if (OnMoved != null) OnMoved(Rect.Y - _minPos);
                            }
                            _mouseY = InputManager.Instance.MouseState.Y;
                        }
                        return _isPressed;
                    }
                    if (_rectForPress.Contains(InputManager.Instance.GetMousePositionToVector2()))
                    {
                        if (!_isPressed)
                        {
                            _isPressed = true;
                        }
                        _mouseInside = true;
                        _mouseY = InputManager.Instance.MouseState.Y;
                    }
                }
                else
                {
                    _isPressed = false;
                }
                return _mouseInside;
            }

            private void OffsetThumb(int y)
            {
                Rect.Offset(0, y);
                _rectForPress.Offset(0, y);
                _value = Rect.Y - _minPos;
            }

            public void SetPositionThumb(int y)
            {
                OffsetThumb(y - Rect.Y);
            }

            public override void Offset(int x, int y)
            {
                Rect.Offset(x, y);
                _rectForPress.Offset(x, y);
                _minPos += y;
                _maxPos += y;
            }

            public override void SetPosition(int x, int y)
            {
                Offset(x - Rect.X, y - Rect.Y);
            }
        }
    }
}