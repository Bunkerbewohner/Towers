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
using Towers.Cameras;


namespace Towers.Graphics
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TowerRenderer : DrawableGameComponent
    {
        Model model;
        ICamera camera;

        TowerInstanceVertex[] instanceVertices;
        List<TowerInstance> instances;
        DynamicVertexBuffer instanceVertexBuffer;

        public TowerRenderer(Game game)
            : base(game)
        {
            instanceVertices = new TowerInstanceVertex[0];
            instances = new List<TowerInstance>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            camera = Game.Services.GetService(typeof(ICamera)) as ICamera;

            base.Initialize();
        }

        public void AddInstance(TowerInstance instance)
        {
            instances.Add(instance);
        }

        public void AddInstances(IEnumerable<TowerInstance> instances)
        {
            this.instances.AddRange(instances);
        }

        public void RemoveInstance(TowerInstance instance)
        {
            if (instance == null)
                instances.RemoveAt(instances.Count - 1);
            else
                instances.Remove(instance);            
        }

        public void RemoveInstances(int n)
        {
            int num = Math.Min(n, instances.Count - 1);
            int index = instances.Count - num;

            if (num > 0)
                instances.RemoveRange(index, num);
        }

        public void Clear()
        {
            instances.Clear();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("Tower");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawInstances();
            
            base.Draw(gameTime);
        }

        private void DrawInstances()
        {
            if (instances.Count == 0) return;

            // Gather instance transform matrices into a single array.
            Array.Resize(ref instanceVertices, instances.Count);

            for (int i = 0; i < instances.Count; i++)
            {
                instanceVertices[i].Transform = instances[i].Transform;
                instanceVertices[i].Size = instances[i].Size;
                instanceVertices[i].Color = instances[i].Color;
            }

            // Draw all instances using hardware instancing
            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (instanceVertices.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, typeof(TowerInstanceVertex),
                                                               instanceVertices.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(instanceVertices, 0, instanceVertices.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(Matrix.Identity);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instanceVertices.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            

            base.Update(gameTime);
        }
    }
}
