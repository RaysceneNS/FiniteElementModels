using System;

namespace Core.Geometry
{
    public struct AxisAlignedBox2
    {
        public static readonly AxisAlignedBox2 Empty = new AxisAlignedBox2();

        private AxisAlignedBox2(Point2 center, float extentX, float extentY)
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

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ ExtentX.GetHashCode() ^ ExtentY.GetHashCode();
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
            return new AxisAlignedBox2(rect.Center, rect.ExtentX + x, rect.ExtentY + y);
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

        private float Left
        {
            get { return Center.X - ExtentX; }
        }

        private float Right
        {
            get { return Center.X + ExtentX; }
        }

        private float Top
        {
            get { return Center.Y + ExtentY; }
        }

        private float Bottom
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

        private float ExtentX { get; }
        private float ExtentY { get; }
    }
}