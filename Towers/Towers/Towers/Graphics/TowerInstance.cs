using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Towers.Graphics
{
    public class TowerInstance
    {
        public Matrix Transform = Matrix.Identity;
        public Vector3 Size = Vector3.One;
        public Color Color = Color.Yellow;

        public TowerInstance(Matrix transform, Vector3 size, Color color)
        {
            Transform = transform;
            Size = size;
            Color = color;
        }
    }
}
