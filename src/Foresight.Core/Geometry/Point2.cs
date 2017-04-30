using System;
using System.Diagnostics;
using Core.MathLib;

namespace Core.Geometry
{
    [DebuggerDisplay("x={_x} y={_y}")]
    public struct Point2
    {
        private readonly float _x, _y;
        public static readonly Point2 Origin;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        public Point2(float x, float y)
        {
            this._x = x;
            this._y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2" /> struct.
        /// </summary>
        /// <param name="other">The other.</param>
        public Point2(Point2 other)
        {
            _x = other._x;
            _y = other._y;
        }

        /// <summary>
        ///     Return the minimum components of a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Point2 ComponentWiseMin(Point2 a, Point2 b)
        {
            return new Point2(
                a._x < b._x ? a._x : b._x,
                a._y < b._y ? a._y : b._y);
        }

        /// <summary>
        ///     Return the maximum components of a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Point2 ComponentWiseMax(Point2 a, Point2 b)
        {
            return new Point2(
                a._x > b._x ? a._x : b._x,
                a._y > b._y ? a._y : b._y);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector2" /> to <see cref="Point2" />.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Point2(Vector2 f)
        {
            return new Point2(f.X, f.Y);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector2" /> to <see cref="Point2" />.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Point2(Point3 p)
        {
            return new Point2(p.X, p.Y);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator +(Point2 p, Vector2 v)
        {
            return new Point2(p._x + v.X, p._y + v.Y);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator +(Point2 a, Point2 b)
        {
            return new Point2(a._x + b._x, a._y + b._y);
        }

        /// <summary>
        ///     Add
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator +(Point2 a, Point3 b)
        {
            return new Point2(a._x + b.X, a._y + b.Y);
        }

        /// <summary>
        ///     Subtraction
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator -(Point2 p1, Point2 p2)
        {
            return new Point2(p1._x - p2._x, p1._y - p2._y);
        }

        /// <summary>
        ///     Subtraction
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator -(Point2 p, Vector2 v)
        {
            return new Point2(p._x - v.X, p._y - v.Y);
        }

        /// <summary>
        ///     Used to negate the point.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator -(Point2 left)
        {
            return new Point2(-left._x, -left._y);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point2 a, Point2 b)
        {
            return MathCore.EqualityTest(a._x, b._x) && MathCore.EqualityTest(a._y, b._y);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point2 a, Point2 b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="p">The p.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator *(float value, Point2 p)
        {
            return new Point2(value * p._x, value * p._y);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator *(Point2 p, float value)
        {
            return new Point2(value * p._x, value * p._y);
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="p">The p.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator *(double value, Point2 p)
        {
            return new Point2((float) (value * p._x), (float) (value * p._y));
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Point2 operator *(Point2 p, double value)
        {
            return new Point2((float) (value * p._x), (float) (value * p._y));
        }

        /// <summary>
        ///     Multiply
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static float operator *(Point2 u, Point2 v)
        {
            return u._x * v._x + u._y * v._y;
        }

        /// <summary>
        ///     Equalses the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Point2))
                return false;

            return (Point2) obj == this;
        }

        /// <summary>
        ///     Gets the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        /// <summary>
        ///     Toes the string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _x.ToString("f3") + ", " + _y.ToString("f3");
        }

        /// <summary>
        ///     Offsets the specified point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <returns></returns>
        public static Point2 Offset(Point2 p, float dx, float dy)
        {
            return new Point2(p._x + dx, p._y + dy);
        }

        /// <summary>
        ///     Returns the distance between a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static float Distance(Point2 a, Point2 b)
        {
            var dx = a._x - b._x;
            var dy = a._y - b._y;
            return (float) Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        ///     Returns the distance between this point and the other point
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns></returns>
        public float Distance(Point2 a)
        {
            return Distance(this, a);
        }

        /// <summary>
        ///     Computes the angle between this point and other point
        /// </summary>
        /// <param name="that">The that.</param>
        /// <returns>the angle between 'this' and 'that' point seen from the x-axis.</returns>
        public double Angle(Point2 that)
        {
            double dX = that._x - _x;
            double dY = that._y - _y;
            double t;

            if (dX == 0.0)
            {
                if (dY == 0.0)
                    t = 0.0;
                else if (dY > 0.0)
                    t = Math.PI / 2.0;
                else
                    t = Math.PI * 3.0 / 2.0;
            }
            else if (dY == 0.0)
            {
                if (dX > 0.0)
                    t = 0.0;
                else
                    t = Math.PI;
            }
            else
            {
                if (dX < 0.0)
                    t = Math.Atan(dY / dX) + Math.PI;
                else if (dY < 0.0)
                    t = Math.Atan(dY / dX) + 2 * Math.PI;
                else
                    t = Math.Atan(dY / dX);
            }
            return t * 180 / Math.PI;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is not a number.
        /// </summary>
        /// <value><c>true</c> if this instance is na N; otherwise, <c>false</c>.</value>
        public bool IsNaN
        {
            get { return float.IsNaN(_x) || float.IsNaN(_y); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is infinity.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is infinity; otherwise, <c>false</c>.
        /// </value>
        public bool IsInfinity
        {
            get { return float.IsInfinity(_x) || float.IsInfinity(_y); }
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
    }
}