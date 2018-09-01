using System;

namespace Core.Geometry
{
    public struct Vector3
    {
        private static readonly Vector3 Empty = new Vector3(0f, 0f, 0f);

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        /// <summary>
        ///     Is the length of this vector zero?
        /// </summary>
        public bool IsZero
        {
            get { return MathCore.EqualityTest(0.0, Magnitude); }
        }

        /// <summary>
        ///     Get the length of this vector, this is also refered to as it's magnitude
        /// </summary>
        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public Vector3 Normalize()
        {
            return Normalize(this);
        }

        /// <summary>
        ///     Returns a normalized instance of this vector
        /// </summary>
        /// <returns></returns>
        private static Vector3 Normalize(Vector3 source)
        {
            var length = (float) source.Magnitude;
            if (length <= float.Epsilon)
                return Empty;
            var invlength = 1.0f / length;
            return new Vector3(source.X * invlength, source.Y * invlength, source.Z * invlength);
        }

        /// <summary>
        ///     Returns the angle projected enabled the XY plane.
        /// </summary>
        /// <returns></returns>
        public float AngleOnXy()
        {
            return (float) (Math.Atan2(Y, X) * 180.0 / Math.PI);
        }

        /// <summary>
        ///     Returns the angle from XY plane.
        /// </summary>
        /// <returns></returns>
        public float AngleFromXy()
        {
            var x = Math.Sqrt(this.X * this.X + Y * Y);
            return (float)(Math.Atan2(Z, x) * 180.0 / Math.PI);
        }

        /// <summary>
        ///     Returns the cross product (i.e. outer product) of a + b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return MathCore.EqualityTest(a.X, b.X) && MathCore.EqualityTest(a.Y, b.Y) &&
                   MathCore.EqualityTest(a.Z, b.Z);
        }
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3))
                return false;
            return (Vector3) obj == this;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    }
}