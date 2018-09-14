namespace Core.Fea
{
    public class Node
    {
        public Node(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public void FixAll()
        {
            this.ConstraintX = true;
            this.ConstraintY = true;

            this.DisplacementX = 0;
            this.DisplacementY = 0;
        }

        public void ApplyLoad(float loadInX, float loadInY)
        {
            this.LoadX = loadInX;
            this.LoadY = loadInY;
            Loaded = true;
        }

        internal void SetStress(float sx, float sy, float sz)
        {
            this.Stress = new [] { sx, sy, sz };
            this.VonMises = Element.VonMises3D(sx, sy, 0f, sz, 0f, 0f);
        }

        internal void SetIndex(int index)
        {
            this.Index = index;
        }

        internal void SetFreedom(float ux, float uy)
        {
            FreedomX = ux;
            FreedomY = uy;
        }

        public int ColorIndex { get; set; }
        public int Index { get; private set; }

        public bool Constrained
        {
            get { return this.ConstraintX || ConstraintY; }
        }

        public bool ConstraintX { get; private set; }
        public bool ConstraintY { get; private set; }

        public float DisplacementX { get; private set; }
        public float DisplacementY { get; private set; }


        public float LoadX { get; private set; }
        public float LoadY { get; private set; }
        public bool Loaded { get; private set; }

        public float[] Stress { get; private set; }
        
        public float FreedomX { get; private set; }
        public float FreedomY { get; private set; }

        public float VonMises { get; private set; }

        public float X { get; }
        public float Y { get; }
    }
}
