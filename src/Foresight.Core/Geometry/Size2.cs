using System.ComponentModel;
using System.Diagnostics;

namespace Core.Geometry
{
    [DebuggerDisplay("{_width} x {_height}")]
    public struct Size2
    {
        private readonly float _width;
        private readonly float _height;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Size2" /> struct.
        /// </summary>
        /// <param name="size">The size.</param>
        public Size2(Size2 size)
        {
            _width = size._width;
            _height = size._height;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Size2" /> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size2(float width, float height)
        {
            this._width = width;
            this._height = height;
        }
        
        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The result of the operator.</returns>
        public static Size2 operator *(Size2 sz, float factor)
        {
            return Multiply(sz, factor);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>The result of the operator.</returns>
        public static Size2 operator *(float factor, Size2 sz)
        {
            return Multiply(factor, sz);
        }

        /// <summary>
        ///     Implements the operator /.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>The result of the operator.</returns>
        public static Size2 operator /(Size2 sz, float divisor)
        {
            return Divide(sz, divisor);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>The result of the operator.</returns>
        public static Size2 operator +(Size2 sz1, Size2 sz2)
        {
            return Add(sz1, sz2);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>The result of the operator.</returns>
        public static Size2 operator -(Size2 sz1, Size2 sz2)
        {
            return Subtract(sz1, sz2);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Size2 sz1, Size2 sz2)
        {
            return sz1._width == sz2._width && sz1._height == sz2._height;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Size2 sz1, Size2 sz2)
        {
            return !(sz1 == sz2);
        }

        /// <summary>
        ///     Used to negate the size.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <returns>The result of the operator.</returns>
        public static Size2 operator -(Size2 sz)
        {
            return new Size2(-sz._width, -sz._height);
        }

        /// <summary>
        ///     Gets the center of this size as width/2 & height /2.
        /// </summary>
        /// <value>The center.</value>
        public Point2 Center
        {
            get { return new Point2(_width / 2f, _height / 2f); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool IsEmpty
        {
            get { return _width == 0f && _height == 0f; }
        }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get { return _width; }
        }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get { return _height; }
        }

        /// <summary>
        ///     Divides the specified sz.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns></returns>
        public static Size2 Divide(Size2 sz, float divisor)
        {
            return new Size2(sz._width / divisor, sz._height / divisor);
        }

        /// <summary>
        ///     Multiplies the specified sz.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <param name="factor">The factor.</param>
        /// <returns></returns>
        public static Size2 Multiply(Size2 sz, float factor)
        {
            return new Size2(sz._width * factor, sz._height * factor);
        }

        /// <summary>
        ///     Multiplies the specified sz.
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <param name="sz">The sz.</param>
        /// <returns></returns>
        public static Size2 Multiply(float factor, Size2 sz)
        {
            return new Size2(sz._width * factor, sz._height * factor);
        }

        /// <summary>
        ///     Adds the specified SZ1.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns></returns>
        public static Size2 Add(Size2 sz1, Size2 sz2)
        {
            return new Size2(sz1._width + sz2._width, sz1._height + sz2._height);
        }

        /// <summary>
        ///     Subtracts the specified SZ1.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns></returns>
        public static Size2 Subtract(Size2 sz1, Size2 sz2)
        {
            return new Size2(sz1._width - sz2._width, sz1._height - sz2._height);
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
            if (!(obj is Size2))
                return false;

            var ef = (Size2) obj;
            return ef._width == _width && ef._height == _height && ef.GetType().Equals(GetType());
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return "{Width=" + _width + ", Height=" + _height + "}";
        }
    }
}