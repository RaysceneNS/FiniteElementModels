using System;
using System.Diagnostics;

namespace Core.Geometry
{
    [DebuggerDisplay("center={Center} extentX={ExtentX} extentY={ExtentY} extentZ={ExtentZ}")]
    public struct AxisAlignedBox3
    {
        public static readonly AxisAlignedBox3 Empty = new AxisAlignedBox3();

        private AxisAlignedBox3(Point3 center, float extentX, float extentY, float extentZ)
        {
            Center = center;

            if (extentX < 0)
                throw new ArgumentOutOfRangeException(nameof(extentX), extentX,
                    "X extent must be greater than or equal to zero");
            if (extentY < 0)
                throw new ArgumentOutOfRangeException(nameof(extentY), extentY,
                    "Y extent must be greater than or equal to zero");
            if (extentZ < 0)
                throw new ArgumentOutOfRangeException(nameof(extentZ), extentZ,
                    "Z extent must be greater than or equal to zero");

            ExtentX = extentX;
            ExtentY = extentY;
            ExtentZ = extentZ;
        }

        /// <summary>
        ///     Creates a new axis aligned box from the extreme corner points provided
        /// </summary>
        /// <returns></returns>
        public static AxisAlignedBox3 FromExtents(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            var center = new Point3((maxX + minX) / 2f, (maxY + minY) / 2f, (maxZ + minZ) / 2f);

            var extentX = (maxX - minX) / 2f;
            var extentY = (maxY - minY) / 2f;
            var extentZ = (maxZ - minZ) / 2f;

            return new AxisAlignedBox3(center, extentX, extentY, extentZ);
        }
        
        public override int GetHashCode()
        {
            return Center.GetHashCode() ^
                   ExtentX.GetHashCode() ^
                   ExtentY.GetHashCode() ^
                   ExtentZ.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AxisAlignedBox3))
                return false;
            return (AxisAlignedBox3) obj == this;
        }

        /// <summary>
        ///     Creates the smallest possible third box that can contain both of two boxes that form a union.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static AxisAlignedBox3 Union(AxisAlignedBox3 a, AxisAlignedBox3 b)
        {
            var minX = Math.Min(a.Left, b.Left);
            var maxX = Math.Max(a.Right, b.Right);
            var minY = Math.Min(a.Bottom, b.Bottom);
            var maxY = Math.Max(a.Top, b.Top);
            var minZ = Math.Min(a.Back, b.Back);
            var maxZ = Math.Max(a.Front, b.Front);

            return FromExtents(minX, minY, minZ, maxX, maxY, maxZ);
        }
        
        public static bool operator ==(AxisAlignedBox3 left, AxisAlignedBox3 right)
        {
            return left.Center == right.Center &&
                   Equals(left.ExtentX, right.ExtentX) &&
                   Equals(left.ExtentY, right.ExtentY) &&
                   Equals(left.ExtentZ, right.ExtentZ);
        }
        public static bool operator !=(AxisAlignedBox3 left, AxisAlignedBox3 right)
        {
            return !(left == right);
        }

        public Point3 Center { get; }

        private float Left
        {
            get { return Center.X - ExtentX; }
        }

        private float Right
        {
            get { return Center.X + ExtentX; }
        }

        private float Bottom
        {
            get { return Center.Y - ExtentY; }
        }

        private float Top
        {
            get { return Center.Y + ExtentY; }
        }

        private float Back
        {
            get { return Center.Z - ExtentZ; }
        }

        private float Front
        {
            get { return Center.Z + ExtentZ; }
        }

        private float ExtentX { get; }
        private float ExtentY { get; }
        private float ExtentZ { get; }

        /// <summary>
        ///     Gets the extent of this box as the distance measured from it's opposite corners.
        /// </summary>
        /// <value>The extent.</value>
        public float Extent
        {
            get
            {
                var xSize = ExtentX * 2;
                var ySize = ExtentY * 2;
                var zSize = ExtentZ * 2;
                return (float) Math.Sqrt(xSize * xSize + ySize * ySize + zSize * zSize);
            }
        }
    }
}