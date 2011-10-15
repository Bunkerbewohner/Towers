using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Towers.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using JigLibX.Physics;
using Towers.Cameras;

namespace Towers.World.Actors
{
    public class Player
    {
        PlayerController controller;
        PlayerPhysicsObject physicsObject;
        float movementSpeed = 5f;
        Vector3 size;

        public float MovementSpeed
        {
            get { return movementSpeed; }
        }

        public Vector3 Size
        {
            get { return size; }
        }

        public Controller PhysicsController
        {
            get { return controller; }
        }

        public PlayerPhysicsObject PhysicsObject
        {
            get { return physicsObject; }
        }

        public Vector3 Position
        {
            get { return physicsObject.Position; }
            set { physicsObject.Position = value; }
        }

        public Player(Vector3 pos, QuakeCamera camera)
        {
            size = new Vector3(1, 1, 1);
            Matrix orientation = Matrix.Identity;

            physicsObject = new PlayerPhysicsObject(size, orientation, pos);

            controller = new PlayerController(this, camera);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            controller.UpdateController((float)gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond);
        }
    }
}
