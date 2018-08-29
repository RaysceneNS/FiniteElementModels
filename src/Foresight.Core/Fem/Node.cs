namespace Core.Fem
{
    public class Node
    {
        private float[] _freedom;

        public Node(float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.Constraint = null;
            this.Displacement = null;
            this._freedom = new float[2];
            this.Load = null;
        }

        public void FixAll()
        {
            if (this.Constraint == null)
                this.Constraint = new bool[3];

            this.Constraint[0] = true;
            this.Constraint[1] = true;

            if (this.Displacement == null)
                this.Displacement = new float[3];

            this.Displacement[0] = 0;
            this.Displacement[1] = 0;
        }

        public void ApplyDisplacementAlongX(float amount)
        {
            if (this.Constraint == null)
                this.Constraint = new bool[3];

            this.Constraint[0] = true;
            
            if (this.Displacement == null)
                this.Displacement = new float[3];

            this.Displacement[0] = amount;
        }

        public void ApplyDisplacementAlongY(float amount)
        {
            if (this.Constraint == null)
                this.Constraint = new bool[3];

            this.Constraint[1] = true;

            if (this.Displacement == null)
                this.Displacement = new float[3];
            this.Displacement[1] = amount;
        }

        public void ApplyLoad(float loadInX, float loadInY)
        {
            this.Load = new [] { loadInX, loadInY };
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
            this._freedom = new [] { ux, uy };
        }

        public int ColorIndex { get; set; }

        public bool Constrained
        {
            get
            {
                return this.Constraint != null;
            }
        }

        public bool[] Constraint { get; private set; }
        internal float[] Displacement { get; private set; }
        public int Index { get; private set; }
        public float[] Load { get; private set; }

        public bool Loaded
        {
            get
            {
                return this.Load != null;
            }
        }
        
        public float[] Stress { get; private set; }

        public float SX
        {
            get
            {
                return this.Stress[0];
            }
        }

        public float SY
        {
            get
            {
                return this.Stress[1];
            }
        }

        public float UX
        {
            get
            {
                return this._freedom[0];
            }
        }

        public float UY
        {
            get
            {
                return this._freedom[1];
            }
        }

        public float VonMises { get; private set; }

        public float X { get; }

        public float Y { get; }
    }
}
