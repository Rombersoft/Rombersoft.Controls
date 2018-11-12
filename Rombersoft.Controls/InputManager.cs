using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Rombersoft.Controls
{
    public class InputManager
    {
        private KeyboardState keyState;
#if TOUCH
        private MyMouseState[] _mouseState;
        private MyMouseState[] _mouseOldState;
#else
        private MyMouseState _mouseState;
        private MyMouseState _mouseOldState;
#endif
        private ButtonState _mouseLeftButtonState;
        private Point _mousePosition;
        private bool _mousePressed;
        public event Action<SlideDirection> OnSlide;
        private List<MyMouseState> _listMouseStates;
        private int _x, _y;
        private static InputManager _instance;

        public static InputManager Instance
        {
            get
            {
                if (_instance == null) _instance = new InputManager();
                return _instance;
            }
        }
        private InputManager()
        {
            _mouseLeftButtonState = ButtonState.Released;
            _listMouseStates = new List<MyMouseState>();
        }
#if TOUCH
        public MyMouseState[] MouseState
        {
            get { return _mouseState; }
        }
#else
        public MyMouseState MouseState
        {
            get { return _mouseState; }
        }

#endif
        public void Update(float[] scale)
        {
            _mouseOldState = _mouseState;
            keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            _mouseState = new MyMouseState((int)(mouseState.X / scale[0]), (int)((mouseState.Y) / scale[1]), mouseState.LeftButton, mouseState.RightButton, mouseState.MiddleButton,
                                           mouseState.ScrollWheelValue);
            _mousePosition = new Point(_mouseState.X, _mouseState.Y);
        }

        public bool IsKeyDown(Keys key)
        {
            return keyState.IsKeyDown(key);
        }

        public bool IsMove()
        {
            return (_mouseState.X != _mouseOldState.X || _mouseState.Y != _mouseOldState.Y);
        }

        public bool IsMouseLeftClick()
        {
            return _mouseOldState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMouseRightDown()
        {
            return _mouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMouseRightClick()
        {
            return IsMouseRightDown() && _mouseOldState.RightButton == ButtonState.Released;
        }

        public bool IsMouseMiddleDown()
        {
            return _mouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsMouseMiddleClick()
        {
            return IsMouseMiddleDown() && _mouseOldState.MiddleButton == ButtonState.Released;
        }

        public bool IsMouseWheelUp()
        {
            return _mouseState.ScrollWheelValue > _mouseOldState.ScrollWheelValue;
        }

        public bool IsMouseWheelDown()
        {
            return _mouseState.ScrollWheelValue < _mouseOldState.ScrollWheelValue;
        }

        public Point GetMousePositionToVector2()
        {
            return _mousePosition;
        }

        public void VerifySlide()
        {
            if(GraphicInstance.Window.Width == 1366)
            {
                if (_mouseState.LeftButton == ButtonState.Pressed)
                {
                    if(!_mousePressed) _mousePressed = true;
                    _listMouseStates.Add(_mouseState);
                }
                else if(_mousePressed)
                {
                    _mousePressed = false;
                    if (_listMouseStates.Count > 10)
                    {
                        if ((Math.Abs(_listMouseStates[0].X - _listMouseStates[_listMouseStates.Count - 1].X) > 60) &&
                            ((Math.Abs(_listMouseStates[0].X - _listMouseStates[_listMouseStates.Count - 1].X)) > 
                                (Math.Abs(_listMouseStates[0].Y - _listMouseStates[_listMouseStates.Count - 1].Y))))
                        {
                            if(OnSlide != null)
                            {
                                if (_listMouseStates[0].X > _listMouseStates[_listMouseStates.Count - 1].X) OnSlide(SlideDirection.Up);
                                else OnSlide(SlideDirection.Down);
                            }
                        }
                    }
                    _listMouseStates.Clear();
                }
            }
            else
            {
                if (_mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!_mousePressed) _mousePressed = true;
                    if (_listMouseStates.Count == 0) _listMouseStates.Add(_mouseState);
                    else if (Math.Sqrt(Math.Pow(_listMouseStates[0].X - _mouseState.X, 2) + Math.Pow(_listMouseStates[0].Y - _mouseState.Y, 2)) > 10) _listMouseStates.Add(_mouseState);
                    if (_listMouseStates.Count > 10 && OnSlide != null)
                    {
                        int dx = _listMouseStates[_listMouseStates.Count - 1].X - _listMouseStates[0].X;
                        int dy = _listMouseStates[_listMouseStates.Count - 1].Y - _listMouseStates[0].Y;
                        if (Math.Abs(dx) > Math.Abs(dy)) //horizontal moving
                        {
                            if (_listMouseStates[_listMouseStates.Count - 1].X > _listMouseStates[0].X)
                            {
                                for (int i = 1; i < _listMouseStates.Count - 1; i++)
                                {
                                    if (_listMouseStates[i].X > _listMouseStates[_listMouseStates.Count - 1].X)
                                    {
                                        _listMouseStates.Clear();
                                        return;
                                    }
                                }
                                OnSlide(SlideDirection.Right);
                            }
                            else
                            {
                                for (int i = 1; i < _listMouseStates.Count - 1; i++)
                                {
                                    if (_listMouseStates[i].X < _listMouseStates[_listMouseStates.Count - 1].X)
                                    {
                                        _listMouseStates.Clear();
                                        return;
                                    }
                                }
                                OnSlide(SlideDirection.Left);
                            }
                        }
                        else  //vertical moving
                        {
                            if (_listMouseStates[_listMouseStates.Count - 1].Y > _listMouseStates[0].Y)
                            {
                                for (int i = 1; i < _listMouseStates.Count - 1; i++)
                                {
                                    if (_listMouseStates[i].Y > _listMouseStates[_listMouseStates.Count - 1].Y)
                                    {
                                        _listMouseStates.Clear();
                                        return;
                                    }
                                }
                                OnSlide(SlideDirection.Down);
                            }
                            else
                            {
                                for (int i = 1; i < _listMouseStates.Count - 1; i++)
                                {
                                    if (_listMouseStates[i].Y < _listMouseStates[_listMouseStates.Count - 1].Y)
                                    {
                                        _listMouseStates.Clear();
                                        return;
                                    }
                                }
                                OnSlide(SlideDirection.Up);
                            }
                        }
                    }
                }
                else if (_mousePressed)
                {
                    _mousePressed = false;
                    _listMouseStates.Clear();
                }
            }
        }
    }

    public enum SlideDirection : byte
    {
        Left,
        Right,
        Down,
        Up
    }
}