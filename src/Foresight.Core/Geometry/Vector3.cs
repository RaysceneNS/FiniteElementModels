using System;
using System.Diagnostics;
using Core.MathLib;

namespace Core.Geometry
{
    /// <summary>
    /// </summary>
    [DebuggerDisplay("x={_x} y={_y} z={_z}")]
    public struct Vector3
    {
        private readonly float _x, _y, _z;

        private static readonly Vector3 Empty = new Vector3(0f, 0f, 0f);

        public static readonly Vector3 XUnit = new Vector3(1f, 0f, 0f);
        public static readonly Vector3 YUnit = new Vector3(0f, 1f, 0f);
        public static readonly Vector3 ZUnit = new Vector3(0f, 0f, 1f);
        
        
        public Vector3(float x, float y, float z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }
        
        public float X
        {
            get { return _x; }
        }
        
        public float Y
        {
            get { return _y; }
        }
        
        public float Z
        {
            get { return _z; }
        }

        /// <summary>
        ///     Is the length of this vector zero?
        /// </summary>
        public bool IsZero
        {
            get { return MathCore.EqualityTest(0.0, Magnitude); }
        }

        /// <summary>
        ///     Is the length of this vector 1.0 or very near to it?
        /// </summary>
        public bool IsUnit
        {
            get { return MathCore.EqualityTest(1.0, Magnitude); }
        }

        /// <summary>
        ///     Get the length of this vector, this is also refered to as it's magnitude
        /// </summary>
        public double Magnitude
        {
            get { return Math.Sqrt(_x * _x + _y * _y + _z * _z); }
        }

        public Vector3 Normalize()
        {
            return Normalize(this);
        }

        /// <summary>
        ///     Returns a normalized instance of this vector
        /// </summary>
        /// <returns></returns>
        public static Vector3 Normalize(Vector3 source)
        {
            var length = (float) source.Magnitude;
            if (length <= float.Epsilon)
                return Empty;
            var invlength = 1.0f / length;
            return new Vector3(source._x * invlength, source._y * invlength, source._z * invlength);
        }

        /// <summary>
        ///     Returns the angle projected enabled the XY plane.
        /// </summary>
        /// <returns></returns>
        public float AngleOnXy()
        {
            return (float) (Math.Atan2(_y, _x) * 180.0 / Math.PI);
        }

        /// <summary>
        ///     Returns the angle from XY plane.
        /// </summary>
        /// <returns></returns>
        public float AngleFromXy()
        {
            var x = Math.Sqrt(this._x * this._x + _y * _y);
            return (float)(Math.Atan2(_z, x) * 180.0 / Math.PI);
        }

        /// <summary>
        ///     Angle between two vectors (in radians)
        /// </summary>
        /// <param name="pU">The p U.</param>
        /// <param name="pV">The p V.</param>
        /// <returns></returns>
        public static double Angle(Vector3 pU, Vector3 pV)
        {
            var normU = pU.Magnitude;
            var normV = pV.Magnitude;
            var product = normU * normV;

            if (product == 0)
                return 0.0;

            double scalar = pU.X * pV.X +
                            pU.Y * pV.Y +
                            pU.Z * pV.Z;

            var cosinus = scalar / product;

            // Sinus
            var w = Cross(pU, pV);
            var wLength = w.Magnitude;

            var absSinus = wLength / product;

            // Remove degeneracy
            absSinus = absSinus > 1 ? 1 : absSinus;
            absSinus = absSinus < -1 ? -1 : absSinus;

            if (cosinus >= 0)
                return Math.Asin(absSinus);
            return Math.PI - Math.Asin(absSinus);
        }

        /// <summary>
        ///     Returns the reflection of a vector off a surface that has the specified normal.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <returns>The reflected vector.</returns>
        /// <remarks>
        ///     Reflect only gives the direction of a reflection off a surface, it does not determine
        ///     whether the original vector was close enough to the surface to hit it.
        /// </remarks>
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            var dot = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;

            return new Vector3(
                vector.X - 2.0f * dot * normal.X,
                vector.Y - 2.0f * dot * normal.Y,
                vector.Z - 2.0f * dot * normal.Z);
        }

        /// <summary>
        ///     Returns the linear blended interpolation of the 2 input vectors
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <param name="blend">The blend.</param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float blend)
        {
            return new Vector3(
                blend * (b._x - a._x) - a._x,
                blend * (b._y - a._y) - a._y,
                blend * (b._z - a._z) - a._z);
        }

        /// <summary>
        ///     Determine whether sequency a-b-c is Clockwise, Counterclockwise or Collinear
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int Orientation(Vector3 a, Vector3 b, Vector3 c)
        {
            var cross = Cross(b - a, b - c);
            var magnitudeSquare = cross._x * cross._x + cross._y * cross._y + cross._z * cross._z;

            if (magnitudeSquare > 0)
                return 1;
            if (magnitudeSquare < 0)
                return -1;
            return 0;
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
                a._y * b._z - a._z * b._y,
                a._z * b._x - a._x * b._z,
                a._x * b._y - a._y * b._x);
        }

        /// <summary>
        ///     Performs a Cross Product operation on 2 vectors, which returns a vector that is perpendicular
        ///     to the intersection of the 2 vectors.  Useful for finding face normals. Warning: cross products
        ///     are non commutative v1 X v2 != v2 X v1
        /// </summary>
        /// <param name="vector">A vector to perform the Cross Product against.</param>
        /// <returns>
        ///     A new Vector3 perpedicular to the 2 original vectors.
        /// </returns>
        public Vector3 Cross(Vector3 vector)
        {
            return new Vector3(
                _y * vector._z - _z * vector._y,
                _z * vector._x - _x * vector._z,
                _x * vector._y - _y * vector._x);
        }

        /// <summary>
        ///     Performs a Dot Product operation on 2 vectors, which produces the angle between them.
        /// </summary>
        /// <param name="v">The vector to perform the Dot Product against.</param>
        /// <returns>The angle between the 2 vectors.</returns>
        public float Dot(Vector3 v)
        {
            return _x * v._x + _y * v._y + _z * v._z;
        }

        /// <summary>
        ///     Performs a Dot Product operation on 2 vectors, which produces the angle between them.
        /// </summary>
        /// <returns>The angle between the 2 vectors.</returns>
        public static float Dot(Vector3 u, Vector3 v)
        {
            return u._x * v._x + u._y * v._y + u._z * v._z;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Point3" /> to <see cref="Vector3" />.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0f);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Point3" /> to <see cref="Vector3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3(Point3 point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        /// <summary>
        ///     Creates a vector from the subtraction of two points
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Vector3 Subtract(Point3 a, Point3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        ///     Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator -(Vector3 left)
        {
            return new Vector3(-left._x, -left._y, -left._z);
        }

        /// <summary>
        ///     Used to reinforce the elements of a vector.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator +(Vector3 left)
        {
            return new Vector3(+left._x, +left._y, +left._z);
        }

        
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return MathCore.EqualityTest(a._x, b._x) && MathCore.EqualityTest(a._y, b._y) &&
                   MathCore.EqualityTest(a._z, b._z);
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
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left._x - right._x, left._y - right._y, left._z - right._z);
        }

        /// <summary>
        ///     Subtracts a scalar value from all components of thsi vector
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 left, float right)
        {
            return new Vector3(left._x - right, left._y - right, left._z - right);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a._x + b._x, a._y + b._y, a._z + b._z);
        }

        /// <summary>
        ///     Add
        /// </summary>
        public static Vector3 operator +(Vector3 a, float right)
        {
            return new Vector3(a._x + right, a._y + right, a._z + right);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(float scalar, Vector3 v)
        {
            return new Vector3(scalar * v._x, scalar * v._y, scalar * v._z);
        }

        /// <summary>
        ///     Used when a Vector3 is multiplied by a scalar scalar.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 left, float scalar)
        {
            return new Vector3(left._x * scalar, left._y * scalar, left._z * scalar);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 operator *(double scalar, Vector3 v)
        {
            return new Vector3(
                (float) (scalar * v._x),
                (float) (scalar * v._y),
                (float) (scalar * v._z));
        }

        /// <summary>
        ///     Used when a Vector3 is multiplied by a scalar scalar.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 left, double scalar)
        {
            return new Vector3(
                (float) (left._x * scalar),
                (float) (left._y * scalar),
                (float) (left._z * scalar));
        }

        /// <summary>
        ///     scalar product
        /// </summary>
        /// <param name="a">first vector</param>
        /// <param name="b">second vector</param>
        /// <returns>scalar product</returns>
        public static float operator *(Vector3 a, Vector3 b)
        {
            return a._x * b._x + a._y * b._y + a._z * b._z;
        }

        /// <summary>
        ///     matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(Matrix3X3 matrix, Vector3 vector)
        {
            return new Vector3(
                matrix[0, 0] * vector._x + matrix[0, 1] * vector._y + matrix[0, 2] * vector._z,
                matrix[1, 0] * vector._x + matrix[1, 1] * vector._y + matrix[1, 2] * vector._z,
                matrix[2, 0] * vector._x + matrix[2, 1] * vector._y + matrix[2, 2] * vector._z);
        }

        /// <summary>
        ///     vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(Vector3 vector, Matrix3X3 matrix)
        {
            return new Vector3(
                matrix[0, 0] * vector._x + matrix[0, 1] * vector._y + matrix[0, 2] * vector._z,
                matrix[1, 0] * vector._x + matrix[1, 1] * vector._y + matrix[1, 2] * vector._z,
                matrix[2, 0] * vector._x + matrix[2, 1] * vector._y + matrix[2, 2] * vector._z);
        }

        /// <summary>
        ///     Transforms the given 3-D vector by the matrix, projecting the
        ///     result back into <i>w</i> = 1.
        ///     <p />
        ///     This means that the initial <i>w</i> is considered to be 1.0,
        ///     and then all the tree elements of the resulting 3-D vector are
        ///     divided by the resulting <i>w</i>.
        /// </summary>
        /// <param name="m">A Matrix4.</param>
        /// <param name="v">A Vector3.</param>
        /// <returns>A new vector.</returns>
        public static Vector3 operator *(Matrix4X4 m, Vector3 v)
        {
            //TODO verify this multiplication....
            return new Vector3(
                m[0, 0] * v._x + m[0, 1] * v._y + m[0, 2] * v._z + m[0, 3],
                m[1, 0] * v._x + m[1, 1] * v._y + m[1, 2] * v._z + m[1, 3],
                m[2, 0] * v._x + m[2, 1] * v._y + m[2, 2] * v._z + m[2, 3]);
        }

        /// <summary>
        ///     Used to divide a vector by a scalar scalar.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 left, float scalar)
        {
            Debug.Assert(scalar != 0.0f, "Cannot divide a Vector3 by zero.");

            // get the inverse of the scalar up front to avoid doing multiple divides later
            var inverse = 1.0f / scalar;
            return new Vector3(left._x * inverse, left._y * inverse, left._z * inverse);
        }
    }
}