using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Towers.GUI
{
    /// <summary>
    /// Computes and draws the current Framerate
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game)
            : base(game)
        {
            content = game.Content;
            DrawOrder = 100;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("DefaultFont");
        }

        /// <summary>
        /// Computes the current Framerate
        /// </summary>
        /// <param name="gameTime">base gametime of Computation</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        /// <summary>
        /// Draws a string which shows the current Framerate in the left upper corner
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("FPS: {0}", frameRate);

            spriteBatch.Begin();

            int screenWidth = GraphicsDevice.Viewport.Width;
            spriteBatch.DrawString(spriteFont, fps, new Vector2(screenWidth - 100, 20), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(screenWidth - 102, 18), Color.White);

            spriteBatch.End();
        }
    }
}