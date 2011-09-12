using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Towers.Graphics;
using Towers.Cameras;
using Towers.GUI;

namespace Towers
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TowerRenderer towerRenderer;
        ICamera camera;

        Color backgroundColor = Color.Green;
        
        int numInitialTowers = 10000 * 20 ;
        int minPos = -2500 * 2;
        int maxPos = 2500 * 2;

        int minSize = 10;
        int maxSize = 70;

        Random rand = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            towerRenderer = new TowerRenderer(this);
            Components.Add(towerRenderer);

            camera = new QuakeCamera(this);
            Components.Add(camera);

            var fpsCounter = new FrameRateCounter(this);
            Components.Add(fpsCounter);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AddInstances(numInitialTowers);            
        }

        TowerInstance CreateRandomInstance()
        {
            Vector3 pos = new Vector3(rand.Next(minPos, maxPos),
                    rand.Next(minPos, maxPos), rand.Next(minPos, maxPos));

            Matrix transform = Matrix.CreateTranslation(pos);
            transform = Matrix.Transpose(transform);

            Vector3 size = new Vector3(rand.Next(minSize, maxSize),
                rand.Next(minSize, maxSize), rand.Next(minSize, maxSize));

            Color color = new Color(rand.Next(255), rand.Next(255), rand.Next(255));

            return new TowerInstance(transform, size, color);
        }

        void AddInstances(int n)
        {
            List<TowerInstance> instances = new List<TowerInstance>();

            for (int i = 0; i < n; i++)
            {
                instances.Add(CreateRandomInstance());
            }

            towerRenderer.AddInstances(instances);
        }

        void RemoveInstances(int n)
        {
            towerRenderer.RemoveInstances(n);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            const int f = 20;

#if WINDOWS
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.LeftAlt) && keyboard.IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            if (keyboard.IsKeyDown(Keys.OemPlus))
                AddInstances(f);
            else if (keyboard.IsKeyDown(Keys.OemMinus))
                RemoveInstances(f);

#endif
#if XBOX || true
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

            if (gamepad.IsButtonDown(Buttons.RightTrigger))
                AddInstances(f);
            else if (gamepad.IsButtonDown(Buttons.LeftTrigger))
                RemoveInstances(f);
#endif

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
