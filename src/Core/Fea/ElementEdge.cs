using System;

namespace Core.Fea
{
    public struct ElementEdge : IEquatable<ElementEdge>
    {
        internal ElementEdge(int v1, int v2)
        {
            this.V1 = v1;
            this.V2 = v2;
        }

        public int V1 { get; }
        public int V2 { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is ElementEdge))
                return false;
            ElementEdge other = (ElementEdge)obj;
            return V1 == other.V1 && V2 == other.V2;
        }

        public bool Equals(ElementEdge other)
        {
            return V1 == other.V1 && V2 == other.V2;
        }
    }
}
