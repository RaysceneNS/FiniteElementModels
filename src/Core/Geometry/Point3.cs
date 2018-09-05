namespace Core.Geometry
{
    public struct Point3
    {
        public static readonly Point3 Origin;
        
        /// <summary>
        ///     Construct a point from 3d coordinates
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z">Z.</param>
        public Point3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            return MathCore.EqualityTest(a.X, b.X) && MathCore.EqualityTest(a.Y, b.Y) && MathCore.EqualityTest(a.Z, b.Z);
        }
        public static bool operator !=(Point3 a, Point3 b)
        {
            return !(a == b);
        }
        
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        
        public override bool Equals(object obj)
        {
            if (!(obj is Point3))
                return false;
            return (Point3) obj == this;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
    }
}