using System.Collections.Generic;
using Core.Geometry;

namespace Core.Fea
{
    public class Model
    {
        public Model(int numNodes, int numElements)
            : base()
        {
            this.Nodes = new List<Node>(numNodes);
            this.Elements = new List<Element>(numElements);
        }

        public LinkedList<ElementEdge> Edges { get; private set; }

        public List<Node> Nodes { get; }

        public List<Element> Elements { get; }

        /// <summary>
        /// Calculates the entity's bounding box.
        /// </summary>
        /// <returns></returns>
        public AxisAlignedBox3 AxisAlignedBoundingBox()
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var minZ = float.MinValue;
            var maxZ = float.MinValue;

            foreach (var node2D in this.Nodes)
            {
                if (node2D.X < minX)
                    minX = node2D.X;
                if (node2D.X > maxX)
                    maxX = node2D.X;
                if (node2D.Y < minY)
                    minY = node2D.Y;
                if (node2D.Y > maxY)
                    maxY = node2D.Y;
                minZ = 0f;
                maxZ = 0f;
            }

            return AxisAlignedBox3.FromExtents(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public bool IsSolved { get; set; }

        public int ElementCount
        {
            get
            {
                return this.Elements.Count;
            }
        }
        
        public int NodeCount
        {
            get
            {
                return this.Nodes.Count;
            }
        }

        public float MaxNodeValue { get; private set; }

        public float MinNodeValue { get; private set; }

        public Node MaxNode { get; private set; }

        public Node MinNode { get; private set; }

        public Node Node(int index)
        {
            return this.Nodes[index];
        }

        public Element Element(int index)
        {
            return this.Elements[index];
        }

        public void AddNode(Node node)
        {
            this.Nodes.Add(node);
        }

        public void AddElement(Element element)
        {
            this.Elements.Add(element);
        }
        
        public void ComputeEdges()
        {
            //determine the boundary polygon by eliminating any triangles that share edges with another
            if (this.Edges != null)
            {
                return;
            }

            this.Edges = new LinkedList<ElementEdge>();
            foreach (var element in this.Elements)
            {
                var cn1 = element.NodeList[0];
                var cn2 = element.NodeList[1];
                var cn3 = element.NodeList[2];

                var edgeNode = this.Edges.Find(new ElementEdge(cn2, cn1));
                if (edgeNode != null)
                    this.Edges.Remove(edgeNode);
                else
                    this.Edges.AddLast(new ElementEdge(cn1, cn2));

                edgeNode = this.Edges.Find(new ElementEdge(cn3, cn2));
                if (edgeNode != null)
                    this.Edges.Remove(edgeNode);
                else
                    this.Edges.AddLast(new ElementEdge(cn2, cn3));

                edgeNode = this.Edges.Find(new ElementEdge(cn1, cn3));
                if (edgeNode != null)
                    this.Edges.Remove(edgeNode);
                else
                    this.Edges.AddLast(new ElementEdge(cn3, cn1));
            }
        }
        
        public void PlotAverageVonMises()
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            var elementStresses = new float[this.Nodes.Count, 3];
            var elementCounts = new int[this.Nodes.Count];

            foreach (var element in this.Elements)
            {
                for (var j = 0; j < element.NumberOfNodes; j++)
                {
                    elementStresses[element.NodeList[j], 0] += element.Stress[0];
                    elementStresses[element.NodeList[j], 1] += element.Stress[1];
                    elementStresses[element.NodeList[j], 2] += element.Stress[2];
                    elementCounts[element.NodeList[j]]++;
                }
            }

            foreach (var node in this.Nodes)
            {
                node.SetStress(elementStresses[node.Index, 0] / elementCounts[node.Index], elementStresses[node.Index, 1] / elementCounts[node.Index], elementStresses[node.Index, 2] / elementCounts[node.Index]);

                if (node.VonMises < min)
                {
                    min = node.VonMises;
                    this.MinNode = node;
                }

                if (node.VonMises > max)
                {
                    max = node.VonMises;
                    this.MaxNode = node;
                }
            }

            MinNodeValue = min;
            MaxNodeValue = max;
            var range = MaxNodeValue - MinNodeValue;

            foreach (var node in this.Nodes)
            {
                var avg = (node.VonMises - min) / range;
                node.ColorIndex = MathCore.Clamp(0, (int)(avg * 255), 255);
            }
        }

        public struct ElementEdge
        {
            internal ElementEdge(int v1, int v2)
            {
                this.V1 = v1;
                this.V2 = v2;
            }

            public int V1 { get; }
            public int V2 { get; }
        }
    }
}
