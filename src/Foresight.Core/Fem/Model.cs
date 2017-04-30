using System;
using System.Collections.Generic;
using Core.Geometry;
using Core.MathLib;

namespace Core.Fem
{
	public class Model
	{
		private bool _isSolved;
		private readonly List<Element> _elements;
		private readonly List<Node> _nodes;
	    private LinkedList<ElementEdge> _edges;
	    private float _minNodeValue, _maxNodeValue;
        private Node _minNode, _maxNode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numNodes">The num nodes.</param>
        /// <param name="numElements">The num elements.</param>
        public Model(int numNodes, int numElements)
			: base()
		{
			this._nodes = new List<Node>(numNodes);
			this._elements = new List<Element>(numElements);
		}

	    public LinkedList<ElementEdge> Edges
        {
	        get { return _edges; }
	    }

	    public List<Node> Nodes
	    {
	        get { return _nodes; }
	    }

	    public List<Element> Elements
	    {
	        get { return _elements; }
	    }

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

			foreach (var node2D in this._nodes)
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
        
		/// <summary>
		/// Gets or sets a scalar indicating whether this instance is solved.
		/// </summary>
		/// <scalar><c>true</c> if this instance is solved; otherwise, <c>false</c>.</scalar>
		public bool IsSolved
		{
			get
			{
				return this._isSolved;
			}
			set
			{
				this._isSolved = value;
			}
		}
        
		/// <summary>
		/// Gets the d elements.
		/// </summary>
		/// <scalar>The d elements.</scalar>
		public int NumElements
		{
			get
			{
				return this._elements.Count;
			}
		}
        
        /// <summary>
		/// Gets the node count.
		/// </summary>
		/// <scalar>The number of nodes.</scalar>
		public int NumNodes
		{
			get
			{
				return this._nodes.Count;
			}
		}

	    public float MaxNodeValue
	    {
	        get { return _maxNodeValue; }
	    }

	    public float MinNodeValue
	    {
	        get { return _minNodeValue; }
	    }

	    public Node MaxNode
	    {
	        get { return _maxNode; }
	    }

	    public Node MinNode
	    {
	        get { return _minNode; }
	    }

	    /// <summary>
		/// Nodes the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public Node Node(int index)
		{
			return this._nodes[index];
		}

		/// <summary>
		/// Elements the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public Element Element(int index)
		{
			return this._elements[index];
		}

		/// <summary>
		/// Adds the node.
		/// </summary>
		/// <param name="node">The node.</param>
		public void AddNode(Node node)
		{
			this._nodes.Add(node);
		}

		/// <summary>
		/// Adds the element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void AddElement(Element element)
		{
			this._elements.Add(element);
		}
        
	    public void ComputeEdges()
	    {
	        //determine the boundary polygon by eliminating any triangles that share edges with another
	        if (this._edges != null)
	        {
	            return;
	        }

	        this._edges = new LinkedList<ElementEdge>();
	        foreach (var element in this._elements)
	        {
	            var cn1 = element.NodeList[0];
	            var cn2 = element.NodeList[1];
	            var cn3 = element.NodeList[2];

	            var edgeNode = this._edges.Find(new ElementEdge(cn2, cn1));
	            if (edgeNode != null)
	                this._edges.Remove(edgeNode);
	            else
	                this._edges.AddLast(new ElementEdge(cn1, cn2));

	            edgeNode = this._edges.Find(new ElementEdge(cn3, cn2));
	            if (edgeNode != null)
	                this._edges.Remove(edgeNode);
	            else
	                this._edges.AddLast(new ElementEdge(cn2, cn3));

	            edgeNode = this._edges.Find(new ElementEdge(cn1, cn3));
	            if (edgeNode != null)
	                this._edges.Remove(edgeNode);
	            else
	                this._edges.AddLast(new ElementEdge(cn3, cn1));
	        }
	    }

	    public void PlotDisplacement()
	    {
	        var min = float.MaxValue;
	        var max = float.MinValue;
            foreach (var node in this.Nodes)
	        {
	            var displacementSquared = node.UX * node.UX + node.UY * node.UY;

	            if (displacementSquared < min)
	            {
	                min = displacementSquared;
	                this._minNode = node;
	            }

	            if (displacementSquared > max)
	            {
	                max = displacementSquared;
	                this._maxNode = node;
	            }
	        }

	        _minNodeValue = (float)Math.Sqrt(min);
	        _maxNodeValue = (float)Math.Sqrt(max);
	        var range = _maxNodeValue - _minNodeValue;

            foreach (var node in this.Nodes)
	        {
	            var displacement = Math.Sqrt(node.UX * node.UX + node.UY * node.UY);
	            var percent = (displacement - min) / range;
	            node.ColorIndex = MathCore.Clamp(0, (int)(percent * 255), 255);
	        }
        }

	    public void PlotVonMises()
	    {
	        var min = float.MaxValue;
	        var max = float.MinValue;
            //determine the min and max extents of the values for this plot
            foreach (var element in this.Elements)
	        {
	            if (element.VonMises < min)
	            {
	                min = element.VonMises;
	                this._minNode = this.Nodes[element.NodeList[0]];
	            }

	            if (element.VonMises > max)
	            {
	                max = element.VonMises;
	                this._maxNode = this.Nodes[element.NodeList[0]];
	            }
	        }

	        //update the legend if needed
	        _minNodeValue = min;
	        _maxNodeValue = max;
	        var range = _maxNodeValue - _minNodeValue;

            //set the color i for each element acccording to it's scalar relative to min/max
            foreach (var element in this.Elements)
	        {
	            var vonMise = (element.VonMises - min) / range;
	            element.ColorIndex = MathCore.Clamp(0, (int)(vonMise * 255), 255);
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
	                this._minNode = node;
	            }

	            if (node.VonMises > max)
	            {
	                max = node.VonMises;
	                this._maxNode = node;
	            }
	        }

	        _minNodeValue = min;
	        _maxNodeValue = max;
	        var range = _maxNodeValue - _minNodeValue;

	        foreach (var node in this.Nodes)
	        {
	            var avgVonMise = (node.VonMises - min) / range;
	            node.ColorIndex = MathCore.Clamp(0, (int)(avgVonMise * 255), 255);
	        }
        }

        public struct ElementEdge
	    {
	        private readonly int _v1;
	        private readonly int _v2;

	        internal ElementEdge(int v1, int v2)
	        {
	            this._v1 = v1;
	            this._v2 = v2;
	        }

	        public int V1
	        {
	            get { return _v1; }
	        }

	        public int V2
	        {
	            get { return _v2; }
	        }
        }
	}

}
