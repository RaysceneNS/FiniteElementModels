using System;

namespace Core.Geometry
{
    public struct Point2
    {
        public static readonly Point2 Origin;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        public Point2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        
        /// <summary>
        ///     Return the minimum components of a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Point2 ComponentWiseMin(Point2 a, Point2 b)
        {
            return new Point2(a.X < b.X ? a.X : b.X, a.Y < b.Y ? a.Y : b.Y);
        }

        /// <summary>
        ///     Return the maximum components of a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Point2 ComponentWiseMax(Point2 a, Point2 b)
        {
            return new Point2(a.X > b.X ? a.X : b.X, a.Y > b.Y ? a.Y : b.Y);
        }

        public static Point2 operator +(Point2 a, Point2 b)
        {
            return new Point2(a.X + b.X, a.Y + b.Y);
        }
        public static Point2 operator -(Point2 left)
        {
            return new Point2(-left.X, -left.Y);
        }

        public static bool operator ==(Point2 a, Point2 b)
        {
            return MathCore.EqualityTest(a.X, b.X) && MathCore.EqualityTest(a.Y, b.Y);
        }
        public static bool operator !=(Point2 a, Point2 b)
        {
            return !(a == b);
        }

        public static Point2 operator *(Point2 p, double value)
        {
            return new Point2((float) (value * p.X), (float) (value * p.Y));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2))
                return false;

            return (Point2) obj == this;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
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
            return new Point2(p.X + dx, p.Y + dy);
        }

        /// <summary>
        ///     Returns the distance between a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static float Distance(Point2 a, Point2 b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return (float) Math.Sqrt(dx * dx + dy * dy);
        }

        public float X { get; }
        public float Y { get; }
    }
}