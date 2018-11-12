using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class TextureCreator : IDisposable
    {
        private ContentManager _manager;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _map;
        /// <summary>
        /// Initializes a new instance of the <see cref="XNA.Controls.TextureCreator"/> class.
        /// </summary>
        /// <param name="contentPath">path to .xnb files relatively Content folder</param>
        public TextureCreator(string contentPath)
        {
            _manager = new ContentManager(GraphicInstance.ServiceProvider, "Content/" + contentPath);
            _spriteBatch = new SpriteBatch(GraphicInstance.Device);
            _spriteBatch.Name = "TextureCreator";
            _map = new RenderTarget2D(GraphicInstance.Device, 1280, 1024);
            GraphicInstance.Device.Clear(Color.Transparent);
            _spriteBatch.Begin();
        }

        public TextureCreator(int width, int height, Color color)
        {
            _manager = new ContentManager(GraphicInstance.ServiceProvider, "Content/Additional");
            _spriteBatch = new SpriteBatch(GraphicInstance.Device);
            _spriteBatch.Name = "TextureCreator";
            _map = new RenderTarget2D(GraphicInstance.Device, width, height);
            GraphicInstance.Device.SetRenderTarget(_map);
            GraphicInstance.Device.Clear(color);
            _spriteBatch.Begin();
        }
        /// <summary>
        /// Малює текстуру в масштабі 1:1, яка є частиною атласу.
        /// </summary>
        /// <param name="rectSouce">Область атласу</param>
        /// <param name="textureAtlas">Атлас з якого буде братися область</param>
        /// <param name="destPosition">Позиція на полотні намальваної текстури</param>
        /// <param name="effect">Ефекти відзеркалювання текстури</param>
        public void Draw(Texture2D textureAtlas, Vector2 destPosition, Rectangle rectSouce, SpriteEffects effect)
        {
            _spriteBatch.Draw(textureAtlas, destPosition, rectSouce, Color.White, 0, Vector2.Zero, 1f, effect, 0);
        }
        /// <summary>
        /// Малює текстуру в заданому масштабі, яка є частиною атласу.
        /// </summary>
        /// <param name="rectSouce">Область атласу</param>
        /// <param name="textureAtlas">Атлас з якого буде братися область</param>
        /// <param name="destRectangle">Позиція і розмір на полотні намальваної текстури</param>
        /// <param name="effect">Ефекти відзеркалювання текстури</param>
        public void Draw(Texture2D textureAtlas, Rectangle destRectangle, Rectangle rectSouce, SpriteEffects effect)
        {
            _spriteBatch.Draw(textureAtlas, destRectangle, rectSouce, Color.White, 0, Vector2.Zero, effect, 0);
        }
        /// <summary>
        /// Малює текстуру в масштабі 1:1
        /// </summary>
        /// <param name="name">Назва файлу .xnb, з якої треба отримати текстуру. Файл повинен обов’язково знаходитись в папці Content/Additional/</param>
        /// <param name="destPosition">Позиція на полотні намальваної текстури</param>
        /// <param name="effect">Ефекти відзеркалювання текстури</param>
        public void Draw(string name, Vector2 destPosition, SpriteEffects effect)
        {
            Texture2D texture = _manager.Load<Texture2D>(name);
            Rectangle destRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            _spriteBatch.Draw(texture, destPosition, destRectangle, Color.White, 0, Vector2.Zero, 1f, effect, 0);
        }
        /// <summary>
        /// Малює текстуру в заданому масштабі
        /// </summary>
        /// <param name="name">Назва файлу .xnb, з якої треба отримати текстуру. Файл повинен обов’язково знаходитись в папці Content/Additional/</param>
        /// <param name="destPosition">Позиція і розмір на полотні намальваної текстури</param>
        /// <param name="effect">Ефекти відзеркалювання текстури</param>
        public void Draw(string name, Rectangle destPosition, SpriteEffects effect)
        {
            Texture2D texture = _manager.Load<Texture2D>(name);
            Rectangle destRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            _spriteBatch.Draw(texture, destPosition, destRectangle, Color.White, 0, Vector2.Zero, effect, 0);
        }

        public void DrawControl(Control control)
        {
            control.Draw(_spriteBatch);
        }
        /// <summary>
        /// Повертає полотно з намальованими текстурами
        /// </summary>
        /// <returns>Повертає текстуру</returns>
        public Texture2D GetCanvas()
        {
            _spriteBatch.End();
            GraphicInstance.Device.SetRenderTarget(null);
            Texture2D texture = new Texture2D(GraphicInstance.Device, _map.Bounds.Width, _map.Bounds.Height);
            Color[] color = new Color[_map.Bounds.Width * _map.Bounds.Height];
            _map.GetData<Color>(color);
            texture.SetData<Color>(color);
            return texture;
        }

        public SpriteBatch GetSpriteBatch()
        {
            return _spriteBatch;
        }

        public static Texture2D ClipTexture(Texture2D textureSource, Rectangle destRect)
        {
            if (textureSource.Width < destRect.Width || textureSource.Height < destRect.Height)
                throw new Exception("Размер исходной картинки не может быть меньше размера целевой картинки");
            Texture2D texture = new Texture2D(GraphicInstance.Device, destRect.Width, destRect.Height);
            Color[] color = new Color[destRect.Width * destRect.Height];
            textureSource.GetData<Color>(0, destRect, color, 0, color.Length);
            texture.SetData<Color>(color);
            return texture;
        }

        /// <summary>
        /// Loads the texture from .xnb file.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="path">Path.</param>
        public Texture2D LoadTexture(string name)
        {
            try
            {
                return _manager.Load<Texture2D>(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                _manager.Dispose();
                _map.Dispose();
                _spriteBatch.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

//SpriteBatch sb = new SpriteBatch(GraphicInstance.Device);
//RenderTarget2D map = new RenderTarget2D(GraphicInstance.Device, 1920, 1080);
//GraphicInstance.Device.SetRenderTarget(map);
//GraphicInstance.Device.Clear(Color.Black);
//sb.Begin();
//sb.Draw(atlas, new Rectangle(200,200,1366,768), Color.White);
//sb.End();
//Commons.GraphicDevice.SetRenderTarget(null);
//_texture = (Texture2D)map;