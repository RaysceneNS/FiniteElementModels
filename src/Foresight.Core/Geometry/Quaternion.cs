using System;

namespace Core.Geometry
{
    /// <summary>
    ///     A quaternion is a hypersphere (4D Sphere), it's used when representing rotations in 3D spaces, the advantage of a
    ///     hypersphere
    ///     over implementations that store yaw,pitch and roll is that they do not suffer from gimbal lock, that's a loss of
    ///     one or more
    ///     degrees of freedom when a rotation of 90 degrees is applied to a single axis, it's able to do this as all degrees
    ///     of movement
    ///     are calculated at once.
    /// </summary>
    public struct Quaternion
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Quaternion" /> class.
        /// </summary>
        private Quaternion(float w, float x, float y, float z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Quaternion" /> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        public Quaternion(Vector3 axis, float angle)
        {
            if (!axis.IsZero)
            {
                var halfAngle = angle * 0.5;
                var sinAngle = (float)Math.Sin(halfAngle);

                W = (float) Math.Cos(halfAngle);
                X = (axis.X * sinAngle);
                Y = (axis.Y * sinAngle);
                Z = (axis.Z * sinAngle);
            }
            else
            {
                W = 1f;
                X = 0f;
                Y = 0f;
                Z = 0f;
            }
        }


        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.W == right.W && left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>Quaternion multiplication is NOT commutative i.e. Q1*Q2 != Q2*Q1</remarks>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            var w = b.W * a.W - b.X * a.X - b.Y * a.Y - b.Z * a.Z;
            var x = b.W * a.X + b.X * a.W + b.Y * a.Z - b.Z * a.Y;
            var y = b.W * a.Y + b.Y * a.W + b.Z * a.X - b.X * a.Z;
            var z = b.W * a.Z + b.Z * a.W + b.X * a.Y - b.Y * a.X;
            return new Quaternion(w, x, y, z);
        }

        /// <summary>
        ///     Gets the degrees of rotation.
        /// </summary>
        /// <value>The degrees.</value>
        public float Degrees
        {
            get
            {
                var len = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
                float angle;
                if (!MathCore.EqualityTest(0, len))
                    angle = (float)Math.Acos(W) * 2;
                else
                    angle = 0.0f;
                return angle * 180.0f / (float)Math.PI;
            }
        }

        /// <summary>
        ///     Return the magnitude of all 4 dimensions
        /// </summary>
        /// <value>The magnitude.</value>
        /// <returns></returns>
        private double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z + W * W); }
        }

        /// <summary>
        ///     Gets the axis of rotation.
        /// </summary>
        /// <value>The axis.</value>
        public Vector3 Axis
        {
            get
            {
                Vector3 axis;
                var len = Math.Sqrt(X * X + Y * Y + Z * Z);
                if (!MathCore.EqualityTest(0f, len))
                {
                    axis = new Vector3((float) (X / len), (float) (Y / len), (float) (Z / len)).Normalize();
                }
                else
                {
                    axis = new Vector3(0f, 0f, 1f);
                }
                return axis;
            }
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public float W { get; }

        /// <summary>
        ///     Normalize this intance
        /// </summary>
        public Quaternion Normalize()
        {
            return Normalize(this);
        }

        /// <summary>
        ///     Normalizes the specified quat.
        /// </summary>
        /// <param name="quat">The quat.</param>
        /// <returns></returns>
        private static Quaternion Normalize(Quaternion quat)
        {
            var mag = (float) quat.Magnitude;

            var w = MathCore.Clamp(-1.0f, quat.W / mag, 1.0f);
            var x = MathCore.Clamp(-1.0f, quat.X / mag, 1.0f);
            var y = MathCore.Clamp(-1.0f, quat.Y / mag, 1.0f);
            var z = MathCore.Clamp(-1.0f, quat.Z / mag, 1.0f);
            return new Quaternion(w, x, y, z);
        }

        /// <summary>
        ///     Start up the trackball.  The trackball works by pretending that a ball
        ///     encloses the 3D view.  You roll this pretend ball with the mouse.  For
        ///     example, if you click on the center of the ball and move the mouse straight
        ///     to the right, you roll the ball around its Y-axis.  This produces a Y-axis
        ///     rotation.  You can click on the "edge" of the ball and roll it around
        ///     in a circle to get a Z-axis rotation.
        ///     The math behind the trackball is simple: start with a vector from the first
        ///     mouse-click on the ball to the center of the 3D view.  At the same time, set the radius
        ///     of the ball to be the smaller dimension of the 3D view.  As you drag the mouse
        ///     around in the 3D view, a second vector is computed from the surface of the ball
        ///     to the center.  The axis of rotation is the cross product of these two vectors,
        ///     and the angle of rotation is the angle between the two vectors.
        /// </summary>
        /// <param name="pointX">The point X.</param>
        /// <param name="pointY">The point Y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Vector3 ProjectToTrackball(float pointX, float pointY, float width, float height)
        {
            // Scale so bounds map to [0,0] - [2,2]
            var x = pointX / (width / 2f);
            var y = pointY / (height / 2f);

            // Translate 0,0 to the center
            x = x - 1;

            // Flip so +Y is up instead of down, this is needed to convert window's spatial coordinates to opengl's
            y = 1 - y;

            // Z^2 = 1 - X^2 - Y^2
            var z2 = 1.0 - x * x - y * y;
            var z = z2 > 0 ? (float) Math.Sqrt(z2) : 0.0f;

            return new Vector3(x, y, z);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion))
                return false;
            return (Quaternion) obj == this;
        }
    }
}