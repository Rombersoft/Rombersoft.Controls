using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rombersoft.AssetsLoader;
using Rombersoft.Controls;
using Executable.Scenes;
                 
namespace Executable.Desktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MyGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            this.Window.IsBorderless = true;
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
            Window.TextInput += Control.Window_TextInput;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.Window.Position = new Point(0, 0);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetsLoader.Instance.Init(GraphicsDevice, Content, 5);
            ControlResources.Atlas = AssetsLoader.Instance.Atlas;
            ControlResources.Brush = AssetsLoader.Instance.Brush;
            ControlResources.AtlasRegions = AssetsLoader.Instance.AtlasRegions;
            ControlResources.Fonts = AssetsLoader.Instance.Fonts;
            ControlResources.DefaultFont = ControlResources.Fonts["Roboto16"];
            ControlResources.DisabledColor = Color.FromNonPremultiplied(0, 0, 0, 120);
            ControlResources.DefaultColor = Color.Black;
            GraphicInstance.SpriteBatch = _spriteBatch;
            GraphicInstance.Window = Window.ClientBounds;
            GraphicInstance.Device = GraphicsDevice;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update(new float[2] { 1, 1 });
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            SceneManager.Instance.Update(gameTime);
            TextBox.UpdateKeyboard();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin((SpriteSortMode)0,
                null,
                null,
                null,
                null,
                null,
                Matrix.CreateScale(1, 1, 1));
            SceneManager.Instance.Draw(gameTime);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
