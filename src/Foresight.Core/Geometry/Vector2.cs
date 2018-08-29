using System;
using System.Diagnostics;
using Core.MathLib;

namespace Core.Geometry
{
    [DebuggerDisplay("x={_x} y={_y}")]
    public struct Vector2
    {
        private readonly float _x, _y;

        /// <summary>
        ///     Vector2D(0,0)
        /// </summary>
        private static readonly Vector2 Empty = new Vector2(0f, 0f);

        /// <summary>
        ///     X-axis unit vector (1,0)
        /// </summary>
        public static readonly Vector2 XUnit = new Vector2(1f, 0f);

        /// <summary>
        ///     Y-axis unit vector (0,1)
        /// </summary>
        public static readonly Vector2 YUnit = new Vector2(0f, 1f);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Vector2(float x, float y)
        {
            this._x = x;
            this._y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2" /> class.
        /// </summary>
        /// <param name="vec"></param>
        public Vector2(Vector2 vec)
        {
            _x = vec._x;
            _y = vec._y;
        }

        /// <summary>
        ///     Resize the vector to unit length.
        /// </summary>
        public Vector2 Normalize()
        {
            return Normalize(this);
        }
        
        /// <summary>
        ///     This returns the Normalized Vector2D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector2D to be Normalized.</param>
        /// <returns>The Normalized Vector2D. (Unit Vector)</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector" />
        /// </remarks>
        public static Vector2 Normalize(Vector2 source)
        {
            var magnitude = (float) source.Magnitude;

            if (magnitude <= float.Epsilon)
                return Empty;
            var magnitudeInv = 1.0f / magnitude;
            return new Vector2(source._x * magnitudeInv, source._y * magnitudeInv);
        }

        /// <summary>
        ///     Creates a Vector2D With the given length (<see cref="Magnitude" />) and the given Angle.
        /// </summary>
        /// <param name="length">The length (<see cref="Magnitude" />) of the Vector2D to be created</param>
        /// <param name="radianAngle">The angle of the from the XAxis) in Radians</param>
        /// <returns>
        ///     a Vector2D With the given length and angle.
        /// </returns>
        /// <remarks>
        ///     <code>FromLengthAndAngle(1,Math.PI/2)</code> would create a Vector2D equal to <code>new Vector2D(0,1)</code>.
        ///     And <code>FromLengthAndAngle(1,0)</code> would create a Vector2D equal to <code>new Vector2D(1,0)</code>.
        /// </remarks>
        public static Vector2 FromLengthAndAngle(double length, double radianAngle)
        {
            return new Vector2(
                (float) (length * Math.Cos(radianAngle)),
                (float) (length * Math.Sin(radianAngle)));
        }

        /// <summary>
        ///     Gets a Vector2D that is perpendicular(orthogonal) to the passed Vector2D while staying on the XY Plane.
        /// </summary>
        /// <param name="source">The Vector2D whose perpendicular(orthogonal) is to be determined.</param>
        /// <returns>An perpendicular(orthogonal) Vector2D using the Right Hand Rule</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Right-hand_rule" />
        /// </remarks>
        public static Vector2 GetRightHandNormal(Vector2 source)
        {
            return new Vector2(-source.Y, source.X);
        }

        /// <summary>
        ///     Gets a Vector2D that is perpendicular(orthogonal) to the passed Vector2D while staying on the XY Plane.
        /// </summary>
        /// <param name="source">The Vector2D whose perpendicular(orthogonal) is to be determined.</param>
        /// <returns>An perpendicular(orthogonal) Vector2D using the Left Hand Rule (opposite of the Right hand Rule)</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Right-hand_rule#Left-hand_rule" />
        /// </remarks>
        public static Vector2 GetLeftHandNormal(Vector2 source)
        {
            return new Vector2(source.Y, -source.X);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Point2" /> to <see cref="Vector2" />.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector2(Point2 f)
        {
            return new Vector2(f.X, f.Y);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Point2" /> to <see cref="Vector2" />.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        /// <summary>
        ///     Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator -(Vector2 left)
        {
            return new Vector2(-left._x, -left._y);
        }

        /// <summary>
        ///     Returns true if the vector's scalar components are all smaller
        ///     that the ones of the vector it is compared against.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Vector2 left, Vector2 right)
        {
            return left._x > right._x && left._y > right._y;
        }

        /// <summary>
        ///     Returns true if the vector's scalar components are all greater
        ///     that the ones of the vector it is compared against.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Vector2 left, Vector2 right)
        {
            return left._x < right._x && left._y < right._y;
        }

        /// <summary>
        ///     Subtracts 2 Vector2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Difference of the 2 Vector2Ds.</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction" />
        /// </remarks>
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///     Subtracts a scalar valaue from both components of this vector
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 left, float right)
        {
            return new Vector2(left._x - right, left._y - right);
        }

        /// <summary>
        ///     Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Sum of the 2 Vector2Ds.</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction" />
        /// </remarks>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///     Adds a scalar component to this vector
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 left, float right)
        {
            return new Vector2(left.X + right, left.Y + right);
        }

        /// <summary>
        ///     Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The Product of the Scaler Multiplication.
        /// </returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#float_multiplication" />
        /// </remarks>
        public static Vector2 operator *(Vector2 source, float scalar)
        {
            return new Vector2(source.X * scalar, source.Y * scalar);
        }

        /// <summary>
        ///     Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <returns>
        ///     The Product of the Scaler Multiplication.
        /// </returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#float_multiplication" />
        /// </remarks>
        public static Vector2 operator *(float scalar, Vector2 source)
        {
            return new Vector2(scalar * source.X, scalar * source.Y);
        }

        /// <summary>
        ///     scalar product
        /// </summary>
        /// <param name="a">first vector</param>
        /// <param name="b">second vector</param>
        /// <returns>scalar product</returns>
        public static float operator *(Vector2 a, Vector2 b)
        {
            return a._x * b._x + a._y * b._y;
        }

        /// <summary>
        ///     matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator *(Matrix3X3 matrix, Vector2 vector)
        {
            var inverseZ = 1.0 / (vector.X * matrix[2, 0] + vector.Y * matrix[2, 1] + matrix[2, 2]);
            return new Vector2(
                (float) (matrix[0, 0] * vector._x + matrix[0, 1] * vector._y + matrix[0, 2] * inverseZ),
                (float) (matrix[1, 0] * vector._x + matrix[1, 1] * vector._y + matrix[1, 2] * inverseZ));
        }

        /// <summary>
        ///     matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator *(Vector2 vector, Matrix3X3 matrix)
        {
            var inverseZ = 1.0 / (vector.X * matrix[2, 0] + vector.Y * matrix[2, 1] + matrix[2, 2]);
            return new Vector2(
                (float) (matrix[0, 0] * vector._x + matrix[0, 1] * vector._y + matrix[0, 2] * inverseZ),
                (float) (matrix[1, 0] * vector._x + matrix[1, 1] * vector._y + matrix[1, 2] * inverseZ));
        }

        /// <summary>
        ///     Used to divide a vector by a scalar.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 left, float scalar)
        {
            Debug.Assert(scalar != 0.0f, "Cannot divide a Vector2 by zero.");

            // get the inverse of the scalar up front to avoid doing multiple divides later
            var inverse = 1.0f / scalar;
            return new Vector2(left._x * inverse, left._y * inverse);
        }

        /// <summary>
        ///     Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Z value of the resulting vector.</returns>
        /// <remarks>
        ///     This 2D Cross Product is using a cheat. Since the Cross product (in 3D space)
        ///     always generates a vector perpendicular (orthogonal) to the 2 vectors used as
        ///     arguments. The cheat is that the only vector that can be perpendicular to two
        ///     vectors in the XY Plane will parallel to the Z Axis. Since any vector that is
        ///     parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        ///     the cross product of 2 vectors in the XY plane as single scalar: Z. Also the
        ///     Cross Product of and Vector on the XY plan and that of one ont on the Z Axis
        ///     will result in a vector on the XY Plane. So the ZCross Methods were well thought
        ///     out and can be trusted.
        ///     <seealso href="http://en.wikipedia.org/wiki/Cross_product" />
        /// </remarks>
        public static float ZCross(Vector2 left, Vector2 right)
        {
            return left._x * right._y - left._y * right._x;
        }

        /// <summary>
        ///     Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public float ZCross(Vector2 right)
        {
            return _x * right._y - _y * right._x;
        }

        /// <summary>
        ///     Crosses this instance.
        /// </summary>
        /// <returns></returns>
        public Vector2 Cross()
        {
            return new Vector2(_y, -_x);
        }

        /// <summary>
        ///     Generates a vector perpendicular to this vector (eg an 'up' vector).
        ///     This method will return a vector which is perpendicular to this
        ///     vector. There are an infinite number of possibilities but this
        ///     method will guarantee to generate one of them. If you need more
        ///     control you should use the Quaternion class.
        /// </summary>
        /// <returns></returns>
        public static Vector2 Perpendicular(Vector2 vector)
        {
            return new Vector2(-vector._y, vector._x);
        }

        /// <summary>
        ///     Generates a vector perpendicular to this vector (eg an 'up' vector).
        /// </summary>
        /// <returns></returns>
        public Vector2 Perpendicular()
        {
            return Perpendicular(this);
        }

        /// <summary>
        ///     Calculates a reflection vector to the plane with the given normal .
        ///     assumes 'this' is pointing AWAY FROM the plane, invert if it is not.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="normal">The normal.</param>
        /// <returns></returns>
        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            return new Vector2(vector - 2 * vector.Dot(normal) * normal);
        }

        public Vector2 Reflect(Vector2 normal)
        {
            return Reflect(this, normal);
        }

        /// <summary>
        ///     Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Dot_product" />
        /// </remarks>
        public static float Dot(Vector2 left, Vector2 right)
        {
            return left.Y * right.Y + left.X * right.X;
        }

        /// <summary>
        ///     Performs a Dot Product operation on 2 vectors, which produces the angle between them.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>The angle between the 2 vectors.</returns>
        public float Dot(Vector2 v)
        {
            return _x * v._x + _y * v._y;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return MathCore.EqualityTest(a._x, b._x) && MathCore.EqualityTest(a._y, b._y);
        }


        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2))
                return false;

            return (Vector2) obj == this;
        }

        public override int GetHashCode()
        {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }


        /// <summary>
        ///     Determines the current angle in radians of the Vector2D and Returns it.
        /// </summary>
        /// <param name="source">The Vector2D of whos angle is to be Determined.</param>
        /// <returns>
        ///     The angle in radians of the Vector2D.
        /// </returns>
        public static double GetAngle(Vector2 source)
        {
            var result = Math.Atan2(source.Y, source.X);
            if (result < 0)
                result += Math.PI * 2;
            return result;
        }

        /// <summary>
        ///     Inverts the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public static Vector2 Invert(Vector2 vector)
        {
            return new Vector2(-vector._x, -vector._y);
        }

        /// <summary>
        ///     Projects the left Vector onto the Right Vector.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Projected Vector2D.</returns>
        /// <remarks>
        ///     <seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29" />
        /// </remarks>
        public static Vector2 Project(Vector2 left, Vector2 right)
        {
            var tmp = Dot(left, right) / (right.X * right.X + right.Y * right.Y);
            return right * tmp;
        }

        /// <summary>
        ///     Creates a vector from the subtraction of two points
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Vector2 FromPoints(Point2 a, Point2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        ///     Returns the linear blended interpolation of the 2 input vectors
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <param name="blend">The blend.</param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
        {
            return new Vector2(
                blend * (b._x - a._x) - a._x,
                blend * (b._y - a._y) - a._y);
        }

        /// <summary>
        ///     Interpolate 3 vectors using barycentric coordinates
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <param name="c">The c.</param>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        /// <returns>a when u=0 v=0, b when u=1 v=0, c when u=0 v=1, and a linear combination otherwise</returns>
        public static Vector2 BaryCentric(Vector2 a, Vector2 b, Vector2 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }

        /// <summary>
        ///     Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="value3">The value3.</param>
        /// <param name="value4">The value4.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>A vector that is the result of the Catmull-Rom interpolation</returns>
        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            var squared = amount * amount;
            var cubed = amount * squared;

            return new Vector2(
                0.5f * (2.0f * value2.X + (-value1.X + value3.X) * amount +
                        (2.0f * value1.X - 5.0f * value2.X + 4.0f * value3.X - value4.X) * squared +
                        (-value1.X + 3.0f * value2.X - 3.0f * value3.X + value4.X) * cubed),
                0.5f * (2.0f * value2.Y + (-value1.Y + value3.Y) * amount +
                        (2.0f * value1.Y - 5.0f * value2.Y + 4.0f * value3.Y - value4.Y) * squared +
                        (-value1.Y + 3.0f * value2.Y - 3.0f * value3.Y + value4.Y) * cubed)
            );
        }

        /// <summary>
        ///     Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position vector.</param>
        /// <param name="tangent1">First source tangent vector.</param>
        /// <param name="value2">Second source position vector.</param>
        /// <param name="tangent2">Second source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>
        ///     The result of the Hermite spline interpolation.
        /// </returns>
        public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
        {
            var squared = amount * amount;
            var cubed = amount * squared;
            var part1 = 2.0f * cubed - 3.0f * squared + 1.0f;
            var part2 = -2.0f * cubed + 3.0f * squared;
            var part3 = cubed - 2.0f * squared + amount;
            var part4 = cubed - squared;

            return new Vector2(
                value1.X * part1 + value2.X * part2 + tangent1.X * part3 + tangent2.X * part4,
                value1.Y * part1 + value2.Y * part2 + tangent1.Y * part3 + tangent2.Y * part4);
        }

        /// <summary>
        ///     Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end" />.</param>
        /// <returns>
        ///     The cubic interpolation of the two vectors.
        /// </returns>
        public static Vector2 SmoothStep(Vector2 start, Vector2 end, float amount)
        {
            amount = amount > 1.0f ? 1.0f : (amount < 0.0f ? 0.0f : amount);
            amount = amount * amount * (3.0f - .02f * amount);

            return new Vector2(
                start.X + (end.X - start.X) * amount,
                start.Y + (end.Y - start.Y) * amount);
        }

        /// <summary>
        ///     returns the vector with the least magnitude
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return a.Magnitude <= b.Magnitude ? a : b;
        }

        /// <summary>
        ///     returns the vector with the greatest magnitude
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns></returns>
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return a.Magnitude >= b.Magnitude ? a : b;
        }
        
        /// <summary>
        ///     Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public float Y
        {
            get { return _y; }
        }

        /// <summary>
        ///     Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public float X
        {
            get { return _x; }
        }

        /// <summary>
        ///     Get the magnitude (length) of this vector
        /// </summary>
        public double Magnitude
        {
            get { return Math.Sqrt(_x * _x + _y * _y); }
        }

        /// <summary>
        ///     Is the length of this vector zero?
        /// </summary>
        public bool IsZero
        {
            get { return MathCore.EqualityTest(0.0, Magnitude); }
        }

        /// <summary>
        ///     Is the length of this vector 1?
        /// </summary>
        public bool IsUnit
        {
            get { return MathCore.EqualityTest(1.0, Magnitude); }
        }
    }
}