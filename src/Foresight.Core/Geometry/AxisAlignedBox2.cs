using System;
using System.Diagnostics;

namespace Core.Geometry
{
    [DebuggerDisplay("center={_center} extentX={ExtentX} extentY={ExtentY}")]
    public struct AxisAlignedBox2
    {
        public static readonly AxisAlignedBox2 Empty = new AxisAlignedBox2();

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxisAlignedBox2" /> struct.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="extentX">The extent X.</param>
        /// <param name="extentY">The extent Y.</param>
        public AxisAlignedBox2(float x, float y, float extentX, float extentY)
        {
            Center = new Point2(x, y);
            ExtentX = extentX;
            ExtentY = extentY;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxisAlignedBox2" /> struct.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="extentX">The extent X.</param>
        /// <param name="extentY">The extent Y.</param>
        public AxisAlignedBox2(Point2 center, float extentX, float extentY)
        {
            Center = center;
            ExtentX = extentX;
            ExtentY = extentY;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxisAlignedBox2" /> struct.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        public AxisAlignedBox2(Point2 a, Point2 b)
        {
            var min = Point2.ComponentWiseMin(a, b);
            var max = Point2.ComponentWiseMax(a, b);

            Center = new Point2((max.X + min.X) / 2f, (max.Y + min.Y) / 2f);
            ExtentX = (max.X - min.X) / 2f;
            ExtentY = (max.Y - min.Y) / 2f;
        }

        /// <summary>
        ///     Creates a new axis aligned box from the extreme corners
        /// </summary>
        /// <param name="minX">The min X.</param>
        /// <param name="minY">The min Y.</param>
        /// <param name="maxX">The max X.</param>
        /// <param name="maxY">The max Y.</param>
        /// <returns></returns>
        public static AxisAlignedBox2 FromExtents(float minX, float minY, float maxX, float maxY)
        {
            var center = new Point2((maxX + minX) / 2f, (maxY + minY) / 2f);
            var extentX = (maxX - minX) / 2f;
            var extentY = (maxY - minY) / 2f;

            return new AxisAlignedBox2(center, extentX, extentY);
        }

        /// <summary>
        ///     Determines whether [contains] [the specified pt].
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <returns>
        ///     <c>true</c> if [contains] [the specified pt]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Point2 pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        ///     Determines whether [contains] [the specified rect].
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>
        ///     <c>true</c> if [contains] [the specified rect]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(AxisAlignedBox2 rect)
        {
            return Left <= rect.Left && Right >= rect.Right &&
                   Bottom <= rect.Bottom && Top >= rect.Top;
        }

        /// <summary>
        ///     Determines whether [contains] [the specified x].
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <returns>
        ///     <c>true</c> if [contains] [the specified x]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(float x, float y)
        {
            return Left <= x && Right >= x &&
                   Bottom <= y && Top >= y;
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return Center.GetHashCode() ^
                   ExtentX.GetHashCode() ^
                   ExtentY.GetHashCode();
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        ///     true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is AxisAlignedBox2))
                return false;

            return (AxisAlignedBox2) obj == this;
        }

        /// <summary>
        ///     Creates the smallest possible third rectangle that can contain both of two rectangles that form a union.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static AxisAlignedBox2 Union(AxisAlignedBox2 a, AxisAlignedBox2 b)
        {
            var minX = Math.Min(a.Left, b.Left);
            var maxX = Math.Max(a.Right, b.Right);

            var minY = Math.Min(a.Bottom, b.Bottom);
            var maxY = Math.Max(a.Top, b.Top);

            return FromExtents(minX, minY, maxX, maxY);
        }
        
        /// <summary>
        ///     Inflates the AxisAlignedBox2 structure by the specified amount.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <returns></returns>
        public static AxisAlignedBox2 Inflate(AxisAlignedBox2 rect, float x, float y)
        {
            return new AxisAlignedBox2(rect.X, rect.Y, rect.ExtentX+x, rect.ExtentY+y);
        }

        /// <summary>
        ///     Returns a AxisAlignedBox2 structure that represents the intersection of two rectangles. If there is no intersection,
        ///     and empty AxisAlignedBox2 is returned.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static AxisAlignedBox2 Intersect(AxisAlignedBox2 a, AxisAlignedBox2 b)
        {
            var x1 = Math.Max(a.Left, b.Left);
            var x2 = Math.Min(a.Right, b.Right);
            var y1 = Math.Max(a.Bottom, b.Bottom);
            var y2 = Math.Min(a.Top, b.Top);

            if (x2 >= x1 && y2 >= y1)
                return FromExtents(x1, y1, x2, y2);
            return Empty;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(AxisAlignedBox2 left, AxisAlignedBox2 right)
        {
            return left.Center == right.Center &&
                   Equals(left.ExtentX, right.ExtentX) &&
                   Equals(left.ExtentY, right.ExtentY);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(AxisAlignedBox2 left, AxisAlignedBox2 right)
        {
            return !(left == right);
        }
        
        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return ExtentX <= 0f || ExtentY <= 0f; }
        }

        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point2 Center { get; }

        /// <summary>
        ///     Gets the center X coord.
        /// </summary>
        /// <value>The X.</value>
        public float X
        {
            get { return Center.X; }
        }

        /// <summary>
        ///     Gets the center Y coord.
        /// </summary>
        /// <value>The Y.</value>
        public float Y
        {
            get { return Center.Y; }
        }

        /// <summary>
        ///     Gets the left.
        /// </summary>
        /// <value>The left.</value>
        public float Left
        {
            get { return Center.X - ExtentX; }
        }

        /// <summary>
        ///     Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public float Right
        {
            get { return Center.X + ExtentX; }
        }

        /// <summary>
        ///     Gets the top.
        /// </summary>
        /// <value>The top.</value>
        public float Top
        {
            get { return Center.Y + ExtentY; }
        }

        /// <summary>
        ///     Gets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public float Bottom
        {
            get { return Center.Y - ExtentY; }
        }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get { return ExtentX * 2; }
        }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get { return ExtentY * 2; }
        }

        /// <summary>
        ///     Gets or sets the extent X.
        /// </summary>
        /// <value>The extent X.</value>
        public float ExtentX { get; }

        /// <summary>
        ///     Gets or sets the extent Y.
        /// </summary>
        /// <value>The extent Y.</value>
        public float ExtentY { get; }
    }
}