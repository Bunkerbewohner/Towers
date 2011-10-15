using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Towers.Cameras
{
    public interface ICamera : IGameComponent
    {
        Matrix View { get; }

        Matrix Projection { get; }

        Vector3 Position { get; set; }
    }
}
