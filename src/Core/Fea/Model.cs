using System;
using System.Collections.Generic;
using Core.Algorithm;

namespace Core.Fea
{
    public class Model
    {
        public Model(int numNodes, int numElements)
            : base()
        {
            this.Nodes = new List<Node>(numNodes);
            this.Elements = new List<Element>(numElements);
            this.Edges = new LinkedList<ElementEdge>();
        }

        public LinkedList<ElementEdge> Edges { get; }
        public List<Node> Nodes { get; }
        public List<Element> Elements { get; }

        public bool IsSolved { get; private set; }
        
        public float MaxNodeValue { get; private set; }
        public float MinNodeValue { get; private set; }

        public Node MaxNode { get; private set; }
        public Node MinNode { get; private set; }
        
        public void ComputeEdges()
        {
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
                var attachCount = elementCounts[node.Index];
                node.SetStress(elementStresses[node.Index, 0] / attachCount, elementStresses[node.Index, 1] / attachCount, elementStresses[node.Index, 2] / attachCount);

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
                if (idx < 0)
                    idx = 0;
                node.ColorIndex = idx;
            }
        }

        public void Solve(Progress<TaskProgress> progressReport)
        {
            IsSolved = false;
            new PlanarStressSolver(this, 10, 30000, 0.25f).SolvePlaneStress(progressReport);
            IsSolved = true;
        }
    }
}
