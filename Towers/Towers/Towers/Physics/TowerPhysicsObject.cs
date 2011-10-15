using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

namespace Towers.Physics
{
    public class TowerPhysicsObject : PhysicsObject
    {    
        public TowerPhysicsObject(Vector3 size, Matrix orientation, Vector3 pos)
        {            
            body = new Body();
            collision = new CollisionSkin(body);

            collision.AddPrimitive(new Box(Vector3.Zero, orientation, size * 2), 
                new MaterialProperties(e : 0.8f, sr : 0.8f, dr : 0.7f));

            body.CollisionSkin = this.collision;
            
            Vector3 com = SetMass(1.0f);
            body.MoveTo(pos, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();

            body.Immovable = true;
        }
    }
}
