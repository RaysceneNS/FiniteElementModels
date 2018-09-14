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

            var alen0 = a.GetLength(0);
            var alen1 = a.GetLength(1);
            var blen1 = b.GetLength(1);

            var m = new float[alen0, blen1];
            for (var i = 0; i < alen0; i++)
            {
                for (var k = 0; k < alen1; k++)
                {
                    for (var j = 0; j < blen1; j++)
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
            var dim1 = a.GetLength(0);
            var dim2 = a.GetLength(1);
            for (var i = 0; i < dim1; i++)
            {
                for (var j = 0; j < dim2; j++)
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

            var dim1 = matrix.GetLength(0);
            var dim2 = matrix.GetLength(1);

            var m = new float[dim2, dim1];
            for (var i = 0; i < dim1; i++)
            {
                for (var j = 0; j < dim2; j++)
                {
                    m[j, i] = matrix[i, j];
                }
            }
            return m;
        }
    }
}
