using System;
using Core.MathLib;

namespace Core.Fem
{
	public class Element
	{
	    private float[,] _b;
		private int _colorIndex;
		private readonly int[] _conn;
		private float[,] _e; //material
	    private float[,] _k; //stiffness

	    private float[] _stress;
	    private float _vonMises;

	    /// <summary>
		/// Initializes a new instance of the <see cref="Element"/> class.
		/// </summary>
		/// <param name="node1">The node1.</param>
		/// <param name="node2">The node2.</param>
		/// <param name="node3">The node3.</param>
		public Element(int node1, int node2, int node3)
		{
			this._conn = new [] { node1, node2, node3 };
		}

	    /// <summary>
		/// Calcs the elem K.
		/// </summary>
		/// <param name="thickness">The thickness.</param>
		/// <param name="young">The young.</param>
		/// <param name="poisson">The poisson.</param>
		/// <param name="x1">The x1.</param>
		/// <param name="y1">The y1.</param>
		/// <param name="x2">The x2.</param>
		/// <param name="y2">The y2.</param>
		/// <param name="x3">The x3.</param>
		/// <param name="y3">The y3.</param>
		internal void CalcElemK(float thickness, float young, float poisson, float x1, float y1, float x2, float y2, float x3, float y3)
		{
			var b = new float[3, 6];
			b[0, 0] = y2 - y3;
			b[0, 2] = y3 - y1;
			b[0, 4] = y1 - y2;
			b[1, 1] = x3 - x2;
			b[1, 3] = x1 - x3;
			b[1, 5] = x2 - x1;
			b[2, 0] = x3 - x2;
			b[2, 1] = y2 - y3;
			b[2, 2] = x1 - x3;
			b[2, 3] = y3 - y1;
			b[2, 4] = x2 - x1;
			b[2, 5] = y1 - y2;

			float area = Area2D(x1, y1, x2, y2, x3, y3);
			this._b = Matrix.Multiply(1.0f / (2.0f * area), b);

			var e = new float[3, 3];
			e[0, 0] = 1.0f;
			e[0, 1] = poisson;
			e[1, 0] = poisson;
			e[1, 1] = 1.0f;
			e[2, 2] = (1.0f - poisson) / 2.0f;
			this._e = Matrix.Multiply(young / (1.0f - poisson * poisson), e);

			var m1 = Matrix.Transpose(this._b);
			var m2 = Matrix.Multiply(m1, this._e);
			var k = Matrix.Multiply(m2, this._b);

			this._k = Matrix.Multiply(thickness * area, k);
		}

	    /// <summary>
	    ///     Calculate the signed area of the triangle
	    /// </summary>
	    /// <param name="x1">The x1.</param>
	    /// <param name="y1">The y1.</param>
	    /// <param name="x2">The x2.</param>
	    /// <param name="y2">The y2.</param>
	    /// <param name="x3">The x3.</param>
	    /// <param name="y3">The y3.</param>
	    /// <returns>
	    ///     a positive area if all points wind to the right (clockwise),
	    ///     0 if all points are coincident,
	    ///     a negative value if points wind counter clockwise
	    /// </returns>
	    private static float Area2D(float x1, float y1, float x2, float y2, float x3, float y3)
	    {
	        return (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2f;
	    }


        /// <summary>
        /// Calcs the von mises.
        /// </summary>
        /// <param name="localUnknowns">The local unknowns.</param>
        internal void CalcVonMises(float[] localUnknowns)
		{
			this._stress = Matrix.Multiply(this.Material, Matrix.Multiply(this._b, localUnknowns));
			this._vonMises = VonMises3D(this._stress[0], this._stress[1], 0, this._stress[2], 0, 0);
		}

		/// <summary>
		/// Determine the von mises force from the stress and strain
		/// </summary>
		/// <param name="sx">The sx.</param>
		/// <param name="sy">The sy.</param>
		/// <param name="sz">The sz.</param>
		/// <param name="txy">The txy.</param>
		/// <param name="txz">The TXZ.</param>
		/// <param name="tyz">The tyz.</param>
		/// <returns></returns>
		internal static float VonMises3D(float sx, float sy, float sz, float txy, float txz, float tyz)
		{
			var stresses = (sx - sy) * (sx - sy) + (sx - sz) * (sx - sz) + (sy - sz) * (sy - sz);
			var vectors = txy * txy + txz * txz + tyz * tyz;
			return (float)(1.0 / Math.Sqrt(2.0) * Math.Sqrt(stresses + 6.0 * vectors));
		}

	    /// <summary>
		/// Gets or sets the index of the color.
		/// </summary>
		/// <scalar>The index of the color.</scalar>
	    public int ColorIndex
		{
			get
			{
				return this._colorIndex;
			}
			internal set
			{
				this._colorIndex = value;
			}
		}

		/// <summary>
		/// Gets the node list.
		/// </summary>
		/// <scalar>The node list.</scalar>
		public int[] NodeList
		{
			get
			{
				return this._conn;
			}
		}

		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <scalar>The material.</scalar>
		private float[,] Material
		{
			get
			{
				return this._e;
			}
		}

		/// <summary>
		/// Gets the number of nodes.
		/// </summary>
		/// <scalar>The number of nodes.</scalar>
		public int NumberOfNodes
		{
			get
			{
				return this._conn.Length;
			}
		}

		/// <summary>
		/// Gets or sets the stress.
		/// </summary>
		/// <scalar>The stress.</scalar>
		public float[] Stress
		{
			get
			{
				return this._stress;
			}
		}

		/// <summary>
		/// Gets the von mises.
		/// </summary>
		/// <scalar>The von mises.</scalar>
		public float VonMises
		{
			get
			{
				return this._vonMises;
			}
		}

		/// <summary>
		/// Gets the connection to the node.
		/// </summary>
		/// <value>The conn.</value>
		internal int[] Connection
		{
			get { return _conn; }
		}

		/// <summary>
		/// Gets the stiffness.
		/// </summary>
		/// <value>The stiffness.</value>
		internal float[,] Stiffness
		{
			get { return _k; }
		}
	}
}
