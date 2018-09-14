using System;

namespace Core.Fea
{
    internal static class Matrix
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
