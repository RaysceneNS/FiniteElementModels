using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.MathLib;

namespace UI.Controls.Viewport.Overlay
{
    public class Legend : OverlayBase
    {
        private const int NUMBER_OF_VALUES = 10;
        private readonly Font _titleFont;
        private readonly Font _valueFont;
        private ColorScale _colorScale;
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

        public ColorScale ColorScale
        {
            get { return _colorScale; }
            set
            {
                _colorScale = value;
                SetDirty();
            }
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                SetDirty();
            }
        }

        public float MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                SetDirty();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                SetDirty();
            }
        }

        protected override Bitmap CreateImage()
        {
            const int titleSpacingY = 15;
            const int labelOffsetX = 22;
            const int barSize = 16;
            const int border = 4;
            const int barOffsetX = 2;

            // use scientific notation for the values if they do not reprent percentage ranges
            float TOLERANCE = 0.00001f;
            var format = Math.Abs(_maxValue - _minValue - 100) > TOLERANCE ? "0.000E+00" : "0";

            //try to figure out how wide we need this bitmap to be to display everything
            var maxWidth = float.MinValue;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                // measure the title
                var ef = graphics.MeasureString(_title, _titleFont);
                if (ef.Width + border > maxWidth)
                {
                    maxWidth = ef.Width + border;
                }

                //measure the values
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

            // create a bitmap to store the image that this legend creates
            var newImage = new Bitmap((int) maxWidth + border * 2,
                border * 2 + titleSpacingY + (NUMBER_OF_VALUES + 1) * barSize);
            using (var graphics = Graphics.FromImage(newImage))
            {
                //setup a semi-transparent white background to draw the legend on
                graphics.Clear(Color.FromArgb(0xcc, 0xff, 0xff, 0xff));

                // select the text color to use
                using (var textBrush = new SolidBrush(Color.Black))
                {
                    // draw the _title
                    graphics.DrawString(_title, _titleFont, textBrush, border, border);

                    for (var i = NUMBER_OF_VALUES; i >= 0; i--)
                    {
                        var y = titleSpacingY + barSize * i;

                        // draw the label for the scalar
                        var val = _minValue + (NUMBER_OF_VALUES - i) * (_maxValue - _minValue) / NUMBER_OF_VALUES;
                        graphics.DrawString(val.ToString(format), _valueFont, textBrush, labelOffsetX + border,
                            y + border);
                    }
                }

                // paint a linear gradient down the side to list all the colors in use
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

            // swap buffers
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