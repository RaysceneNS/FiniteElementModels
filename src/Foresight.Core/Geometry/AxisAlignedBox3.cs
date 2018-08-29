using System;
using System.Diagnostics;

namespace Core.Geometry
{
    [DebuggerDisplay("center={Center} extentX={ExtentX} extentY={ExtentY} extentZ={ExtentZ}")]
    public struct AxisAlignedBox3
    {
        public static readonly AxisAlignedBox3 Empty = new AxisAlignedBox3();

        public AxisAlignedBox3(float x, float y, float z, float extentX, float extentY, float extentZ)
        {
            Center = new Point3(x, y, z);

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

        public AxisAlignedBox3(Point3 center, float extentX, float extentY, float extentZ)
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

        /// <summary>
        ///     Adjusts the location of this rectangle by the specified amounts.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z"></param>
        public static AxisAlignedBox3 Offset(AxisAlignedBox3 rect, float x, float y, float z)
        {
            return new AxisAlignedBox3(x + x, y + y, z + z, rect.ExtentX, rect.ExtentY, rect.ExtentZ);
        }
        
        /// <summary>
        ///     Inflates the AxisAlignedBox3D structure by the specified amount.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static AxisAlignedBox3 Inflate(AxisAlignedBox3 rect, float x, float y, float z)
        {
            return new AxisAlignedBox3(x, y, z, rect.ExtentX + x, rect.ExtentY + y, rect.ExtentZ + z);
        }

        /// <summary>
        ///     Tests whether the point is within this box.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>
        ///     True if the vector is within this box, false otherwise.
        /// </returns>
        public bool Intersects(Point3 point)
        {
            return point.X >= Left && point.X <= Right &&
                   point.Y >= Bottom && point.Y <= Top &&
                   point.Z >= Back && point.Z <= Front;
        }

        /// <summary>
        ///     Determines if this rectangle intersects with rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public bool Intersects(AxisAlignedBox3 rect)
        {
            return rect.Left < Right && Left < rect.Right &&
                   rect.Bottom < Top && Bottom < rect.Top &&
                   rect.Back < Front && Back < rect.Front;
        }

        /// <summary>
        ///     Returns a AxisAlignedBox3D structure that represents the intersection of two rectangles. If there is no
        ///     intersection, and empty AxisAlignedBox3D is returned.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static AxisAlignedBox3 Intersect(AxisAlignedBox3 a, AxisAlignedBox3 b)
        {
            var x1 = Math.Max(a.Left, b.Left);
            var x2 = Math.Min(a.Right, b.Right);
            var y1 = Math.Max(a.Bottom, b.Bottom);
            var y2 = Math.Min(a.Top, b.Top);
            var z1 = Math.Max(a.Back, b.Back);
            var z2 = Math.Min(a.Front, b.Front);

            if (x2 >= x1 && y2 >= y1 && z2 >= z1)
                return FromExtents(x1, y1, z1, x2, y2, z2);
            return Empty;
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

        /// <summary>
        ///     Gets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public Point3 Maximum
        {
            get { return new Point3(Center.X + ExtentX, Center.Y + ExtentY, Center.Z + ExtentZ); }
        }

        /// <summary>
        ///     Gets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public Point3 Minimum
        {
            get { return new Point3(Center.X - ExtentX, Center.Y - ExtentY, Center.Z - ExtentZ); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return ExtentX <= 0f || ExtentY <= 0f || ExtentZ <= 0f; }
        }

        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point3 Center { get; }

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
        ///     Gets the center Z coord.
        /// </summary>
        /// <value>The Z.</value>
        public float Z
        {
            get { return Center.Z; }
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
        ///     Gets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public float Bottom
        {
            get { return Center.Y - ExtentY; }
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
        ///     Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get { return ExtentX * 2; }
        }

        /// <summary>
        ///     Gets the near.
        /// </summary>
        /// <value>The near.</value>
        public float Back
        {
            get { return Center.Z - ExtentZ; }
        }

        /// <summary>
        ///     Gets the far.
        /// </summary>
        /// <value>The far.</value>
        public float Front
        {
            get { return Center.Z + ExtentZ; }
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
        ///     Gets or sets the Depth.
        /// </summary>
        /// <value>The Depth.</value>
        public float Depth
        {
            get { return ExtentZ * 2; }
        }

        /// <summary>
        ///     Returns an array of 8 corner points, useful for
        ///     collision vs. non-aligned objects.
        /// </summary>
        /// <value>The corners.</value>
        /// <remarks>
        ///     If the order of these corners is important, they are as
        ///     follows: The 4 points of the minimum Z face (note that
        ///     because we use right-handed coordinates, the minimum Z is
        ///     at the 'back' of the box) starting with the minimum point of
        ///     all, then anticlockwise around this face (if you are looking
        ///     onto the face from outside the box). Then the 4 points of the
        ///     maximum Z face, starting with maximum point of all, then
        ///     anticlockwise around this face (looking onto the face from
        ///     outside the box). Like this:
        ///     <pre>
        ///          1-----2
        ///         /|    /|
        ///        / |   / |
        ///       5-----4  |
        ///       |  0--|--3
        ///       | /   | /
        ///       |/    |/
        ///       6-----7
        ///     </pre>
        /// </remarks>
        internal Point3[] Corners
        {
            get
            {
                // return a clone of the array (not the original)
                var vecX = Vector3.XUnit * ExtentX;
                var vecY = Vector3.YUnit * ExtentY;
                var vecZ = Vector3.ZUnit * ExtentZ;

                return new[]
                {
                    Center - vecX - vecY - vecZ,
                    Center + vecX - vecY - vecZ,
                    Center - vecX + vecY - vecZ,
                    Center + vecX + vecY - vecZ,
                    Center - vecX - vecY + vecZ,
                    Center + vecX - vecY + vecZ,
                    Center - vecX + vecY + vecZ,
                    Center + vecX + vecY + vecZ
                };
            }
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

        /// <summary>
        ///     Gets or sets the extent Z.
        /// </summary>
        /// <value>The extent Z.</value>
        public float ExtentZ { get; }

        /// <summary>
        ///     Gets the extent of this box as the distance measured from it's opposite corners.
        /// </summary>
        /// <value>The extent.</value>
        public float Extent
        {
            get
            {
                var xSize = Width;
                var ySize = Height;
                var zSize = Depth;
                return (float)Math.Sqrt(xSize * xSize + ySize * ySize + zSize * zSize);
            }
        }
    }
}