using System;

namespace Core.Algorithm
{
    public struct Point2
    {
        public Point2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X { get; }
        public float Y { get; }
        
        public static bool operator ==(Point2 a, Point2 b)
        {
            return Math.Abs(a.X - b.X) < 0.0001f && 
                   Math.Abs(a.Y - b.Y) < 0.0001f;
        }
        public static bool operator !=(Point2 a, Point2 b)
        {
            return !(a == b);
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
    }
}