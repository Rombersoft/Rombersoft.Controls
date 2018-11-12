using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Rombersoft.Controls
{
    public class Preloader : Control
    {
        private Rectangle _rectDestPreloader, _rectDestPreloaderCentered, _rectDestPreloaderSided;
        private Rectangle[] _rectSourcePreloader, _rectSourcePanel, _rectDestPanel;
        private Label _label;
        public Color BackColor;
        private int _iteration, _labelStartPositionX, _labelStartPositionY, _labelSpace;
        private bool _shown, _withoutLabel;
        static Preloader _instance = new Preloader();
        public static Preloader Instance
        {
            get
            {
                return _instance;
            }
        }

        Preloader()
        {
            //size = 357x100
            BackColor = Color.White;
            if (GraphicInstance.Window.Height == 0 || GraphicInstance.Window.Width == 0) throw new Exception("Invalid a Window aspect ratio");
            int minWindowSize = GraphicInstance.Window.Height < GraphicInstance.Window.Width ? GraphicInstance.Window.Height : GraphicInstance.Window.Width;
            int width;
            if (minWindowSize > 1200)
            {
                width = (int)(minWindowSize * 0.3f);
                _rectDestPreloaderSided = new Rectangle(0, 0, (int)(width/5.25f), (int)(width / 5.328f));
            }
            else
            {
                width = 357;
                _rectDestPreloaderSided = new Rectangle(0, 0, 68, 67);
            }
            int height = (int)(width * 0.28f);
            int x = (GraphicInstance.Window.Width - width) / 2;
            int y = (GraphicInstance.Window.Height - height) / 2;
            Rect = new Rectangle(x, y, width, height);
            _rectDestPreloaderSided.X = x + (height - _rectDestPreloaderSided.Height) / 2;
            _rectDestPreloaderSided.Y = y + (height - _rectDestPreloaderSided.Height) / 2;
            _rectDestPreloaderCentered = new Rectangle(x + (width - _rectDestPreloaderSided.Width)/2, y + (height - _rectDestPreloaderSided.Height)/2,
                                                       _rectDestPreloaderSided.Width, _rectDestPreloaderSided.Height);
            _rectSourcePreloader = new Rectangle[8];
            _rectSourcePanel = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            _rectDestPanel = SpliteRect(Rect, 12);
            for (byte i = 0; i < _rectSourcePreloader.Length; i++)
                _rectSourcePreloader[i] = ControlResources.AtlasRegions["Preloader" + (i+1)];
            _labelStartPositionX = _rectDestPreloaderSided.X + _rectDestPreloaderSided.Width + (_rectDestPreloaderSided.X - x);
            _labelSpace = width + x - _rectDestPreloaderSided.X + x - _labelStartPositionX;
            _label = new Label(0, 0);
            _label.Color = ControlResources.DefaultColor;
            _label.WithShadow = true;
            _label.Text = string.Empty;
            _labelStartPositionY = y + ((int)(height - Font.MeasureString("A").Y) / 2);
            _iteration = 0;
        }

        /// <summary>
        /// Запускає прелоадер з текстом для 3-х мов
        /// </summary>
        /// <param name="text">text</param>
        public void Show(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Show();
                return;
            }
            _label.Text = text;
            AlignText();
            _rectDestPreloader = _rectDestPreloaderSided;
            _withoutLabel = false;
            _shown = true;
        }

        public void Show()
        {
            _rectDestPreloader = _rectDestPreloaderCentered;
            _withoutLabel = true;
            _shown = true;
        }

        public Color TextColor
        {
            get { return _label.Color; }
            set { _label.Color = value; }
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            _label.SetFont(font);
            _labelStartPositionY = Rect.Y + ((int)(Rect.Height - Font.MeasureString("A").Y) / 2);
            AlignText();
        }

        void AlignText()
        {
            _label.SetPosition(_labelStartPositionX + (_labelSpace - (int)Font.MeasureString(_label.Text).X)/2, _labelStartPositionY);
        }
        /// <summary>
        /// Закриває прелоадер
        /// </summary>
        public void Close()
        {
            _shown = false;
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_shown) return;
            if (!_withoutLabel)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[0], _rectSourcePanel[0], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[1], _rectSourcePanel[1], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[2], _rectSourcePanel[2], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[3], _rectSourcePanel[3], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[4], _rectSourcePanel[4], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[5], _rectSourcePanel[5], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[6], _rectSourcePanel[6], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[7], _rectSourcePanel[7], BackColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectDestPanel[8], _rectSourcePanel[8], BackColor);
                _label.Draw(spriteBatch);
            }
            spriteBatch.Draw(ControlResources.Atlas, _rectDestPreloader, _rectSourcePreloader[(++_iteration/3)%8], Color.White);
        }

        public override bool ClickInControl()
        {
            return false;
        }

        /// <summary>
        /// Устанавливает позицию контрола
        /// </summary>
        /// <param nameRU="x">новая позиция Х</param>
        /// <param nameRU="y">новая позиция У</param>
        public override void SetPosition(int x, int y)
        {
            
        }
        /// <summary>
        /// Смещает позицию котрола на указаное количество пикселей
        /// </summary>
        /// <param nameRU="x">шаг смешения по Х</param>
        /// <param nameRU="y">шаг смещения по Y</param>
        /// <returns>новую позицию контрола</returns>
        public override void Offset(int x, int y)
        {
            
        }
    }
}