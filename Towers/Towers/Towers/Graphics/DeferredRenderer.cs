using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Towers.Graphics
{
    /// <summary>
    /// Render Job for the deferred renderer.    
    /// </summary>
    public interface RenderJob
    {
        /// <summary>
        /// Render geometry for deferred shading.
        /// The shader must write three color values for albedo color (COLOR0),
        /// normal (COLOR1) and depth (COLOR2).
        /// </summary>
        void Render();
    }

    public class DeferredRenderer : DrawableGameComponent
    {
        // Writes diffuse colors of objects
        Effect colorEffect;

        // Writes lighting information
        Effect lightEffect;

        // Combines light and colors
        Effect combineEffect;

        RenderTarget2D colorTarget;
        RenderTarget2D normalTarget;
        RenderTarget2D depthTarget;

        RenderTargetBinding[] renderTargetBindings;

        List<RenderJob> renderJobs = new List<RenderJob>();

        public DeferredRenderer(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            colorEffect = Game.Content.Load<Effect>("DiffuseColors.fx");
            lightEffect = Game.Content.Load<Effect>("DeferredShading.fx");
            combineEffect = Game.Content.Load<Effect>("Combine.fx");

            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            colorTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, 
                DepthFormat.Depth24Stencil8);
            normalTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, 
                DepthFormat.Depth24Stencil8);
            depthTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Single, 
                DepthFormat.Depth24Stencil8);

            renderTargetBindings = new RenderTargetBinding[] {
                new RenderTargetBinding(colorTarget), new RenderTargetBinding(normalTarget), 
                new RenderTargetBinding(depthTarget)
            };
            
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SetRenderTargets(renderTargetBindings);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            foreach (var job in renderJobs)
                job.Render();

            GraphicsDevice.SetRenderTargets(null);            
            
            base.Draw(gameTime);
        }
    }
}
