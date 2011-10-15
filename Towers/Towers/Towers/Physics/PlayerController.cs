using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JigLibX.Physics;
using Towers.World.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Towers.Cameras;
using Towers.Graphics;

namespace Towers.Physics
{
    public class PlayerController : Controller
    {
        Player player;
        Body body;
        QuakeCamera camera;
        KeyboardState keyboard, prevKeyboard;

        float diffEpsilon = 0.001f;
        float stopEpsilon = 0.5f;

        float speed = 100;
        float jumpSpeed = 50;
        float jumpAcceleration = 50f;
        float maxVelocity = 20;
        float acceleration = 100f;
        float decceleration = 100f;

        Vector3 destVelocity = Vector3.Zero;
        Vector3 prevVelocity = Vector3.Zero;

        float jumpVelocity = 0;

        float previousDiff = float.MaxValue;

        public PlayerController(Player player, QuakeCamera camera)
        {
            this.camera = camera;
            this.player = player;
            body = player.PhysicsObject.PhysicsBody;
        }

        public override void UpdateController(float dt)
        {
            keyboard = Keyboard.GetState();
            if (!body.IsActive) body.SetActive();
            
            //Walk(dt);

            body.Velocity = GetMoveDir(true) * speed;

            Jump(dt);
            
            prevKeyboard = keyboard;
            prevVelocity = body.Velocity;
        }

        private void Jump(float dt)
        {
            if (keyboard.IsKeyDown(Keys.Space) && prevKeyboard.IsKeyUp(Keys.Space) && jumpVelocity == 0)
            {                
                // Start jump
                jumpVelocity = jumpSpeed;                
            }

            if (jumpVelocity > 0)
            {
                // Performing jump
                float d = jumpAcceleration * dt;

                body.Velocity = new Vector3(body.Velocity.X, body.Velocity.Y + d, body.Velocity.Z);

                jumpVelocity = Math.Max(0, jumpVelocity - d);
            }
        }

        private void Walk(float dt)
        {           
            if (!IsJumping())
                destVelocity = GetMoveDir() * speed;

            if (destVelocity.Length() < 0.1f)
            {
                body.Velocity = new Vector3(0, body.Velocity.Y, 0);
                return;
            }

            Vector2 dir = new Vector2(body.Velocity.X, body.Velocity.Z);            

            float dx = destVelocity.X - body.Velocity.X;
            float dz = destVelocity.Z - body.Velocity.Z;

            if (dx != 0)
                dir.X += Math.Sign(dx) * acceleration * dt;

            if (dz != 0)
                dir.Y += Math.Sign(dz) * acceleration * dt;

            if (dir.Length() > maxVelocity)
            {
                float length = dir.Length();

                dir.X = maxVelocity / length * dir.X;
                dir.Y = maxVelocity / length * dir.Y;
            }            

            body.Velocity = new Vector3(dir.X, body.Velocity.Y, dir.Y);
        }            

        private Vector3 GetMoveDir(bool threedim = false)
        {
            Vector3 v = IsJumping() ? destVelocity : Vector3.Zero;

            Vector3 forward = new Vector3(camera.Forward.X, threedim ? camera.Forward.Y : 0, camera.Forward.Z);
            Vector3 side = Vector3.Cross(forward, camera.UpVector);

            if (keyboard.IsKeyDown(Keys.W))
                v += forward;
            if (keyboard.IsKeyDown(Keys.S))
                v -= forward;            

            if (keyboard.IsKeyDown(Keys.A))
                v -= side;
            if (keyboard.IsKeyDown(Keys.D))
                v += side;

            if (v != Vector3.Zero) v.Normalize();            

            return v;
        }

        private bool IsJumping()
        {
            return Math.Abs(prevVelocity.Y - body.Velocity.Y) != 0;
        }
    }
}
