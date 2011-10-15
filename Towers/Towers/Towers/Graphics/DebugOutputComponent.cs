using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Towers.Graphics
{
    public class DebugOutput : DrawableGameComponent
    {
        private static DebugOutput _instance = new DebugOutput();
        public static DebugOutput Instance { get { return _instance; } }        

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        List<Output> outputs = new List<Output>();

        private DebugOutput()
            : base(null)
        {

        }

        public DebugOutput(Game game)
            : base(game)
        {
            DrawOrder = 100;
            _instance = this;
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteFont = Game.Content.Load<SpriteFont>("DefaultFont");
            
            base.LoadContent();
        }

        public void Add(Output output)
        {
            if (Game == null) return;
            outputs.Add(output);
        }

        public void Add(String message, Vector2 position)
        {
            if (Game == null) return;
            outputs.Add(new Output(message, position));
        }

        public void Add(String message, Vector3 v, Vector2 position)
        {
            message += String.Format("{0:0.0000},{1:0.0000},{2:0.0000}", v.X,
                v.Y, v.Z);

            Add(message, position);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            foreach (var output in outputs)
            {
                spriteBatch.DrawString(spriteFont, output.Message, output.Position, Color.White);
            }

            spriteBatch.End();            

            base.Draw(gameTime);
            outputs.Clear();
        }
    }

    public struct Output
    {
        public Vector2 Position;
        public String Message;

        public Output(String message, Vector2 position)
        {
            Position = position;
            Message = message;
        }
    }
}
