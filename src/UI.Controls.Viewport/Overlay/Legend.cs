using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Geometry;

namespace UI.Controls.Viewport.Overlay
{
    public class Legend : OverlayBase
    {
        private const int NUMBER_OF_VALUES = 10;
        private readonly Font _titleFont;
        private readonly Font _valueFont;
        private readonly ColorScale _colorScale;
        private float _minValue, _maxValue;
        private string _title;

        internal Legend()
        {
            _minValue = 0f;
            _maxValue = 100f;
            _colorScale = ColorScale.Gradient;
            _title = string.Empty;
            _valueFont = new Font("Tahoma", 8.25f, FontStyle.Regular);
            _titleFont = new Font("MS Sans Serif", 8.25f, FontStyle.Bold);
        }

        public void SetRange(string title, float max, float min)
        {
            _title = title;
            _maxValue = max;
            _minValue = min;
            SetDirty();
        }

        protected override Bitmap CreateImage()
        {
            const int titleSpacingY = 15;
            const int labelOffsetX = 22;
            const int barSize = 16;
            const int border = 4;
            const int barOffsetX = 2;

            float TOLERANCE = 0.00001f;
            var format = Math.Abs(_maxValue - _minValue - 100) > TOLERANCE ? "0.000E+00" : "0";
            var maxWidth = float.MinValue;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                var ef = graphics.MeasureString(_title, _titleFont);
                if (ef.Width + border > maxWidth)
                {
                    maxWidth = ef.Width + border;
                }

                for (var i = NUMBER_OF_VALUES; i >= 0; i--)
                {
                    var val = _minValue + (NUMBER_OF_VALUES - i) * (_maxValue - _minValue) / NUMBER_OF_VALUES;
                    ef = graphics.MeasureString(string.Format(format, val), _valueFont);
                    if (border + ef.Width + labelOffsetX + border > maxWidth)
                    {
                        maxWidth = border + ef.Width + labelOffsetX + border;
                    }
                }
            }

            var length = ColorScale.ShadeCount - 1;

            var newImage = new Bitmap((int) maxWidth + border * 2,
                border * 2 + titleSpacingY + (NUMBER_OF_VALUES + 1) * barSize);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.Clear(Color.FromArgb(0xcc, 0xff, 0xff, 0xff));

                using (var textBrush = new SolidBrush(Color.Black))
                {
                    graphics.DrawString(_title, _titleFont, textBrush, border, border);
                    for (var i = NUMBER_OF_VALUES; i >= 0; i--)
                    {
                        var y = titleSpacingY + barSize * i;
                        var val = _minValue + (NUMBER_OF_VALUES - i) * (_maxValue - _minValue) / NUMBER_OF_VALUES;
                        graphics.DrawString(val.ToString(format), _valueFont, textBrush, labelOffsetX + border, y + border);
                    }
                }

                var myColors = new Color[NUMBER_OF_VALUES + 1];
                var myPositions = new float[NUMBER_OF_VALUES + 1];
                for (var i = 0; i <= NUMBER_OF_VALUES; i++)
                {
                    var index = MathCore.Clamp(0, length - (int) (i * length / (float) NUMBER_OF_VALUES), length);
                    myColors[i] = _colorScale[index];
                    myPositions[i] = i / (float) NUMBER_OF_VALUES;
                }

                var colorBlend = new ColorBlend
                {
                    Colors = myColors,
                    Positions = myPositions
                };
                var rect = Rectangle.FromLTRB(border + barOffsetX, border + titleSpacingY - 1,
                    border + barOffsetX + barSize, border + titleSpacingY + barSize * NUMBER_OF_VALUES + barSize);
                using (var brush = new LinearGradientBrush(rect, myColors[myColors.Length - 1], myColors[0],
                    LinearGradientMode.Vertical))
                {
                    brush.InterpolationColors = colorBlend;
                    graphics.FillRectangle(brush, rect);
                }
            }

            newImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            return newImage;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _valueFont?.Dispose();
                _titleFont?.Dispose();
            }
        }
    }
}