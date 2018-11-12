using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class ProgressBarLinear : Control
    {
        Rectangle _rectSourceBeginPB, _rectSourceEndPB, _rectSourceMiddlePB, _rectSourceThumbBegin, _rectSourceThumbEnd, _rectSourceThumbMiddle;
        Vector2 _vectorDestThumb;
        float _scalePipe, _scaleThumb;
        const float _thumbToPipeRelation = 0.884615384615f;
        int _maxStep, _currentLevel, _widthThumb, _heightThumb, _sideDifference;
        public Color PipeColor, ThumbColor;

        public ProgressBarLinear(int x, int y, int width, int height)
        {
            Rect = new Rectangle(x, y, width, height);
            _scalePipe = (float)height /52f;
            float heightThumbF = height * _thumbToPipeRelation;
            _heightThumb = (int)heightThumbF;
            if (heightThumbF - _heightThumb > 0 || _heightThumb % 2 == 1)
            {
                if (heightThumbF - _heightThumb > 0)
                {
                    if (_heightThumb % 2 == 1) _heightThumb--;
                    _scaleThumb = _heightThumb / 46f;
                }
            }
            _sideDifference = (height - _heightThumb) / 2;
            _vectorDestThumb = new Vector2(_sideDifference + x, _sideDifference + y);
            _rectSourceBeginPB = ControlResources.AtlasRegions["ProgressBarPipeBegin"];
            _rectSourceEndPB = ControlResources.AtlasRegions["ProgressBarPipeEnd"];
            _rectSourceMiddlePB = ControlResources.AtlasRegions["ProgressBarPipeMiddle"];
            _rectSourceThumbBegin = ControlResources.AtlasRegions["ProgressBarThumbBegin"];
            _rectSourceThumbEnd = ControlResources.AtlasRegions["ProgressBarThumbEnd"];
            _rectSourceThumbMiddle = ControlResources.AtlasRegions["ProgressBarThumbMiddle"];
            MaxStep = 100;
            PipeColor = Color.White;
            ThumbColor = Color.Blue;
            CurrentLevel = 0;
            _widthThumb = -1;
        }

        public int CurrentLevel
        {
            get { return _currentLevel; }
            set
            {
                _currentLevel = value;
                if (_maxStep < _currentLevel) _currentLevel = _maxStep;
                if (_currentLevel < 0) _currentLevel = 0;
                _widthThumb = (int)((Rect.Width - 2 * _sideDifference) * _currentLevel / MaxStep - 46 * _scaleThumb);
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

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(Rect.X + 26 * _scalePipe), Rect.Y, (int)(Rect.Width- 52 * _scalePipe), (int)(52 * _scalePipe)), 
                             _rectSourceMiddlePB, PipeColor);
            spriteBatch.Draw(ControlResources.Atlas, new Rectangle(Rect.X, Rect.Y, (int)(26 * _scalePipe), (int)(52 * _scalePipe)), _rectSourceBeginPB, PipeColor);
            spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(Rect.X + Rect.Width- 26 * _scalePipe), Rect.Y, (int)(26 * _scalePipe), (int)(52 * _scalePipe)), 
                             _rectSourceEndPB, PipeColor);
            if (_widthThumb >= 0)
            {
                spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(_vectorDestThumb.X + 23 * _scaleThumb), (int)_vectorDestThumb.Y, _widthThumb, _heightThumb),
                                 _rectSourceThumbMiddle, ThumbColor);
                spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(_vectorDestThumb.X), (int)(_vectorDestThumb.Y), (int)(23 * _scaleThumb), _heightThumb),
                                 _rectSourceThumbBegin, ThumbColor);
                spriteBatch.Draw(ControlResources.Atlas, new Rectangle((int)(_vectorDestThumb.X + 23 * _scaleThumb + _widthThumb), (int)(_vectorDestThumb.Y), 
                                                                       (int)(23 * _scaleThumb), _heightThumb), _rectSourceThumbEnd, ThumbColor);
            }
        }

        public override bool ClickInControl()
        {
            return true;
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            _vectorDestThumb.X += x;
            _vectorDestThumb.Y += y;
            Rect.Offset(x,y);
        }
    }

    public enum Design
    {
        Green,Blue
    }
}
