using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JigLibX.Physics;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using JigLibX.Geometry;

namespace Towers.Physics
{
    public class PhysicsObject
    {
        protected Body body;
        protected CollisionSkin collision;

        public Matrix Transform
        {
            get
            {                
                return body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * body.Orientation *
                    Matrix.CreateTranslation(body.Position);
            }
        }

        public Vector3 Position
        {
            get { return body.Position; }
            set { body.Position = value; }
        }

        public Body PhysicsBody
        {
            get { return body; }
        }

        public CollisionSkin PhysicsSkin
        {
            get { return collision; }
        }

        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, 
                PrimitiveProperties.MassTypeEnum.Density, mass);

            float junk; Vector3 com; Matrix it, itCoM;

            collision.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }
    }
}
