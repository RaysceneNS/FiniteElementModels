using System;

namespace Core.Fem
{
    public class Element
    {
        private float[,] _b;

        public Element(int node1, int node2, int node3)
        {
            this.Connection = new [] { node1, node2, node3 };
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

        internal void CalcVonMises(float[] localUnknowns)
        {
            this.Stress = Matrix.Multiply(this.Material, Matrix.Multiply(this._b, localUnknowns));
            this.VonMises = VonMises3D(this.Stress[0], this.Stress[1], 0, this.Stress[2], 0, 0);
        }

        internal static float VonMises3D(float sx, float sy, float sz, float txy, float txz, float tyz)
        {
            var stresses = (sx - sy) * (sx - sy) + (sx - sz) * (sx - sz) + (sy - sz) * (sy - sz);
            var vectors = txy * txy + txz * txz + tyz * tyz;
            return (float)(1.0 / Math.Sqrt(2.0) * Math.Sqrt(stresses + 6.0 * vectors));
        }

        public int ColorIndex { get; internal set; }

        public int[] NodeList
        {
            get
            {
                return this.Connection;
            }
        }

        private float[,] Material { get; set; }

        public int NumberOfNodes
        {
            get
            {
                return this.Connection.Length;
            }
        }

        public float[] Stress { get; private set; }

        public float VonMises { get; private set; }

        internal int[] Connection { get; }

        internal float[,] Stiffness { get; private set; }


        private static class Matrix
        {
            public static float[,] Multiply(float[,] a, float[,] b)
            {
                if (a == null)
                    throw new ArgumentNullException(nameof(a));
                if (b == null)
                    throw new ArgumentNullException(nameof(b));

                var m = new float[a.GetLength(0), b.GetLength(1)];
                for (var i = 0; i < a.GetLength(0); i++)
                {
                    for (var j = 0; j < b.GetLength(1); j++)
                    {
                        for (var k = 0; k < a.GetLength(1); k++)
                        {
                            m[i, j] += a[i, k] * b[k, j];
                        }
                    }
                }
                return m;
            }

            public static float[,] Multiply(float factor, float[,] m)
            {
                if (m == null)
                    throw new ArgumentNullException(nameof(m));

                var dim1 = m.GetLength(0);
                var dim2 = m.GetLength(1);
                var retval = new float[dim1, dim2];

                for (var j = 0; j < dim1; j++)
                {
                    for (var i = 0; i < dim2; i++)
                    {
                        retval[j, i] = m[j, i] * factor;
                    }
                }
                return retval;
            }

            public static float[] Multiply(float[,] a, float[] b)
            {
                if (a == null)
                    throw new ArgumentNullException(nameof(a));
                if (b == null)
                    throw new ArgumentNullException(nameof(b));

                var m = new float[b.Length];
                for (var i = 0; i < a.GetLength(0); i++)
                {
                    for (var j = 0; j < a.GetLength(1); j++)
                    {
                        m[i] += a[i, j] * b[j];
                    }
                }
                return m;
            }

            public static float[,] Transpose(float[,] matrix)
            {
                if (matrix == null)
                    throw new ArgumentNullException(nameof(matrix));

                var m = new float[matrix.GetLength(1), matrix.GetLength(0)];
                for (var i = 0; i < matrix.GetLength(0); i++)
                {
                    for (var j = 0; j < matrix.GetLength(1); j++)
                    {
                        m[j, i] = matrix[i, j];
                    }
                }
                return m;
            }
        }
    }
}
