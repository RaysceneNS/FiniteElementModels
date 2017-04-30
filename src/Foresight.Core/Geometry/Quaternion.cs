using System;
using System.Diagnostics;
using Core.MathLib;

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
    [DebuggerDisplay("x={X} y={Y} z={Z} w={_w}")]
    public struct Quaternion
    {
        /// <summary>
        ///     the component vectors for this quaternion
        /// </summary>
        private readonly float _w;
        private readonly float _x;
        private readonly float _y;
        private readonly float _z;

        /// <summary>
        ///     Identity quaternion
        /// </summary>
        public static readonly Quaternion Identity = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        ///     Empty quaternion
        /// </summary>
        public static readonly Quaternion Zero = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Quaternion" /> class.
        /// </summary>
        /// <param name="w">W.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z">Z.</param>
        private Quaternion(float w, float x, float y, float z)
        {
            _w = w;
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Quaternion" /> class.
        /// </summary>
        /// <param name="q">The q.</param>
        public Quaternion(Quaternion q)
        {
            _w = q._w;
            _x = q.X;
            _y = q.Y;
            _z = q.Z;
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

                _w = (float) Math.Cos(halfAngle);
                _x = (axis.X * sinAngle);
                _y = (axis.Y * sinAngle);
                _z = (axis.Z * sinAngle);
            }
            else
            {
                _w = 1f;
                _x = 0f;
                _y = 0f;
                _z = 0f;
            }
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Quaternion" /> to <see cref="Matrix4X4" />.
        /// </summary>
        /// <param name="quaternion">The quaternion.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Matrix4X4(Quaternion quaternion)
        {
            return Matrix4X4.CreateRotation(quaternion);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left._w == right._w && left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     divide quaternion by a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static Quaternion operator /(Quaternion a, float divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Dividing quaternion by zero");

            var inv = 1.0f / divisor;
            return new Quaternion(a.W * inv, a.X * inv, a.Y * inv, a.Z * inv);
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
            var w = b._w * a._w - b.X * a.X - b.Y * a.Y - b.Z * a.Z;
            var x = b._w * a.X + b.X * a._w + b.Y * a.Z - b.Z * a.Y;
            var y = b._w * a.Y + b.Y * a._w + b.Z * a.X - b.X * a.Z;
            var z = b._w * a.Z + b.Z * a._w + b.X * a.Y - b.Y * a.X;
            return new Quaternion(w, x, y, z);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="quat">The quat.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(Quaternion quat, Vector3 vector)
        {
            // nVidia SDK implementation
            Vector3 uv, uuv;
            var qvec = new Vector3(quat.X, quat.Y, quat.Z);

            uv = qvec.Cross(vector);
            uuv = qvec.Cross(uv);
            uv *= 2.0f * quat._w;
            uuv *= 2.0f;

            return vector + uv + uuv;
        }

        /// <summary>
        ///     Used when a float scalar is multiplied by a Quaternion.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Quaternion operator *(float scalar, Quaternion right)
        {
            return new Quaternion(scalar * right._w, scalar * right.X, scalar * right.Y, scalar * right.Z);
        }

        /// <summary>
        ///     Used when a Quaternion is multiplied by a float scalar.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The result of the operator.</returns>
        public static Quaternion operator *(Quaternion left, float scalar)
        {
            return new Quaternion(scalar * left._w, scalar * left.X, scalar * left.Y, scalar * left.Z);
        }

        /// <summary>
        ///     Used when a Quaternion is added to another Quaternion.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return new Quaternion(left._w + right._w, left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        ///     subtract two quaternions
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.W - right.W, left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        ///     Negates a Quaternion, which simply returns a new Quaternion
        ///     with all components negated.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Quaternion operator -(Quaternion right)
        {
            return new Quaternion(-right._w, -right.X, -right.Y, -right.Z);
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
                    angle = (float)Math.Acos(_w) * 2;
                else
                    angle = 0.0f;

                return (angle * 180.0f / (float)Math.PI);
            }
        }

        /// <summary>
        ///     Return the magnitude of all 4 dimensions
        /// </summary>
        /// <value>The magnitude.</value>
        /// <returns></returns>
        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z + _w * _w); }
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

        /// <summary>
        ///     Gets the X.
        /// </summary>
        /// <value>The X.</value>
        public float X
        {
            get { return _x; }
        }

        /// <summary>
        ///     Gets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public float Y
        {
            get { return _y; }
        }

        /// <summary>
        ///     Gets the Z.
        /// </summary>
        /// <value>The Z.</value>
        public float Z
        {
            get { return _z; }
        }

        /// <summary>
        ///     Gets the W.
        /// </summary>
        /// <value>The W.</value>
        public float W
        {
            get { return _w; }
        }

        /// <summary>
        ///     Get the conjugate of the specified quat.
        /// </summary>
        /// <param name="quat">The quat.</param>
        /// <returns></returns>
        public static Quaternion Conjugate(Quaternion quat)
        {
            return new Quaternion(quat._w, -quat.X, -quat.Y, -quat.Z);
        }

        /// <summary>
        ///     Get the conjugate of this quaternion
        /// </summary>
        public Quaternion Conjugate()
        {
            return Conjugate(this);
        }

        /// <summary>
        ///     Returns a new quaternion that is the inverse of this instance
        /// </summary>
        /// <returns></returns>
        public Quaternion Invert()
        {
            return Invert(this);
        }

        /// <summary>
        ///     Inverts the specified quaternion.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <returns></returns>
        public static Quaternion Invert(Quaternion q)
        {
            var magnitudeSquared = q.X * q.X + q.Y * q.Y + q.Z * q.Z + q._w * q._w;
            if (magnitudeSquared != 0.0f)
            {
                var inverse = 1.0f / magnitudeSquared;
                return new Quaternion(q._w * inverse, q.X * -inverse, q.Y * -inverse, q.Z * -inverse);
            }
            return Zero;
        }

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
        public static Quaternion Normalize(Quaternion quat)
        {
            var mag = (float) quat.Magnitude;

            float w, x, y, z;
            w = MathCore.Clamp(-1.0f, quat._w / mag, 1.0f);
            x = MathCore.Clamp(-1.0f, quat.X / mag, 1.0f);
            y = MathCore.Clamp(-1.0f, quat.Y / mag, 1.0f);
            z = MathCore.Clamp(-1.0f, quat.Z / mag, 1.0f);
            return new Quaternion(w, x, y, z);
        }
        
        /// <summary>
        ///     Calculate the dot product (inner product) between this quaternion and the other
        /// </summary>
        /// <param name="quat">The quat.</param>
        /// <returns></returns>
        public float DotProduct(Quaternion quat)
        {
            return DotProduct(this, quat);
        }

        /// <summary>
        ///     Calculate the dot product (inner product) between two quaternions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DotProduct(Quaternion a, Quaternion b)
        {
            return a.X * b.X +
                   a.Y * b.Y +
                   a.Z * b.Z +
                   a.W * b.W;
        }

        /// <summary>
        ///     Spherical cubic interpolation between a and b.
        /// </summary>
        /// <param name="a">The start quaternion.</param>
        /// <param name="b">The end quaternion.</param>
        /// <param name="ta">The tangent for point a. Can be calculated by Spline.</param>
        /// <param name="tb">The tangent for point b. Can be calculated by Spline.</param>
        /// <param name="t">The interpolation time [0..1].</param>
        /// <returns>The interpolated quaternion.</returns>
        public static Quaternion Squad(Quaternion a, Quaternion b, Quaternion ta, Quaternion tb, float t)
        {
            var slerpT = 2.0f * t * (1.0f - t);
            var p = Slerp(a, b, t);
            var q = Slerp(ta, tb, t);
            return Slerp(p, q, slerpT);
        }

        /// <summary>
        ///     Get the spherically interpolated quaternion at position t between a and b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <param name="t">The blend factor.</param>
        /// <returns>A smooth interpolation between the give quaternions</returns>
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
        {
            // Calculate the cosine of the angle between the two
            float fScale0, fScale1;
            double dCos = DotProduct(a, b);

            // If the angle is significant, use the spherical interpolation
            if (1.0 - Math.Abs(dCos) > 1e-6f)
            {
                var dTemp = Math.Acos(Math.Abs(dCos));
                var dSin = Math.Sin(dTemp);
                fScale0 = (float) (Math.Sin((1.0 - t) * dTemp) / dSin);
                fScale1 = (float) (Math.Sin(t * dTemp) / dSin);
            }
            // Else use the cheaper linear interpolation
            else
            {
                fScale0 = 1.0f - t;
                fScale1 = t;
            }

            if (dCos < 0.0)
                fScale1 = -fScale1;

            // Return the interpolated result
            return a * fScale0 + b * fScale1;
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

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ _w.GetHashCode();
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        ///     true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion))
                return false;

            return (Quaternion) obj == this;
        }
    }
}