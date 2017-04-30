namespace Core.Fem
{
	public class Node
	{
	    private readonly float _positionX;
	    private readonly float _positionY;
	    private float[] _displacement;
		private float[] _stress;
		private float _vonMises;
		private float[] _freedom;
		private bool[] _constrained;
		private int _colorIndex;
		private int _index;
		private float[] _load;

	    /// <summary>
		/// Initializes a new instance of the <see cref="Node"/> class.
		/// </summary>
		/// <param name="x">X.</param>
		/// <param name="y">Y.</param>
		public Node(float x, float y)
		{
			this._positionX = x;
			this._positionY = y;
			this._constrained = null;
			this._displacement = null;
			this._freedom = new float[2];
			this._load = null;
		}

	    /// <summary>
		/// Constrains both x and y.
		/// </summary>
		public void FixAll()
		{
			if (this._constrained == null)
				this._constrained = new bool[3];

			this._constrained[0] = true;
			this._constrained[1] = true;

			if (this._displacement == null)
				this._displacement = new float[3];

			this._displacement[0] = 0;
			this._displacement[1] = 0;
		}

		/// <summary>
		/// Applies the displacement along X.
		/// </summary>
		/// <param name="amount">The amount.</param>
		public void ApplyDisplacementAlongX(float amount)
		{
			if (this._constrained == null)
				this._constrained = new bool[3];

			this._constrained[0] = true;
			
			if (this._displacement == null)
				this._displacement = new float[3];

			this._displacement[0] = amount;
		}

		/// <summary>
		/// Applies the displacement along Y.
		/// </summary>
		/// <param name="amount">The amount.</param>
		public void ApplyDisplacementAlongY(float amount)
		{
			if (this._constrained == null)
				this._constrained = new bool[3];

			this._constrained[1] = true;

			if (this._displacement == null)
				this._displacement = new float[3];
			this._displacement[1] = amount;
		}

		/// <summary>
		/// Applies the load.
		/// </summary>
		/// <param name="loadInX">The load in X.</param>
		/// <param name="loadInY">The load in Y.</param>
		public void ApplyLoad(float loadInX, float loadInY)
		{
			this._load = new [] { loadInX, loadInY };
		}

		/// <summary>
		/// Sets the stress, used by the solver.
		/// </summary>
		/// <param name="sx">The sx.</param>
		/// <param name="sy">The sy.</param>
		/// <param name="sz">The sz.</param>
		public void SetStress(float sx, float sy, float sz)
		{
			this._stress = new [] { sx, sy, sz };
			this._vonMises = Element.VonMises3D(sx, sy, 0f, sz, 0f, 0f);
		}

		/// <summary>
		/// Sets the index.
		/// </summary>
		/// <param name="index">The index.</param>
		internal void SetIndex(int index)
		{
			this._index = index;
		}

		/// <summary>
		/// Sets the freedom.
		/// </summary>
		/// <param name="ux">The ux.</param>
		/// <param name="uy">The uy.</param>
		internal void SetFreedom(float ux, float uy)
		{
			this._freedom = new [] { ux, uy };
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
			set
			{
				this._colorIndex = value;
			}
		}

		/// <summary>
		/// Gets a scalar indicating whether this <see cref="Node"/> is constrained.
		/// </summary>
		/// <scalar><c>true</c> if constrained; otherwise, <c>false</c>.</scalar>
		public bool Constrained
		{
			get
			{
				return this._constrained != null;
			}
		}

		/// <summary>
		/// Gets the constraint.
		/// </summary>
		/// <scalar>The constraint.</scalar>
		public bool[] Constraint
		{
			get
			{
				return this._constrained;
			}
		}

		/// <summary>
		/// Gets the displacement.
		/// </summary>
		/// <scalar>The displacement.</scalar>
		internal float[] Displacement
		{
			get
			{
				return this._displacement;
			}
		}

		/// <summary>
		/// Gets the index.
		/// </summary>
		/// <scalar>The index.</scalar>
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		/// <summary>
		/// Gets the load.
		/// </summary>
		/// <scalar>The load.</scalar>
		public float[] Load
		{
			get
			{
				return this._load;
			}
		}

		/// <summary>
		/// Gets a scalar indicating whether this <see cref="Node"/> is loaded.
		/// </summary>
		/// <scalar><c>true</c> if loaded; otherwise, <c>false</c>.</scalar>
		public bool Loaded
		{
			get
			{
				return this._load != null;
			}
		}
        
		/// <summary>
		/// Gets the stress.
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
		/// Gets the SX.
		/// </summary>
		/// <scalar>The SX.</scalar>
		public float SX
		{
			get
			{
				return this._stress[0];
			}
		}

		/// <summary>
		/// Gets the SY.
		/// </summary>
		/// <scalar>The SY.</scalar>
		public float SY
		{
			get
			{
				return this._stress[1];
			}
		}

		/// <summary>
		/// Gets the UX.
		/// </summary>
		/// <scalar>The UX.</scalar>
		public float UX
		{
			get
			{
				return this._freedom[0];
			}
		}

		/// <summary>
		/// Gets the UY.
		/// </summary>
		/// <scalar>The UY.</scalar>
		public float UY
		{
			get
			{
				return this._freedom[1];
			}
		}

		/// <summary>
		/// Gets or sets the von mises.
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
		/// Gets or sets the X.
		/// </summary>
		/// <scalar>The X.</scalar>
		public float X
		{
			get
			{
				return this._positionX;
			}
		}

		/// <summary>
		/// Gets or sets the Y.
		/// </summary>
		/// <scalar>The Y.</scalar>
		public float Y
		{
			get
			{
				return this._positionY;
			}
		}
	}
}
