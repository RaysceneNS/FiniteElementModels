using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace UI.Controls.Viewport
{
    public class ColorScale
    {
        private const short SHADES = 256;
        private readonly byte[,] _color;

        private ColorScale(params KeyColor[] keyColors)
        {
            _color = new byte[3, SHADES];

            Build(keyColors);
        }
        
        public static int ShadeCount
        {
            get { return SHADES; }
        }

        public Color this[int index]
        {
            get { return Color.FromArgb(_color[0, index], _color[1, index], _color[2, index]); }
        }

        private void Build(IReadOnlyList<KeyColor> keys)
        {
            var keysCount = (short) keys.Count;

            // if reverse then reorder the color values from back to front...
            for (var i = 0; i < keysCount - 1; i++)
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
    }
}