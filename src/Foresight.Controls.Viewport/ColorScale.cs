using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace UI.Controls.Viewport
{
    public struct ColorScale
    {
        private const short SHADES = 256;
        private readonly byte[,] _color;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorScale" /> class.
        /// </summary>
        /// <param name="keyColors">The key colors.</param>
        private ColorScale(params KeyColor[] keyColors)
        {
            _color = new byte[3, SHADES];

            Build(keyColors);
        }
        
        /// <summary>
        ///     Gets the shades.
        /// </summary>
        /// <value>The shades.</value>
        public static int ShadeCount
        {
            get { return SHADES; }
        }

        /// <summary>
        ///     Color indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Color this[int index]
        {
            get { return Color.FromArgb(_color[0, index], _color[1, index], _color[2, index]); }
        }

        /// <summary>
        ///     Build, interpolate the colors that fall between the fixed node values
        /// </summary>
        private void Build(IReadOnlyList<KeyColor> keys)
        {
            var nbNode = (short) keys.Count;
            Debug.Assert(nbNode >= 2);

            // if reverse then reorder the color values from back to front...
            for (short i = 0; i < nbNode - 1; i++)
            {
                var x1 = keys[i].Position;
                var x2 = keys[i + 1].Position;
                Debug.Assert(x1 < x2);

                var r1 = keys[i].Color.R;
                var r2 = keys[i + 1].Color.R;
                var g1 = keys[i].Color.G;
                var g2 = keys[i + 1].Color.G;
                var b1 = keys[i].Color.B;
                var b2 = keys[i + 1].Color.B;

                var xDist = (float) (x2 - x1);
                var ra = (r2 - r1) / xDist;
                var rb = r1 - ra * x1;
                var ga = (g2 - g1) / xDist;
                var gb = g1 - ga * x1;
                var ba = (b2 - b1) / xDist;
                var bb = b1 - ba * x1;

                for (int j = x1; j <= x2; j++)
                {
                    //_color[0, SHADES - j - 1] = (byte)(ra * (float)j + rb);
                    _color[0, j] = (byte) (ra * j + rb);
                    _color[1, j] = (byte) (ga * j + gb);
                    _color[2, j] = (byte) (ba * j + bb);
                }
            }
        }
        
        /// <summary>
        ///     Gets the rainbow.
        /// </summary>
        /// <value>The rainbow.</value>
        public static ColorScale Gradient
        {
            get
            {
                return new ColorScale(
                    new KeyColor(0, Color.Blue),
                    new KeyColor(85, Color.Cyan),
                    new KeyColor(170, Color.Yellow),
                    new KeyColor(255, Color.Red));
            }
        }

        private struct KeyColor
        {
            public KeyColor(short position, Color color)
            {
                Position = position;
                Color = color;
            }

            internal short Position { get; }

            internal Color Color { get; }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ColorScale left, ColorScale right)
        {
            var equal = true;
            for (var i = 0; i < 256; i++)
            {
                if (left._color[0, i] != right._color[0, i] ||
                    left._color[1, i] != right._color[1, i] ||
                    left._color[2, i] != right._color[2, i])
                {
                    equal = false;
                    break;
                }
            }
            return equal;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ColorScale left, ColorScale right)
        {
            return !(left == right);
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
            var hash = 0;
            for (var i = 0; i < 256; i++)
            {
                hash ^= _color[0, i] ^ _color[1, i] ^ _color[2, i];
            }
            return hash;
        }

        /// <summary>
        ///     Compares this to another object.  This should be done because the
        ///     equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        ///     true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ColorScale))
                return false;

            return (ColorScale) obj == this;
        }
    }
}