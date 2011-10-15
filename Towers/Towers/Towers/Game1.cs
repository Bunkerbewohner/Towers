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
using JigLibX.Physics;
using JigLibX.Collision;
using Towers.World.Objects;
using Towers.World.Actors;
using System.Globalization;

namespace Towers
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        TowerRenderer towerRenderer;
        ICamera camera;

        PhysicsSystem physics;
        DebugDrawer ddrawer;

        Color backgroundColor = Color.Green;
        
        const int numInitialTowers = 2000;
        int minPos = -500 * 1;
        int maxPos = 500 * 1;

        float numInstances = numInitialTowers;
        int numPolygons = 0;

        int minSize = 4;
        int maxSize = 20;

        Random rand = new Random();

        List<Tower> towers = new List<Tower>();
        Player player;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;

            physics = new PhysicsSystem();
            physics.CollisionSystem = new CollisionSystemSAP();
            physics.EnableFreezing = true;
            physics.SolverType = PhysicsSystem.Solver.Fast;
            physics.CollisionSystem.UseSweepTests = true;
            physics.NumCollisionIterations = 10;
            physics.NumContactIterations = 10;
            physics.NumPenetrationRelaxtionTimesteps = 15;

            physics.Gravity = new Vector3(0, -50, 0);
            physics.Gravity = Vector3.Zero;
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

            ddrawer = new DebugDrawer(this);
            Components.Add(ddrawer);

            var fpsCounter = new FrameRateCounter(this);
            Components.Add(fpsCounter);

            Components.Add(new DebugOutput(this));

            player = new Player(new Vector3(0, maxPos, 0), camera as QuakeCamera);
            physics.AddController(player.PhysicsController);
            player.PhysicsController.EnableController();            

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

            Tower ground = new Tower(new Vector3(0, minPos, 0), new Vector3(maxPos - minPos, 5, maxPos - minPos),
                Matrix.Identity, Color.Gray);
            towerRenderer.AddInstance(ground.GraphicsInstance);

            spriteFont = Content.Load<SpriteFont>("DefaultFont");

            AddInstances(numInitialTowers);            
        }

        Tower CreateRandomTower()
        {
            Vector3 pos = new Vector3(rand.Next(minPos, maxPos),
                    rand.Next(minPos, maxPos), rand.Next(minPos, maxPos));            

            Matrix transform = Matrix.Identity;            

            Vector3 size = new Vector3(rand.Next(minSize, maxSize),
                rand.Next(minSize, maxSize), rand.Next(minSize, maxSize));

            Color color = new Color(rand.Next(255), rand.Next(255), rand.Next(255));

            return new Tower(pos, size, transform, color);
        }

        void AddInstances(float n)
        {                        
            List<TowerInstance> instances = new List<TowerInstance>();

            for (int i = 0; i < n; i++)
            {
                Tower tower = CreateRandomTower();
                instances.Add(tower.GraphicsInstance);
                towers.Add(tower);
            }

            towerRenderer.AddInstances(instances);
        }

        void RemoveInstances(int n)
        {
            int num = Math.Min(n, towers.Count - 1);
            int index = towers.Count - num;

            if (num > 0)
                towers.RemoveRange(index, num);
            
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
            // Update physics engine
            float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            if (timeStep < 1.0f / 60.0f) physics.Integrate(timeStep);
            else physics.Integrate(1.0f / 60.0f);

            // Update towers based on new physical state
            foreach (var tower in towers)
                tower.Update(gameTime);

            player.Update(gameTime);

            camera.Position = player.Position + new Vector3(0, player.Size.Y * 0.5f, 0);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float f = 200;
            if (gameTime.ElapsedGameTime.Milliseconds > 0) f *= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            else f = 0;                        

#if WINDOWS
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.LeftControl)) f *= 10;

            if (keyboard.IsKeyDown(Keys.LeftAlt) && keyboard.IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            if (keyboard.IsKeyDown(Keys.OemPlus))
                numInstances += f;
            else if (keyboard.IsKeyDown(Keys.OemMinus))
                numInstances -= f;

#endif
#if XBOX || true
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

            if (gamepad.IsButtonDown(Buttons.RightTrigger))
                numInstances += f;
            else if (gamepad.IsButtonDown(Buttons.LeftTrigger))
                numInstances -= f;
#endif

            SyncNumInstances();

            base.Update(gameTime);
        }

        private void SyncNumInstances()
        {
            int d = (int)numInstances - towers.Count;
            if (d == 0) return;

            if (d > 0) AddInstances((int)d);
            else RemoveInstances((int)d);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.BlendState = BlendState.Opaque;

            /*
            foreach (Tower tower in towers)
            {
                var wf = tower.PhysicsObject.PhysicsSkin.GetLocalSkinWireframe();
                tower.PhysicsObject.PhysicsBody.TransformWireframe(wf);
                ddrawer.DrawShape(wf);
            }
            //*/

            base.Draw(gameTime);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, "#Instances = " + towerRenderer.NumInstances, 
                new Vector2(20, 20), Color.White);

            spriteBatch.DrawString(spriteFont, "#Polygons = " + String.Format(CultureInfo.InvariantCulture, 
                "{0:n}", towerRenderer.NumPolygons), new Vector2(20, 40), Color.White);

            spriteBatch.End();
        }
    }
}
