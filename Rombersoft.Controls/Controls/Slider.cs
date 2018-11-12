using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public class Slider : Control
    {
        Rectangle _rectSourceBeginPB, _rectSourceEndPB, _rectSourceMiddlePB, _rectSourceThumb, _rectDestThumb, _rectDestPipe;
        public Color PipeColor, ThumbColor;
        float _scalePipe;
        int _currentLevel, _maxStep, _lastX;
        bool _pressed;
        public Action<Slider, int> OnValueChanged;

        public Slider(int x, int y, int width)
        {
            float height = GraphicInstance.Window.Height < GraphicInstance.Window.Width ? GraphicInstance.Window.Height/40 : GraphicInstance.Window.Width/40;
            Rect = new Rectangle(x, y, width, (int)height*3);
            _rectDestPipe = new Rectangle(x + (int)height, y + (int)height, width - (int)height * 2, (int)height);
            _scalePipe = height / 52f;
            _rectSourceBeginPB = ControlResources.AtlasRegions["ProgressBarPipeBegin"];
            _rectSourceEndPB = ControlResources.AtlasRegions["ProgressBarPipeEnd"];
            _rectSourceMiddlePB = ControlResources.AtlasRegions["ProgressBarPipeMiddle"];
            _rectDestThumb = new Rectangle(x, y, (int)height * 3, (int)height * 3);
            _rectSourceThumb = ControlResources.AtlasRegions["Lever4"];
            PipeColor = ThumbColor = Color.White;
            _maxStep = 10;
        }

        public int Value
        {
            get { return _currentLevel; }
            set
            {
                if (_currentLevel != value)
                {
                    _currentLevel = value;
                    if (_maxStep < _currentLevel) _currentLevel = _maxStep;
                    if (_currentLevel < 0) _currentLevel = 0;
                    _rectDestThumb.X = Rect.X + (int)((_rectDestPipe.Width - _rectDestPipe.Height) / (float)_maxStep * _currentLevel);
                    if (OnValueChanged != null) OnValueChanged(this, _currentLevel);
                }
            }
        }

        public int MaxStep
        {
            get { return _maxStep; }
            set
            {
                _maxStep = value == 0 ? 1 : value;
                if (_maxStep < _currentLevel) throw new Exception("Текущая позиция за пределами границ контрола");
            }
        }

        public override bool ClickInControl()
        {
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (_pressed)
                {
                    if (InputManager.Instance.MouseState.Y != _lastX)
                    {
                        int delta = InputManager.Instance.MouseState.X - _lastX;
                        _lastX = InputManager.Instance.MouseState.X;
                        if (delta < 0 && _rectDestThumb.X != Rect.X)
                        {
                            if (_rectDestThumb.X + delta >= Rect.X) OffsetThumb(delta);
                            else SetPositionThumb(Rect.X);
                        }
                        else if (delta > 0 && _rectDestThumb.X != Rect.X + Rect.Width - Rect.Height)
                        {
                            if (_rectDestThumb.X + delta <= Rect.X + Rect.Width - Rect.Height) OffsetThumb(delta);
                            else SetPositionThumb(Rect.X + Rect.Width - Rect.Height);
                        }
                    }
                    return _pressed;
                }
                if (_rectDestThumb.Contains(InputManager.Instance.GetMousePositionToVector2()))
                {
                    _lastX = InputManager.Instance.MouseState.X;
                    _pressed = true;
                }
            }
            else
            {
                if (_pressed) CorrectThumbPosition();
                _pressed = false;
            }
            return false;
        }

        void CorrectThumbPosition()
        {
            float pos = (float)(_rectDestThumb.X - Rect.X) / ((float)(_rectDestPipe.Width - _rectDestPipe.Height) / (float)_maxStep);
            int val = (int)pos;
            if (pos - val > 0.5f)
            {
                val++;
            }
            else if(_currentLevel == val) _rectDestThumb.X = Rect.X + (int)((_rectDestPipe.Width - _rectDestPipe.Height) / (float)_maxStep * _currentLevel);
            Value = val;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(_rectDestPipe.X + 26 * _scalePipe), _rectDestPipe.Y, (int)(_rectDestPipe.Width - 52 * _scalePipe),
                                                                   (int)(52 * _scalePipe)),
                             _rectSourceMiddlePB, PipeColor);
            spriteBatch.Draw(ControlResources.Atlas, new Rectangle(_rectDestPipe.X, _rectDestPipe.Y, (int)(26 * _scalePipe), (int)(52 * _scalePipe)), _rectSourceBeginPB, PipeColor);
            spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(_rectDestPipe.X + _rectDestPipe.Width - 26 * _scalePipe), _rectDestPipe.Y, (int)(26 * _scalePipe),
                                                                   (int)(52 * _scalePipe)), _rectSourceEndPB, PipeColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectDestThumb, _rectSourceThumb, ThumbColor);
        }

        void OffsetThumb(int x)
        {
            _rectDestThumb.Offset(x, 0);
            int val = (_rectDestThumb.X - Rect.X) / ((_rectDestPipe.Width - _rectDestPipe.Height) / _maxStep);
            if (val != _currentLevel)
            {
                _currentLevel = val;
                if (OnValueChanged != null) OnValueChanged(this, _currentLevel);
            }
        }

        void SetPositionThumb(int x)
        {
            _rectDestThumb.X = x;

        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x,y);
            _rectDestPipe.Offset(x, y);
            _rectDestThumb.Offset(x, y);
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }
    }
}