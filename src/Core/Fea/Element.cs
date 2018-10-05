using System;

namespace Core.Fea
{
    public class Element
    {
        private float[,] _b;

        public Element(int node1, int node2, int node3)
        {
            this.Node1 = node1;
            this.Node2 = node2;
            this.Node3 = node3;
        }

        internal void CalcElemK(float thickness, float young, float poisson, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var b = new float[3, 6];
            b[0, 0] = y2 - y3;
            b[0, 2] = y3 - y1;
            b[0, 4] = y1 - y2;
            b[1, 1] = x3 - x2;
            b[1, 3] = x1 - x3;
            b[1, 5] = x2 - x1;
            b[2, 0] = x3 - x2;
            b[2, 1] = y2 - y3;
            b[2, 2] = x1 - x3;
            b[2, 3] = y3 - y1;
            b[2, 4] = x2 - x1;
            b[2, 5] = y1 - y2;

            float area = Area2D(x1, y1, x2, y2, x3, y3);
            this._b = Matrix.Multiply(1.0f / (2.0f * area), b);

            var e = new float[3, 3];
            e[0, 0] = 1.0f;
            e[0, 1] = poisson;
            e[1, 0] = poisson;
            e[1, 1] = 1.0f;
            e[2, 2] = (1.0f - poisson) / 2.0f;
            this.Material = Matrix.Multiply(young / (1.0f - poisson * poisson), e);

            var m1 = Matrix.Transpose(this._b);
            var m2 = Matrix.Multiply(m1, this.Material);
            var k = Matrix.Multiply(m2, this._b);

            this.Stiffness = Matrix.Multiply(thickness * area, k);
        }

        /// <summary>
        ///     Calculate the signed area of the triangle
        /// </summary>
        /// <returns>
        ///     a positive area if all points wind to the right (clockwise),
        ///     0 if all points are coincident,
        ///     a negative value if points wind counter clockwise
        /// </returns>
        private static float Area2D(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2f;
        }

        internal void CalculateStress(float[] localUnknowns)
        {
            this.Stress = Matrix.Multiply(this.Material, Matrix.Multiply(this._b, localUnknowns));
        }

        internal static float VonMises3D(float sx, float sy, float sz, float txy, float txz, float tyz)
        {
            var stresses = (sx - sy) * (sx - sy) +
                           (sx - sz) * (sx - sz) + 
                           (sy - sz) * (sy - sz);
            var vectors = txy * txy + txz * txz + tyz * tyz;
            return (float)(1.0 / Math.Sqrt(2.0) * Math.Sqrt(stresses + 6.0 * vectors));
        }
        
        private float[,] Material { get; set; }
        
        public float[] Stress { get; private set; }

        public int Node1 { get; }
        public int Node2 { get; }
        public int Node3 { get; }

        internal float[,] Stiffness { get; private set; }
    }
}
