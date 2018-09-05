namespace Core.Geometry
{
    public struct Size2
    {
        public Size2(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }

        public static bool operator ==(Size2 sz1, Size2 sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }
        public static bool operator !=(Size2 sz1, Size2 sz2)
        {
            return !(sz1 == sz2);
        }

        public Point2 Center
        {
            get { return new Point2(Width / 2f, Height / 2f); }
        }
        public float Width { get; }
        public float Height { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Size2))
                return false;

            var ef = (Size2) obj;
            return ef.Width == Width && ef.Height == Height && ef.GetType().Equals(GetType());
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}