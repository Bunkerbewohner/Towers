using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;

namespace Towers.Physics
{
    public class PlayerPhysicsObject : PhysicsObject
    {        
        public Character CharacterBody { get { return body as Character; } }

        public PlayerPhysicsObject(Vector3 size, Matrix orientation, Vector3 pos)
        {
            body = new Body();
            collision = new CollisionSkin(body);

            collision.AddPrimitive(new Sphere(pos, size.Y), (int)MaterialTable.MaterialID.NotBouncyNormal);

            body.CollisionSkin = this.collision;

            Vector3 com = SetMass(1.0f);
            body.MoveTo(pos, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();

            //body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);
            body.AllowFreezing = false;
        }
    }

    public class Character : Body
    {
        public Character()            
        {
        }

        public Vector3 DesiredVelocity { get; set; }

        private bool doJump = false;

        public void DoJump()
        {
            doJump = true;
        }

        public override void AddExternalForces(float dt)
        {
            ClearForces();

            if (doJump)
            {
                foreach (CollisionInfo info in CollisionSkin.Collisions)
                {
                    Vector3 N = info.DirToBody0;
                    if (this == info.SkinInfo.Skin1.Owner)
                        Vector3.Negate(ref N, out N);

                    if (Vector3.Dot(N, Orientation.Up) > 0.7f)
                    {
                        Vector3 vel = Velocity; vel.Y = 5.0f;
                        Velocity = vel;
                        break;
                    }
                }
            }

            Vector3 deltaVel = DesiredVelocity - Velocity;

            bool running = true;

            if (DesiredVelocity.LengthSquared() < JiggleMath.Epsilon) running = false;
            else deltaVel.Normalize();

            deltaVel.Y = 0.0f;

            // start fast, slow down slower
            if (running) deltaVel *= 10.0f;
            else deltaVel *= 2.0f;

            float forceFactor = 1000.0f;

            AddBodyForce(deltaVel * Mass * dt * forceFactor);

            doJump = false;
            AddGravityToExternalForce();
        }
    }
}
