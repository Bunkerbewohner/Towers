



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Towers.Graphics
{
    struct TowerInstanceVertex : IVertexType
    {
        public Vector4 TransformRow1;
        public Vector4 TransformRow2;
        public Vector4 TransformRow3;
        public Vector4 TransformRow4;
        public Vector3 Size;
        public Color Color;

        public Matrix Transform
        {
            get
            {
                return CreateMatrix(ref TransformRow1, ref TransformRow2, ref TransformRow3, ref TransformRow4);
            }
            set
            {
                TransformRow1 = new Vector4(value.M11, value.M21, value.M31, value.M41);
                TransformRow2 = new Vector4(value.M12, value.M22, value.M32, value.M42);
                TransformRow3 = new Vector4(value.M13, value.M23, value.M33, value.M43);
                TransformRow4 = new Vector4(value.M14, value.M24, value.M34, value.M44);
            }
        }

        private static Matrix CreateMatrix(ref Vector4 row1, ref Vector4 row2, ref Vector4 row3, ref Vector4 row4)
        {
            return new Matrix(row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W,
                row3.X, row3.Y, row3.Z, row3.W, row4.X, row4.Y, row4.Z, row4.W);
        }

        private static VertexDeclaration _decl;

        static TowerInstanceVertex()
        {
            _decl = new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
                    new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
                    new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
                    new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3),
                    new VertexElement(64, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
                    new VertexElement(76, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                );
        }

        public TowerInstanceVertex(Matrix transform, Vector3 size, Color color)
        {
            Size = size;
            Color = color;

            TransformRow1 = new Vector4(transform.M11, transform.M21, transform.M31, transform.M41);
            TransformRow2 = new Vector4(transform.M12, transform.M22, transform.M32, transform.M42);
            TransformRow3 = new Vector4(transform.M13, transform.M23, transform.M33, transform.M43);
            TransformRow4 = new Vector4(transform.M14, transform.M24, transform.M34, transform.M44);
        }

        public VertexDeclaration VertexDeclaration
        {
            get { return _decl; }
        }
    }
}