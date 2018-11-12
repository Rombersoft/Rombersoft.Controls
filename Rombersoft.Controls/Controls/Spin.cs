using System;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class Spin : Control
    {
        Display _displayCounter;
        Button _buttonAdd, _buttonMinus;
        string _unit;
        int _minValue, _maxValue;
        Rectangle _rectDestBackground;
        public Color TextColor, DisplayBgColor;
        Color _buttonTextColor, _buttonBgColor;

        public event Action<Spin, int> ValueOnChanged;

        public Spin(int x, int y, int width, int height, int strLength)
        {
            Rect = new Rectangle(x, y, width, height);
            _rectDestBackground = new Rectangle(x + height / 2, y, width - height, height);
            _displayCounter = new Display(x, y, width, height, strLength);
            _displayCounter.WithShadow = true;

            TextColor = _buttonTextColor = ControlResources.DefaultColor;
            _buttonBgColor = Color.White;
            DisplayBgColor = Color.White;
            _buttonAdd = new Button(x, y, height/2, height, "ButtonSimpleBegin", "+");
            _buttonAdd.OnClicked += () => Button_Click("plus");
            _buttonAdd.PropertyText(Align.Center, Font, _buttonTextColor);
            _buttonAdd.Text("+");

            _buttonMinus = new Button(x+width-height / 2, y, height / 2, height, "ButtonSimpleEnd", "-");
            _buttonMinus.OnClicked += () => Button_Click("minus");
            _buttonMinus.PropertyText(Align.Center, Font, _buttonTextColor);
            _buttonMinus.Text("-");

            _unit = string.Empty;
            SetRange(0, 100);
            _displayCounter.Text = _minValue.ToString();

        }

        public void SetButtonColor(Color backColor, Color textColor)
        {
            _buttonTextColor = textColor;
            _buttonBgColor = backColor;
            _buttonMinus.PropertyText(Align.Center, Font, _buttonTextColor);
            _buttonAdd.PropertyText(Align.Center, Font, _buttonTextColor);
            _buttonMinus.BackColor = backColor;
            _buttonAdd.BackColor = backColor;
        }
        public void SetFont(SpriteFont font)
        {
            Font = font;
            _displayCounter.SetFont(Font);
            _buttonMinus.PropertyText(Align.Center, Font, _buttonTextColor);
            _buttonAdd.PropertyText(Align.Center, Font, _buttonTextColor);
        }

        public override bool ClickInControl()
        {
            _buttonAdd.ClickInControl();
            _buttonMinus.ClickInControl();
            return true;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _buttonAdd.Draw(spriteBatch);
            _buttonMinus.Draw(spriteBatch);
            spriteBatch.Draw(ControlResources.Brush, _rectDestBackground, DisplayBgColor);
            _displayCounter.Draw(spriteBatch);
        }

        public override void Offset(int x, int y)
        {
            _buttonAdd.Offset(x,y);
            _buttonMinus.Offset(x,y);
            Rect.Offset(x, y);
            _rectDestBackground.Offset(x,y);
            _displayCounter.Offset(x,y);
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        } 

        public bool WithShadows
        {
            get { return _displayCounter.WithShadow; }
            set { _displayCounter.WithShadow = value; }
        }

        public void AddUnits(string text)
        {
            _unit = text;
        }

        public string Value { get { return _displayCounter.Text; } }

        public void SetRange(int minValue, int maxValue)
        {
            if (minValue > maxValue) throw new ArgumentException("minValue can not be more than maxValue");
            _minValue = minValue;
            _maxValue = maxValue;
        }

        private void Button_Click(string command)
        {
            int count = 0;
            switch (command)
            {
                case "plus":
                    count = Convert.ToInt32(_displayCounter.Text.Split(' ')[0]) + 1;
                    break;
                case "minus":
                    count = Convert.ToInt32(_displayCounter.Text.Split(' ')[0]) - 1;
                    break;
            }
            if (count >= _minValue && count <= _maxValue)
            {
                Calculate(count);
                if (ValueOnChanged != null) ValueOnChanged(this, count);
            }
        }

        private void Calculate(int count)
        {
            if (!string.IsNullOrEmpty(_unit)) _displayCounter.Text = count.ToString() + " " + _unit;
            else _displayCounter.Text = count.ToString();
        }
    }
}