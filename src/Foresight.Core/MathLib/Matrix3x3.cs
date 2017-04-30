using System;
using System.Diagnostics;
using System.Text;

namespace Core.MathLib
{
    public struct Matrix3X3
    {
        // this implementation has been chosen over an array as this is a struct and as such the parameterless constructor would have left us with a null array
        internal readonly float M00;
        internal readonly float M01, M02;
        internal readonly float M10;
        internal readonly float M11, M12;
        internal readonly float M20, M21, M22;

        /// <summary>
        ///     The identity matrix
        /// </summary>
        public static readonly Matrix3X3 Identity = new Matrix3X3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        ///     The zero matrix
        /// </summary>
        public static readonly Matrix3X3 Zero = new Matrix3X3(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3X3" /> class.
        /// </summary>
        /// <param name="m00">The M00.</param>
        /// <param name="m01">The M01.</param>
        /// <param name="m02">The M02.</param>
        /// <param name="m10">The M10.</param>
        /// <param name="m11">The M11.</param>
        /// <param name="m12">The M12.</param>
        /// <param name="m20">The M20.</param>
        /// <param name="m21">The M21.</param>
        /// <param name="m22">The M22.</param>
        internal Matrix3X3(
            float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22)
        {
            M00 = m00;
            M01 = m01;
            M02 = m02;
            M10 = m10;
            M11 = m11;
            M12 = m12;
            M20 = m20;
            M21 = m21;
            M22 = m22;
        }

        /// <summary>
        ///     Creates a new matrix. Copy values from a given matrix.
        /// </summary>
        /// <param name="m"></param>
        internal Matrix3X3(Matrix3X3 m)
        {
            M00 = m.M00;
            M01 = m.M01;
            M02 = m.M02;
            M10 = m.M10;
            M11 = m.M11;
            M12 = m.M12;
            M20 = m.M20;
            M21 = m.M21;
            M22 = m.M22;
        }

        /// <summary>
        ///     Returns an inverted matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix3X3 Inverse()
        {
            return Adjoint() * (1.0f / Determinant);
        }

        /// <summary>
        ///     Used to generate the adjoint of this matrix.  Used internally for <see cref="Inverse" />.
        /// </summary>
        /// <returns>
        ///     The adjoint matrix of the current instance.
        /// </returns>
        private Matrix3X3 Adjoint()
        {
            return new Matrix3X3(
                M11 * M22 - M12 * M21, -(M01 * M22 - M02 * M21), M01 * M12 - M02 * M11,
                -(M10 * M22 - M12 * M20), M00 * M22 - M02 * M20, -(M00 * M12 - M02 * M10),
                M10 * M21 - M11 * M20, -(M00 * M21 - M01 * M20), M00 * M11 - M01 * M10);
        }

        /// <summary>
        ///     Returns the determinant of this matrix
        ///     rule of Sarrus
        /// </summary>
        /// <value>The determinant.</value>
        /// <returns></returns>
        public float Determinant
        {
            get
            {
                return
                    M00 * (M11 * M22 - M12 * M21) +
                    M01 * (M12 * M20 - M10 * M22) +
                    M02 * (M10 * M21 - M11 * M20);
            }
        }

        /// <summary>
        ///     Swaps the rows and columns of this matrix and returns the result
        /// </summary>
        /// <returns></returns>
        public Matrix3X3 Transpose()
        {
            return new Matrix3X3(
                M00, M10, M20,
                M01, M11, M21,
                M02, M12, M22);
        }

        /// <summary>
        ///     Multiply (concatenate) two Matrix3X3 instances together.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix3X3 operator *(Matrix3X3 left, Matrix3X3 right)
        {
            return new Matrix3X3(
                left.M00 * right.M00 + left.M01 * right.M10 + left.M02 * right.M20,
                left.M00 * right.M01 + left.M01 * right.M11 + left.M02 * right.M21,
                left.M00 * right.M02 + left.M01 * right.M12 + left.M02 * right.M22,
                left.M10 * right.M00 + left.M11 * right.M10 + left.M12 * right.M20,
                left.M10 * right.M01 + left.M11 * right.M11 + left.M12 * right.M21,
                left.M10 * right.M02 + left.M11 * right.M12 + left.M12 * right.M22,
                left.M20 * right.M00 + left.M21 * right.M10 + left.M22 * right.M20,
                left.M20 * right.M01 + left.M21 * right.M11 + left.M22 * right.M21,
                left.M20 * right.M02 + left.M21 * right.M12 + left.M22 * right.M22);
        }

        /// <summary>
        ///     Multiplies all the items in the Matrix3X3 by a scalar value.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix3X3 operator *(Matrix3X3 matrix, float scalar)
        {
            return new Matrix3X3(
                matrix.M00 * scalar, matrix.M01 * scalar, matrix.M02 * scalar,
                matrix.M10 * scalar, matrix.M11 * scalar, matrix.M12 * scalar,
                matrix.M20 * scalar, matrix.M21 * scalar, matrix.M22 * scalar);
        }

        /// <summary>
        ///     Multiplies all the items in the Matrix3X3 by a scalar value.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix3X3 operator *(float scalar, Matrix3X3 matrix)
        {
            return new Matrix3X3(
                matrix.M00 * scalar, matrix.M01 * scalar, matrix.M02 * scalar,
                matrix.M10 * scalar, matrix.M11 * scalar, matrix.M12 * scalar,
                matrix.M20 * scalar, matrix.M21 * scalar, matrix.M22 * scalar);
        }

        /// <summary>
        ///     Used to add two matrices together.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix3X3 operator +(Matrix3X3 left, Matrix3X3 right)
        {
            return new Matrix3X3(
                left.M00 + right.M00, left.M01 + right.M01, left.M02 + right.M02,
                left.M10 + right.M10, left.M11 + right.M11, left.M12 + right.M12,
                left.M20 + right.M20, left.M21 + right.M21, left.M22 + right.M22);
        }

        /// <summary>
        ///     Used to subtract two matrices.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix3X3 operator -(Matrix3X3 left, Matrix3X3 right)
        {
            return new Matrix3X3(
                left.M00 - right.M00, left.M01 - right.M01, left.M02 - right.M02,
                left.M10 - right.M10, left.M11 - right.M11, left.M12 - right.M12,
                left.M20 - right.M20, left.M21 - right.M21, left.M22 - right.M22);
        }

        /// <summary>
        ///     Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix3X3 operator -(Matrix3X3 matrix)
        {
            return new Matrix3X3(
                -matrix.M00, -matrix.M01, -matrix.M02,
                -matrix.M10, -matrix.M11, -matrix.M12,
                -matrix.M20, -matrix.M21, -matrix.M22);
        }

        /// <summary>
        ///     Test two matrices for (value) equality
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Matrix3X3 left, Matrix3X3 right)
        {
            return left.M00 == right.M00 && left.M01 == right.M01 && left.M02 == right.M02 &&
                   left.M10 == right.M10 && left.M11 == right.M11 && left.M12 == right.M12 &&
                   left.M20 == right.M20 && left.M21 == right.M21 && left.M22 == right.M22;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Matrix3X3 left, Matrix3X3 right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Gets the <see cref="System.float" /> with the specified row.
        /// </summary>
        /// <value></value>
        internal float this[int row, int column]
        {
            get
            {
                Debug.Assert(0 <= column && column < 3);
                Debug.Assert(0 <= row && row < 3);

                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0:
                                return M00;
                            case 1:
                                return M01;
                            case 2:
                                return M02;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0:
                                return M10;
                            case 1:
                                return M11;
                            case 2:
                                return M12;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0:
                                return M20;
                            case 1:
                                return M21;
                            case 2:
                                return M22;
                        }
                        break;
                }

                throw new Exception("attempt to access invalid matrix location");
            }
        }

        /// <summary>
        ///     Overrides the Object.ToString() method to provide a text representation of
        ///     a Matrix4.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat(" | {0} {1} {2} |\r\n", M00, M01, M02);
            builder.AppendFormat(" | {0} {1} {2} |\r\n", M10, M11, M12);
            builder.AppendFormat(" | {0} {1} {2} |", M20, M21, M22);

            return builder.ToString();
        }

        /// <summary>
        ///     Provides a unique hash code based on the member variables of this
        ///     class.  This should be done because the equality operators (==, !=)
        ///     have been overriden by this class.
        ///     <p />
        ///     The standard implementation is a simple XOR operation between all local
        ///     member variables.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            // There are probably better distributions out there than this one...
            return M00.GetHashCode() ^ M01.GetHashCode() ^ M02.GetHashCode()
                   ^ M10.GetHashCode() ^ M11.GetHashCode() ^ M12.GetHashCode()
                   ^ M20.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode();
        }

        /// <summary>
        ///     Compares this Matrix to another object.  This should be done because the
        ///     equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        ///     true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix3X3))
                return false;

            return (Matrix3X3) obj == this;
        }
    }
}