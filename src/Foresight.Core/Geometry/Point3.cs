using System;
using System.Diagnostics;
using Core.MathLib;

namespace Core.Geometry
{
    [DebuggerDisplay("x={_x} y={_y} z={_z}")]
    public struct Point3
    {
        private readonly float _x, _y, _z;
        public static readonly Point3 Origin;
        
        /// <summary>
        ///     Construct a point from 3d coordinates
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z">Z.</param>
        public Point3(float x, float y, float z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector3" /> to <see cref="Point3" />.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Point3(Vector3 f)
        {
            return new Point3(f.X, f.Y, f.Z);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector3" /> to <see cref="Point3" />.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Point3(Point2 p)
        {
            return new Point3(p.X, p.Y, 0f);
        }

        /// <summary>
        ///     Test for equality
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point3 a, Point3 b)
        {
            return MathCore.EqualityTest(a._x, b._x) && MathCore.EqualityTest(a._y, b._y) &&
                   MathCore.EqualityTest(a._z, b._z);
        }

        /// <summary>
        ///     Test for inequality
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point3 a, Point3 b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Subtraction
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator -(Point3 p1, Point3 p2)
        {
            return new Point3(p1._x - p2._x, p1._y - p2._y, p1._z - p2._z);
        }

        /// <summary>
        ///     Subtract
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator -(Point3 p, Vector3 v)
        {
            return new Point3(p._x - v.X, p._y - v.Y, p._z - v.Z);
        }

        /// <summary>
        ///     Used to negate the point.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator -(Point3 left)
        {
            return new Point3(-left._x, -left._y, -left._z);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="p">The p.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator *(float value, Point3 p)
        {
            return new Point3(value * p._x, value * p._y, value * p._z);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator *(Point3 p, float value)
        {
            return new Point3(value * p._x, value * p._y, value * p._z);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static float operator *(Point3 u, Point3 v)
        {
            return u._x * v._x + u._y * v._y + u._z * v._z;
        }

        /// <summary>
        ///     Multiplies the specified point.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="p">The p.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator *(Matrix4X4 m, Point3 p)
        {
            return new Point3(
                m.M00 * p._x + m.M01 * p._y + m.M02 * p._z + m.M03,
                m.M10 * p._x + m.M11 * p._y + m.M12 * p._z + m.M13,
                m.M20 * p._x + m.M21 * p._y + m.M22 * p._z + m.M23);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator +(Point3 p, Vector3 v)
        {
            return new Point3(p._x + v.X, p._y + v.Y, p._z + v.Z);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator +(Point3 a, Point3 b)
        {
            return new Point3(a._x + b._x, a._y + b._y, a._z + b._z);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static Point3 operator +(Point3 a, Point2 b)
        {
            return new Point3(a._x + b.X, a._y + b.Y, a._z);
        }

        /// <summary>
        ///     X
        /// </summary>
        /// <value>The X.</value>
        public float X
        {
            get { return _x; }
        }

        /// <summary>
        ///     Y
        /// </summary>
        /// <value>The Y.</value>
        public float Y
        {
            get { return _y; }
        }

        /// <summary>
        ///     Z
        /// </summary>
        /// <value>The Z.</value>
        public float Z
        {
            get { return _z; }
        }

        /// <summary>
        ///     Crosses the specified points.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Point3 Cross(Point3 a, Point3 b)
        {
            return new Point3(
                a._y * b._z - a._z * b._y,
                a._z * b._x - a._x * b._z,
                a._x * b._y - a._y * b._x);
        }

        /// <summary>
        ///     Return the minimum components of a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Point3 ComponentWiseMin(Point3 a, Point3 b)
        {
            return new Point3(
                a._x < b._x ? a._x : b._x,
                a._y < b._y ? a._y : b._y,
                a._z < b._z ? a._z : b._z);
        }

        /// <summary>
        ///     Return the maximum components of a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Point3 ComponentWiseMax(Point3 a, Point3 b)
        {
            return new Point3(
                a._x > b._x ? a._x : b._x,
                a._y > b._y ? a._y : b._y,
                a._z > b._z ? a._z : b._z);
        }

        /// <summary>
        ///     Determine whether sequence a-b-c is Collinear
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <param name="c">C</param>
        /// <returns></returns>
        public static bool Collinear(Point3 a, Point3 b, Point3 c)
        {
            var crossPoint = Cross(c - a, c - b);

            double length = a._x * crossPoint._x + a._y * crossPoint._y + a._z * crossPoint._z;

            if (length == 0)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point3))
                return false;

            return (Point3) obj == this;
        }

        public override int GetHashCode()
        {
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        /// <summary>
        ///     Offsets the specified point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="dz">The dz.</param>
        /// <returns></returns>
        public static Point3 Offset(Point3 p, float dx, float dy, float dz)
        {
            return new Point3(p._x + dx, p._y + dy, p._z + dz);
        }

        /// <summary>
        ///     Get distance between two points
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns></returns>
        public static float Distance(Point3 p1, Point3 p2)
        {
            var xSeg = p2._x - p1._x;
            var ySeg = p2._y - p1._y;
            var zSeg = p2._z - p1._z;
            return (float) Math.Sqrt(xSeg * xSeg + ySeg * ySeg + zSeg * zSeg);
        }
    }
}