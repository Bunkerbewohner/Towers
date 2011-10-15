using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Towers.Physics;
using Towers.Graphics;

namespace Towers.World.Objects
{
    public class Tower
    {        
        protected TowerPhysicsObject physicsObject;
        protected TowerInstance graphicsInstance;

        public TowerInstance GraphicsInstance
        {
            get { return graphicsInstance; }
        }

        public TowerPhysicsObject PhysicsObject
        {
            get { return physicsObject; }
        }

        public Tower(Vector3 pos, Vector3 size, Matrix orientation, Color color)
        {                        
            // Physical representation
            //physicsObject = new TowerPhysicsObject(size, orientation, pos);

            // Graphical representation         
            Matrix transform = Matrix.CreateTranslation(pos) * orientation;
            graphicsInstance = new TowerInstance(transform, size, color);
        }

        public void Update(GameTime gameTime)
        {
            // Update graphics instance transform according to physics
            //graphicsInstance.Transform = physicsObject.Transform;
        }
    }
}
