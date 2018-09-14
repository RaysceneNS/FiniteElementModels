using System.Collections.Generic;
using System.Diagnostics;

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
                int nodeIdx0 = element.NodeList[0];
                elementStresses[nodeIdx0, 0] += element.Stress[0];
                elementStresses[nodeIdx0, 1] += element.Stress[1];
                elementStresses[nodeIdx0, 2] += element.Stress[2];
                elementCounts[nodeIdx0]++;

                int nodeIdx1 = element.NodeList[1];
                elementStresses[nodeIdx1, 0] += element.Stress[0];
                elementStresses[nodeIdx1, 1] += element.Stress[1];
                elementStresses[nodeIdx1, 2] += element.Stress[2];
                elementCounts[nodeIdx1]++;

                int nodeIdx2 = element.NodeList[2];
                elementStresses[nodeIdx2, 0] += element.Stress[0];
                elementStresses[nodeIdx2, 1] += element.Stress[1];
                elementStresses[nodeIdx2, 2] += element.Stress[2];
                elementCounts[nodeIdx2]++;
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
                var idx = (int) (avg * 255);

                node.ColorIndex = idx;
            }
        }
    }
}
