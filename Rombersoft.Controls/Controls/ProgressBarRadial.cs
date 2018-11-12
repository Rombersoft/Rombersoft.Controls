using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class ProgressBarRadial:Control
    {
        private int _currentLevel;
        private Rectangle _rectSourceOneItem, _rectSourcePreloader;
        private Vector2 _vectorDestRotation, _vectorDestPreloader, _vectorDestPercent;
        private float _step;
        private int _maxStep;
        string _percent;
        public Color BackColor, ItemColor, TextColor;

        public bool Continous { get; set; }

        public string MeasuringUnit { get; set; }

        /// <summary>
        /// Creates radial progress bar with size 144x144 px/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public ProgressBarRadial(int x, int y)
        {
            Rect = new Rectangle(x, y, 144, 144);
            _rectSourcePreloader = ControlResources.AtlasRegions["ProgressBarRadialContainer"];
            _rectSourceOneItem = ControlResources.AtlasRegions["ProgressBarRadialItem"];
            _vectorDestRotation = new Vector2(13, 81);
            _vectorDestPreloader = new Vector2(x, y);
            _step = 0.26179938779914943653855361527329f;
            MaxStep = 100;
            _currentLevel = 0;
            Continous = false;
            BackColor = ItemColor = TextColor = Color.White;
            MeasuringUnit = "%";
            _percent = string.Empty;
        }

        public int CurrentLevel
        {
            get { return _currentLevel; }
            set
            {
                _currentLevel = value * 24 / _maxStep;
                _percent = (value * 100 / _maxStep) + MeasuringUnit;
                SetPosText();
            }
        }

        private void SetPosText()
        {
            int width = (int)Font.MeasureString(_percent).X;
            int height = (int)Font.MeasureString(_percent).Y;
            _vectorDestPercent = new Vector2((144 - width) / 2 + Rect.X, (144 - height)/2 + Rect.Y);
        }

        public int MaxStep
        {
            get { return _maxStep; }
            set
            {
                _maxStep = value;
                if (_maxStep < _currentLevel) throw new Exception("Текущая позиция за пределами границ контрола");
            }
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            SetPosText();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(ControlResources.Atlas, _vectorDestPreloader, _rectSourcePreloader, BackColor);
            if (Continous)
            {
                spriteBatch.Draw(ControlResources.Atlas, new Rectangle(Rect.X + 72, Rect.Y + 72, 25, 39), _rectSourceOneItem, ItemColor, _step * (_currentLevel / 4),
                            _vectorDestRotation, SpriteEffects.None, 0);
                if (96 < _currentLevel++) _currentLevel = 0;
            }
            else
            {
                for (int i = 0; i < _currentLevel; i++)
                {
                    spriteBatch.Draw(ControlResources.Atlas, new Rectangle(Rect.X + 72, Rect.Y + 72, 25, 39), _rectSourceOneItem, ItemColor, _step * i,
                        _vectorDestRotation, SpriteEffects.None, 0);
                }
                if (!string.IsNullOrEmpty(_percent)) spriteBatch.DrawString(Font, _percent, _vectorDestPercent, TextColor);
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
            Rect.Offset(x, y);
            _vectorDestPercent.X += x;
            _vectorDestPercent.Y += y;
            _vectorDestPreloader.X += x;
            _vectorDestPreloader.Y += y;
        }
    }
}