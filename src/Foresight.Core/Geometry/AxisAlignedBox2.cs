using System;
using System.Diagnostics;

namespace Core.Geometry
{
    [DebuggerDisplay("center={_center} extentX={ExtentX} extentY={ExtentY}")]
    public struct AxisAlignedBox2
    {
        public static readonly AxisAlignedBox2 Empty = new AxisAlignedBox2();

        public AxisAlignedBox2(float x, float y, float extentX, float extentY)
        {
            Center = new Point2(x, y);
            ExtentX = extentX;
            ExtentY = extentY;
        }

        public AxisAlignedBox2(Point2 center, float extentX, float extentY)
        {
            Center = center;
            ExtentX = extentX;
            ExtentY = extentY;
        }

        public AxisAlignedBox2(Point2 a, Point2 b)
        {
            var min = Point2.ComponentWiseMin(a, b);
            var max = Point2.ComponentWiseMax(a, b);

            Center = new Point2((max.X + min.X) / 2f, (max.Y + min.Y) / 2f);
            ExtentX = (max.X - min.X) / 2f;
            ExtentY = (max.Y - min.Y) / 2f;
        }

        public static AxisAlignedBox2 FromExtents(float minX, float minY, float maxX, float maxY)
        {
            var center = new Point2((maxX + minX) / 2f, (maxY + minY) / 2f);
            var extentX = (maxX - minX) / 2f;
            var extentY = (maxY - minY) / 2f;

            return new AxisAlignedBox2(center, extentX, extentY);
        }

        public bool Contains(Point2 pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(AxisAlignedBox2 rect)
        {
            return Left <= rect.Left && Right >= rect.Right &&
                   Bottom <= rect.Bottom && Top >= rect.Top;
        }

        public bool Contains(float x, float y)
        {
            return Left <= x && Right >= x &&
                   Bottom <= y && Top >= y;
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^
                   ExtentX.GetHashCode() ^
                   ExtentY.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AxisAlignedBox2))
                return false;

            return (AxisAlignedBox2) obj == this;
        }

        public static AxisAlignedBox2 Union(AxisAlignedBox2 a, AxisAlignedBox2 b)
        {
            var minX = Math.Min(a.Left, b.Left);
            var maxX = Math.Max(a.Right, b.Right);

            var minY = Math.Min(a.Bottom, b.Bottom);
            var maxY = Math.Max(a.Top, b.Top);

            return FromExtents(minX, minY, maxX, maxY);
        }
        
        public static AxisAlignedBox2 Inflate(AxisAlignedBox2 rect, float x, float y)
        {
            return new AxisAlignedBox2(rect.X, rect.Y, rect.ExtentX+x, rect.ExtentY+y);
        }

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

        public static bool operator ==(AxisAlignedBox2 left, AxisAlignedBox2 right)
        {
            return left.Center == right.Center &&
                   Equals(left.ExtentX, right.ExtentX) &&
                   Equals(left.ExtentY, right.ExtentY);
        }

        public static bool operator !=(AxisAlignedBox2 left, AxisAlignedBox2 right)
        {
            return !(left == right);
        }
        
        public bool IsEmpty
        {
            get { return ExtentX <= 0f || ExtentY <= 0f; }
        }

        public Point2 Center { get; }

        public float X
        {
            get { return Center.X; }
        }

        public float Y
        {
            get { return Center.Y; }
        }

        public float Left
        {
            get { return Center.X - ExtentX; }
        }

        public float Right
        {
            get { return Center.X + ExtentX; }
        }

        public float Top
        {
            get { return Center.Y + ExtentY; }
        }

        public float Bottom
        {
            get { return Center.Y - ExtentY; }
        }

        public float Width
        {
            get { return ExtentX * 2; }
        }

        public float Height
        {
            get { return ExtentY * 2; }
        }

        public float ExtentX { get; }

        public float ExtentY { get; }
    }
}