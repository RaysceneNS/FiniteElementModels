using System;
using System.Diagnostics;
using Core.Geometry;

namespace Core.MathLib
{
    /// <summary>
    ///     Class encapsulating a standard 4x4 homogenous matrix.
    /// </summary>
    /// <remarks>
    ///     The engine uses column vectors when applying matrix multiplications,
    ///     This means a vector is represented as a single column, 4-row
    ///     matrix. This has the effect that the tranformations implemented
    ///     by the matrices happens right-to-left e.g. if vector V is to be
    ///     transformed by M1 then M2 then M3, the calculation would be
    ///     M3 * M2 * M1 * V. The order that matrices are concatenated is
    ///     vital since matrix multiplication is not cummatative, i.e. you
    ///     can get a different result if you concatenate in the wrong order.
    ///     <p />
    ///     The use of column vectors and right-to-left ordering is the
    ///     standard in most mathematical texts, and is the same as used in
    ///     OpenGL. It is, however, the opposite of Direct3D, which has
    ///     inexplicably chosen to differ from the accepted standard and uses
    ///     row vectors and left-to-right matrix multiplication.
    ///     <p />
    ///     The engine deals with the differences between D3D and OpenGL etc.
    ///     internally when operating through different render systems. The engine
    ///     users only need to conform to standard maths conventions, i.e.
    ///     right-to-left matrix multiplication, (The engine transposes matrices it
    ///     passes to D3D to compensate).
    ///     <p />
    ///     The generic form M * V which shows the layout of the matrix
    ///     entries is shown below:
    ///     <p />
    ///     | m[0][0]  m[0][1]  m[0][2]  m[0][3] |   {x}
    ///     | m[1][0]  m[1][1]  m[1][2]  m[1][3] |   {y}
    ///     | m[2][0]  m[2][1]  m[2][2]  m[2][3] |   {z}
    ///     | m[3][0]  m[3][1]  m[3][2]  m[3][3] |   {1}
    /// </remarks>
    public struct Matrix4X4
    {
        internal readonly float M00, M01, M02, M03;
        internal readonly float M10, M11, M12, M13;
        internal readonly float M20, M21, M22, M23;
        private readonly float _m30, _m31, _m32, _m33;

        public static readonly Matrix4X4 Identity = new Matrix4X4(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);

        public static readonly Matrix4X4 Zero = new Matrix4X4(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix4X4" /> class.
        /// </summary>
        /// <param name="m00">The M00.</param>
        /// <param name="m01">The M01.</param>
        /// <param name="m02">The M02.</param>
        /// <param name="m03">The M03.</param>
        /// <param name="m10">The M10.</param>
        /// <param name="m11">The M11.</param>
        /// <param name="m12">The M12.</param>
        /// <param name="m13">The M13.</param>
        /// <param name="m20">The M20.</param>
        /// <param name="m21">The M21.</param>
        /// <param name="m22">The M22.</param>
        /// <param name="m23">The M23.</param>
        /// <param name="m30">The M30.</param>
        /// <param name="m31">The M31.</param>
        /// <param name="m32">The M32.</param>
        /// <param name="m33">The M33.</param>
        internal Matrix4X4(float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            M00 = m00;
            M01 = m01;
            M02 = m02;
            M03 = m03;
            M10 = m10;
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M20 = m20;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            _m30 = m30;
            _m31 = m31;
            _m32 = m32;
            _m33 = m33;
        }

        /// <summary>
        ///     Creates a new matrix. Copy values from a given matrix.
        /// </summary>
        /// <param name="m">The m.</param>
        public Matrix4X4(Matrix4X4 m)
        {
            M00 = m.M00;
            M01 = m.M01;
            M02 = m.M02;
            M03 = m.M03;
            M10 = m.M10;
            M11 = m.M11;
            M12 = m.M12;
            M13 = m.M13;
            M20 = m.M20;
            M21 = m.M21;
            M22 = m.M22;
            M23 = m.M23;
            _m30 = m._m30;
            _m31 = m._m31;
            _m32 = m._m32;
            _m33 = m._m33;
        }

        /// <summary>
        ///     Gets the <see cref="System.float" /> with the specified row.
        /// </summary>
        /// <value></value>
        internal float this[int row, int column]
        {
            get
            {
                Debug.Assert(0 <= column && column < 4);
                Debug.Assert(0 <= row && row < 4);

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
                            case 3:
                                return M03;
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
                            case 3:
                                return M13;
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
                            case 3:
                                return M23;
                        }
                        break;

                    case 3:
                        switch (column)
                        {
                            case 0:
                                return _m30;
                            case 1:
                                return _m31;
                            case 2:
                                return _m32;
                            case 3:
                                return _m33;
                        }
                        break;
                }

                throw new Exception("attempt to access invalid matrix location");
            }
        }

        /// <summary>
        ///     Check whether or not the matrix is affine matrix.
        ///     An affine matrix is a 4x4 matrix with row 3 equal to (0, 0, 0, 1),
        ///     e.g. no projective coefficients.
        /// </summary>
        /// <value><c>true</c> if this instance is affine; otherwise, <c>false</c>.</value>
        public bool IsAffine
        {
            get { return _m30 == 0 && _m31 == 0 && _m32 == 0 && _m33 == 1; }
        }

        /// <summary>
        ///     Returns the linear interpolation between left and right matrices
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static Matrix4X4 Lerp(Matrix4X4 left, Matrix4X4 right, float amount)
        {
            return new Matrix4X4(
                (right.M00 - left.M00) * amount + left.M00, (right.M01 - left.M01) * amount + left.M01,
                (right.M02 - left.M02) * amount + left.M02, (right.M03 - left.M03) * amount + left.M03,
                (right.M10 - left.M10) * amount + left.M10, (right.M11 - left.M11) * amount + left.M11,
                (right.M12 - left.M12) * amount + left.M12, (right.M13 - left.M13) * amount + left.M13,
                (right.M20 - left.M20) * amount + left.M20, (right.M21 - left.M21) * amount + left.M21,
                (right.M22 - left.M22) * amount + left.M22, (right.M23 - left.M23) * amount + left.M23,
                (right._m30 - left._m30) * amount + left._m30, (right._m31 - left._m31) * amount + left._m31,
                (right._m32 - left._m32) * amount + left._m32, (right._m33 - left._m33) * amount + left._m33);
        }


        /// <summary>
        ///     Inverts the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static Matrix4X4 Invert(Matrix4X4 m)
        {
            return m.Adjoint() * (1.0f / m.Determinant);
        }

        /// <summary>
        ///     rotate matrix as defined by the quatnerion
        /// </summary>
        /// <param name="quat">The quat.</param>
        public Matrix4X4 Rotate(Quaternion quat)
        {
            return this * CreateRotation(quat);
        }

        /// <summary>
        ///     Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix4X4 Transpose()
        {
            return new Matrix4X4(
                M00, M10, M20, _m30,
                M01, M11, M21, _m31,
                M02, M12, M22, _m32,
                M03, M13, M23, _m33);
        }

        /// <summary>
        ///     Used to multiply (concatenate) two 4x4 Matrices.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4X4 operator *(Matrix4X4 left, Matrix4X4 right)
        {
            return new Matrix4X4(
                left.M00 * right.M00 + left.M01 * right.M10 + left.M02 * right.M20 + left.M03 * right._m30,
                left.M00 * right.M01 + left.M01 * right.M11 + left.M02 * right.M21 + left.M03 * right._m31,
                left.M00 * right.M02 + left.M01 * right.M12 + left.M02 * right.M22 + left.M03 * right._m32,
                left.M00 * right.M03 + left.M01 * right.M13 + left.M02 * right.M23 + left.M03 * right._m33,
                left.M10 * right.M00 + left.M11 * right.M10 + left.M12 * right.M20 + left.M13 * right._m30,
                left.M10 * right.M01 + left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right._m31,
                left.M10 * right.M02 + left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right._m32,
                left.M10 * right.M03 + left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right._m33,
                left.M20 * right.M00 + left.M21 * right.M10 + left.M22 * right.M20 + left.M23 * right._m30,
                left.M20 * right.M01 + left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right._m31,
                left.M20 * right.M02 + left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right._m32,
                left.M20 * right.M03 + left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right._m33,
                left._m30 * right.M00 + left._m31 * right.M10 + left._m32 * right.M20 + left._m33 * right._m30,
                left._m30 * right.M01 + left._m31 * right.M11 + left._m32 * right.M21 + left._m33 * right._m31,
                left._m30 * right.M02 + left._m31 * right.M12 + left._m32 * right.M22 + left._m33 * right._m32,
                left._m30 * right.M03 + left._m31 * right.M13 + left._m32 * right.M23 + left._m33 * right._m33);
        }

        /// <summary>
        ///     Used to multiply a Matrix4 object by a scalar value..
        /// </summary>
        /// <param name="m">The left.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4X4 operator *(Matrix4X4 m, float scalar)
        {
            return new Matrix4X4(
                m.M00 * scalar, m.M01 * scalar, m.M02 * scalar, m.M03 * scalar,
                m.M10 * scalar, m.M11 * scalar, m.M12 * scalar, m.M13 * scalar,
                m.M20 * scalar, m.M21 * scalar, m.M22 * scalar, m.M23 * scalar,
                m._m30 * scalar, m._m31 * scalar, m._m32 * scalar, m._m33 * scalar);
        }

        /// <summary>
        ///     Used to multiply a Matrix4 object by a scalar value..
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="m">The left.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4X4 operator *(float scalar, Matrix4X4 m)
        {
            return new Matrix4X4(
                m.M00 * scalar, m.M01 * scalar, m.M02 * scalar, m.M03 * scalar,
                m.M10 * scalar, m.M11 * scalar, m.M12 * scalar, m.M13 * scalar,
                m.M20 * scalar, m.M21 * scalar, m.M22 * scalar, m.M23 * scalar,
                m._m30 * scalar, m._m31 * scalar, m._m32 * scalar, m._m33 * scalar);
        }

        /// <summary>
        ///     divides a given matrix with a scalar
        /// </summary>
        /// <param name="source">matrix to divide with scalar</param>
        /// <param name="scalar">to divide matrix with</param>
        /// <returns>source/scalar</returns>
        public static Matrix4X4 operator /(Matrix4X4 source, float scalar)
        {
            return source * (1 / scalar);
        }

        /// <summary>
        ///     Used to add two matrices together.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4X4 operator +(Matrix4X4 left, Matrix4X4 right)
        {
            return new Matrix4X4(
                left.M00 + right.M00, left.M01 + right.M01, left.M02 + right.M02, left.M03 + right.M03,
                left.M10 + right.M10, left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13,
                left.M20 + right.M20, left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23,
                left._m30 + right._m30, left._m31 + right._m31, left._m32 + right._m32, left._m33 + right._m33);
        }


        /// <summary>
        ///     Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4X4 operator -(Matrix4X4 matrix)
        {
            return new Matrix4X4(
                -matrix.M00, -matrix.M01, -matrix.M02, -matrix.M03,
                -matrix.M10, -matrix.M11, -matrix.M12, -matrix.M13,
                -matrix.M20, -matrix.M21, -matrix.M22, -matrix.M23,
                -matrix._m30, -matrix._m31, -matrix._m32, -matrix._m33);
        }

        /// <summary>
        ///     Used to subtract two matrices.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4X4 operator -(Matrix4X4 left, Matrix4X4 right)
        {
            return new Matrix4X4(
                left.M00 - right.M00, left.M01 - right.M01, left.M02 - right.M02, left.M03 - right.M03,
                left.M10 - right.M10, left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13,
                left.M20 - right.M20, left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23,
                left._m30 - right._m30, left._m31 - right._m31, left._m32 - right._m32, left._m33 - right._m33);
        }

        public static bool operator ==(Matrix4X4 left, Matrix4X4 right)
        {
            return left.M00 == right.M00 && left.M01 == right.M01 && left.M02 == right.M02 && left.M03 == right.M03 &&
                   left.M10 == right.M10 && left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 &&
                   left.M20 == right.M20 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 &&
                   left._m30 == right._m30 && left._m31 == right._m31 && left._m32 == right._m32 && left._m33 == right._m33;
        }

        public static bool operator !=(Matrix4X4 left, Matrix4X4 right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Matrix3X3" /> to <see cref="Matrix4X4" />.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Matrix4X4(Matrix3X3 right)
        {
            return new Matrix4X4(
                right.M00, right.M01, right.M02, 0,
                right.M10, right.M11, right.M12, 0,
                right.M20, right.M21, right.M22, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Matrix4X4" /> to <see cref="Matrix3X3" />.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Matrix3X3(Matrix4X4 matrix)
        {
            return new Matrix3X3(
                matrix.M00, matrix.M01, matrix.M02,
                matrix.M10, matrix.M11, matrix.M12,
                matrix.M20, matrix.M21, matrix.M22);
        }

        /// <summary>
        ///     Gets the determinant of this matrix.
        /// </summary>
        /// <value>The determinant.</value>
        public float Determinant
        {
            get
            {
                return
                    M00 *
                    (M11 * (M22 * _m33 - _m32 * M23) - M12 * (M21 * _m33 - _m31 * M23) + M13 * (M21 * _m32 - _m31 * M22)) -
                    M01 *
                    (M10 * (M22 * _m33 - _m32 * M23) - M12 * (M20 * _m33 - _m30 * M23) + M13 * (M20 * _m32 - _m30 * M22)) +
                    M02 *
                    (M10 * (M21 * _m33 - _m31 * M23) - M11 * (M20 * _m33 - _m30 * M23) + M13 * (M20 * _m31 - _m30 * M21)) -
                    M03 *
                    (M10 * (M21 * _m32 - _m31 * M22) - M11 * (M20 * _m32 - _m30 * M22) + M12 * (M20 * _m31 - _m30 * M21));
            }
        }

        /// <summary>
        ///     Used to generate the adjoint of this matrix.  Used internally for <see cref="Invert()" />.
        /// </summary>
        /// <returns>
        ///     The adjoint matrix of the current instance.
        /// </returns>
        private Matrix4X4 Adjoint()
        {
            return new Matrix4X4(
                M11 * (M22 * _m33 - _m32 * M23) - M12 * (M21 * _m33 - _m31 * M23) + M13 * (M21 * _m32 - _m31 * M22),
                -(M01 * (M22 * _m33 - _m32 * M23) - M02 * (M21 * _m33 - _m31 * M23) + M03 * (M21 * _m32 - _m31 * M22)),
                M01 * (M12 * _m33 - _m32 * M13) - M02 * (M11 * _m33 - _m31 * M13) + M03 * (M11 * _m32 - _m31 * M12),
                -(M01 * (M12 * M23 - M22 * M13) - M02 * (M11 * M23 - M21 * M13) + M03 * (M11 * M22 - M21 * M12)),
                -(M10 * (M22 * _m33 - _m32 * M23) - M12 * (M20 * _m33 - _m30 * M23) + M13 * (M20 * _m32 - _m30 * M22)),
                M00 * (M22 * _m33 - _m32 * M23) - M02 * (M20 * _m33 - _m30 * M23) + M03 * (M20 * _m32 - _m30 * M22),
                -(M00 * (M12 * _m33 - _m32 * M13) - M02 * (M10 * _m33 - _m30 * M13) + M03 * (M10 * _m32 - _m30 * M12)),
                M00 * (M12 * M23 - M22 * M13) - M02 * (M10 * M23 - M20 * M13) + M03 * (M10 * M22 - M20 * M12),
                M10 * (M21 * _m33 - _m31 * M23) - M11 * (M20 * _m33 - _m30 * M23) + M13 * (M20 * _m31 - _m30 * M21),
                -(M00 * (M21 * _m33 - _m31 * M23) - M01 * (M20 * _m33 - _m30 * M23) + M03 * (M20 * _m31 - _m30 * M21)),
                M00 * (M11 * _m33 - _m31 * M13) - M01 * (M10 * _m33 - _m30 * M13) + M03 * (M10 * _m31 - _m30 * M11),
                -(M00 * (M11 * M23 - M21 * M13) - M01 * (M10 * M23 - M20 * M13) + M03 * (M10 * M21 - M20 * M11)),
                -(M10 * (M21 * _m32 - _m31 * M22) - M11 * (M20 * _m32 - _m30 * M22) + M12 * (M20 * _m31 - _m30 * M21)),
                M00 * (M21 * _m32 - _m31 * M22) - M01 * (M20 * _m32 - _m30 * M22) + M02 * (M20 * _m31 - _m30 * M21),
                -(M00 * (M11 * _m32 - _m31 * M12) - M01 * (M10 * _m32 - _m30 * M12) + M02 * (M10 * _m31 - _m30 * M11)),
                M00 * (M11 * M22 - M21 * M12) - M01 * (M10 * M22 - M20 * M12) + M02 * (M10 * M21 - M20 * M11));
        }

        /// <summary>
        ///     Creates the translation matrix.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z">Z.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateTranslation(float x, float y, float z)
        {
            return new Matrix4X4(
                1.0f, 0.0f, 0.0f, x,
                0.0f, 1.0f, 0.0f, y,
                0.0f, 0.0f, 1.0f, z,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Gets the translation from the matrix.
        /// </summary>
        /// <returns></returns>
        public Vector3 ExtractTranslation()
        {
            return new Vector3(M03, M13, M23);
        }
        
        /// <summary>
        ///     Gets the position from the matrix.
        /// </summary>
        /// <returns></returns>
        public Point3 ExtractPosition()
        {
            return new Point3(M03, M13, M23);
        }

        /// <summary>
        ///     Sets the translation portion of this matrix.
        /// </summary>
        /// <param name="translation">The translation.</param>
        public Matrix4X4 SetPosition(Point3 translation)
        {
            return new Matrix4X4(M00, M01, M02, translation.X,
                M10, M11, M12, translation.Y,
                M20, M21, M22, translation.Z,
                _m30, _m31, _m32, _m33);
        }

        /// <summary>
        ///     Builds a left-handed perspective projection matrix
        /// </summary>
        /// <param name="width">Width of the view-volume at the near view-plane</param>
        /// <param name="height">Height of the view-volume at the near view-plane.</param>
        /// <param name="near"> Z-value of the near view-plane</param>
        /// <param name="far">Z-value of the far view-plane</param>
        /// <returns>left-handed perspective projection matrix</returns>
        public static Matrix4X4 Perspective(float width, float height, float near, float far)
        {
            if (float.IsPositiveInfinity(far))
                return PerspectiveInfinity(width, height, near);
            return new Matrix4X4(
                2 * near / width, 0, 0, 0,
                0, 2 * near / height, 0, 0,
                0, 0, far / (far - near), 1,
                0, 0, near * far / (near - far), 0);
        }

        /// <summary>
        ///     Builds a left-handed perspective projection matrix where the far plane is set to infinity.
        /// </summary>
        /// <param name="width">Width of the view-volume at the near view-plane</param>
        /// <param name="height">Height of the view-volume at the near view-plane.</param>
        /// <param name="near"> Z-value of the near view-plane</param>
        /// <returns>left-handed perspective projection matrix</returns>
        public static Matrix4X4 PerspectiveInfinity(float width, float height, float near)
        {
            const float epsilon = 0.001f;
            return new Matrix4X4(
                2 * near / width, 0, 0, 0,
                0, 2 * near / height, 0, 0,
                0, 0, 1 - epsilon, 1,
                0, 0, near * (epsilon - 1), 0);
        }

        /// <summary>
        ///     Builds a left-handed perspective projection matrix based on a field of view (FOV).
        /// </summary>
        /// <param name="fovY">Field of view, in the y direction, in radians</param>
        /// <param name="ratio">Aspect ratio, defined as view space height divided by width</param>
        /// <param name="near">Z-value of the near view-plane</param>
        /// <param name="far">Z-value of the far view-plane</param>
        /// <returns>left-handed perspective projection matrix based on a field of view (FOV)</returns>
        public static Matrix4X4 PerspectiveFov(float fovY, float ratio, float near, float far)
        {
            if (float.IsPositiveInfinity(far))
                return PerspectiveFovInfinity(fovY, ratio, near);

            // get the co-tangent of this angle
            var h = (float) (Math.Cos(fovY / 2) / Math.Sin(fovY / 2));
            var w = h / ratio;

            return new Matrix4X4(
                w, 0, 0, 0,
                0, h, 0, 0,
                0, 0, far / (far - near), 1,
                0, 0, -near * far / (far - near), 0);
        }

        /// <summary>
        ///     Creates a left-handed orthogonal matrix.
        /// </summary>
        /// <param name="w">The width of the view volume.</param>
        /// <param name="h">The height of the view volume.</param>
        /// <param name="near">The near clipping plane.</param>
        /// <param name="far">The far clipping plane.</param>
        /// <returns>Orthogonal matrix.</returns>
        public static Matrix4X4 Orthogonal(float w, float h, float near, float far)
        {
            return new Matrix4X4(
                2 / w, 0, 0, 0,
                0, 2 / h, 0, 0,
                0, 0, 1 / (far - near), 0,
                0, 0, near / (near - far), 1);
        }

        /// <summary>
        ///     Builds a left-handed perspective projection matrix based on a field of view (FOV) where the
        ///     far plane is set to infinity.
        /// </summary>
        /// <param name="fovY">Field of view, in the y direction, in radians</param>
        /// <param name="ratio">Aspect ratio, defined as view space height divided by width</param>
        /// <param name="near">Z-value of the near view-plane</param>
        /// <returns>left-handed perspective projection matrix based on a field of view (FOV)</returns>
        public static Matrix4X4 PerspectiveFovInfinity(float fovY, float ratio, float near)
        {
            const float epsilon = 0.001f;

            // get the cotangent of this angle
            var h = (float) (Math.Cos(fovY / 2) / Math.Sin(fovY / 2));
            var w = h / ratio;

            return new Matrix4X4(
                w, 0, 0, 0,
                0, h, 0, 0,
                0, 0, 1 - epsilon, 1,
                0, 0, near * (epsilon - 1), 0);
        }

        /// <summary>
        ///     Builds a left-handed, look-at matrix.
        /// </summary>
        /// <param name="eye">vector that defines the eye point. This value is used in translation.</param>
        /// <param name="at">vector that defines the camera look-at target</param>
        /// <param name="up">vector that defines the current world's up, usually [0, 1, 0]. </param>
        /// <returns>left-handed, look-at matrix</returns>
        public static Matrix4X4 LookAt(Vector3 eye, Vector3 at, Vector3 up)
        {
            var zAxis = Vector3.Normalize(at - eye);
            var xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
            var yAxis = Vector3.Cross(zAxis, xAxis);

            return new Matrix4X4(
                xAxis.X, xAxis.Y, xAxis.Z, 0,
                yAxis.X, yAxis.Y, yAxis.Z, 0,
                zAxis.X, zAxis.Y, zAxis.Z, 0,
                -xAxis * eye, -yAxis * eye, -zAxis * eye, 1);
        }

        /// <summary>
        ///     Creates the rotation matrix.
        /// </summary>
        /// <param name="rotAngleInX">The rot angle in X.</param>
        /// <param name="rotAngleInY">The rot angle in Y.</param>
        /// <param name="rotAngleInZ">The rot angle in Z.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateRotation(float rotAngleInX, float rotAngleInY, float rotAngleInZ)
        {
            var cosX = (float) Math.Cos(rotAngleInX * Math.PI / 180.0);
            var sinX = (float) Math.Sin(rotAngleInX * Math.PI / 180.0);
            var cosY = (float) Math.Cos(rotAngleInY * Math.PI / 180.0);
            var sinY = (float) Math.Sin(rotAngleInY * Math.PI / 180.0);
            var cosZ = (float) Math.Cos(rotAngleInZ * Math.PI / 180.0);
            var sinZ = (float) Math.Sin(rotAngleInZ * Math.PI / 180.0);

            // create the rotation matrix
            return new Matrix4X4(
                cosY * cosZ, -cosY * sinZ, sinY, 0.0f,
                sinX * sinY * cosZ + cosX * sinZ, -(sinX * sinY) * sinZ + cosX * cosZ, -sinX * cosY, 0.0f,
                -(cosX * sinY) * cosZ + sinX * sinZ, cosX * sinY * sinZ + sinX * cosZ, cosX * cosY, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Creates the X rotation matrix.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateXRotation(float angle)
        {
            var cos = (float) Math.Cos(angle * Math.PI / 180.0);
            var sin = (float) Math.Sin(angle * Math.PI / 180.0);

            // create the rotation matrix
            return new Matrix4X4(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, cos, -sin, 0.0f,
                0.0f, sin, cos, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Creates the Y rotation matrix.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateYRotation(float angle)
        {
            var cos = (float) Math.Cos(angle * Math.PI / 180.0);
            var sin = (float) Math.Sin(angle * Math.PI / 180.0);

            // create the rotation matrix
            return new Matrix4X4(
                cos, 0, sin, 0,
                0, 1.0f, 0, 0,
                -sin, 0, cos, 0,
                0, 0, 0, 1.0f);
        }

        /// <summary>
        ///     Creates the Z rotation matrix.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateZRotation(float angle)
        {
            var cos = (float) Math.Cos(angle * Math.PI / 180.0);
            var sin = (float) Math.Sin(angle * Math.PI / 180.0);

            // create the rotation matrix
            return new Matrix4X4(
                cos, -sin, 0.0f, 0.0f,
                sin, cos, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Creates the rotation matrix.
        /// </summary>
        /// <param name="rotAngleInX">The rot angle in X.</param>
        /// <param name="rotAngleInY">The rot angle in Y.</param>
        /// <param name="rotAngleInZ">The rot angle in Z.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateRotation(double rotAngleInX, double rotAngleInY, double rotAngleInZ)
        {
            var cosX = (float) Math.Cos(rotAngleInX * Math.PI / 180.0);
            var sinX = (float) Math.Sin(rotAngleInX * Math.PI / 180.0);
            var cosY = (float) Math.Cos(rotAngleInY * Math.PI / 180.0);
            var sinY = (float) Math.Sin(rotAngleInY * Math.PI / 180.0);
            var cosZ = (float) Math.Cos(rotAngleInZ * Math.PI / 180.0);
            var sinZ = (float) Math.Sin(rotAngleInZ * Math.PI / 180.0);

            // create the rotation matrix
            return new Matrix4X4(
                cosY * cosZ, -cosY * sinZ, sinY, 0.0f,
                sinX * sinY * cosZ + cosX * sinZ, -(sinX * sinY) * sinZ + cosX * cosZ, -sinX * cosY, 0.0f,
                -(cosX * sinY) * cosZ + sinX * sinZ, cosX * sinY * sinZ + sinX * cosZ, cosX * cosY, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Create rotation transform from a quaternion
        /// </summary>
        /// <param name="quat">The quat.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateRotation(Quaternion quat)
        {
            // 9 muls, 15 adds
            var x2 = quat.X + quat.X;
            var y2 = quat.Y + quat.Y;
            var z2 = quat.Z + quat.Z;
            float xx = quat.X * x2, xy = quat.X * y2, xz = quat.X * z2;
            float yy = quat.Y * y2, yz = quat.Y * z2, zz = quat.Z * z2;
            float wx = quat.W * x2, wy = quat.W * y2, wz = quat.W * z2;

            return new Matrix4X4(
                1.0f - (yy + zz), xy + wz, xz - wy, 0.0f,
                xy - wz, 1.0f - (xx + zz), yz + wx, 0.0f,
                xz + wy, yz - wx, 1.0f - (xx + yy), 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Create rotation transform from an axis and rotation
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix4X4 CreateRotation(Vector3 axis, float radians)
        {
            return CreateRotation(new Quaternion(axis, radians).Normalize());
        }

        /// <summary>
        ///     Creates the scaling matrix.
        /// </summary>
        /// <param name="scaleInX">The scale in X.</param>
        /// <param name="scaleInY">The scale in Y.</param>
        /// <param name="scaleInZ">The scale in Z.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateScaling(float scaleInX, float scaleInY, float scaleInZ)
        {
            return new Matrix4X4(
                scaleInX, 0.0f, 0.0f, 0.0f,
                0.0f, scaleInY, 0.0f, 0.0f,
                0.0f, 0.0f, scaleInZ, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Creates the scaling matrix.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public static Matrix4X4 CreateScaling(Vector3 scale)
        {
            return new Matrix4X4(
                scale.X, 0.0f, 0.0f, 0.0f,
                0.0f, scale.Y, 0.0f, 0.0f,
                0.0f, 0.0f, scale.Z, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Create transform from given basis
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        /// <returns></returns>
        public static Matrix4X4 FromBasis(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            // results when m * pt:
            //    ( xAxis * pt.X + yAxis * pt.Y + zAxis * pt.Z )
            return new Matrix4X4(
                xAxis.X, yAxis.X, zAxis.X, 0.0f,
                xAxis.Y, yAxis.Y, zAxis.Y, 0.0f,
                xAxis.Z, yAxis.Z, zAxis.Z, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     Extract the matrix' current basis transform
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        internal void ExtractBasis(out Vector3 xAxis, out Vector3 yAxis, out Vector3 zAxis)
        {
            xAxis = new Vector3(M00, M10, M20);
            yAxis = new Vector3(M01, M11, M21);
            zAxis = new Vector3(M02, M12, M22);
        }

        public override int GetHashCode()
        {
            return M00.GetHashCode() ^ M01.GetHashCode() ^ M02.GetHashCode() ^ M03.GetHashCode()
                   ^ M10.GetHashCode() ^ M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode()
                   ^ M20.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode()
                   ^ _m30.GetHashCode() ^ _m31.GetHashCode() ^ _m32.GetHashCode() ^ _m33.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4X4))
                return false;

            return (Matrix4X4) obj == this;
        }
    }
}