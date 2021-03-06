using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace UI.Controls.Viewport
{
    public class LabelFlag : LabelBase
    {
        private readonly string _text;
        private readonly Font _textFont;
        private readonly Color _textColor;
        private readonly Color _backgroundColor;

        public LabelFlag(float x, float y, float z, string text, Font textFont, Color textColor, Color backgroundColor)
            : base(x, y, z)
        {
            _text = text;
            _textFont = textFont;
            _textColor = textColor;
            _backgroundColor = backgroundColor;
        }

        protected override Bitmap CreateImage()
        {
            const int poleHeight = 24;
            const int margin = 8;

            SizeF ef;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                ef = graphics.MeasureString(_text, _textFont);
            }

            var newImage = new Bitmap((int) Math.Ceiling(ef.Width) + margin, (int) Math.Ceiling(ef.Height) + poleHeight,
                PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                // draw the pole
                graphics.DrawLine(Pens.Black, 0, 0, 0, newImage.Height);
                graphics.DrawLine(Pens.Gray, 1, 0, 1, newImage.Height);
                graphics.DrawLine(Pens.Black, 2, 0, 2, newImage.Height);
                graphics.DrawLine(Pens.Black, 0, 0, 2, 0);

                PointF[] points =
                {
                    new PointF(2, 1),
                    new PointF(newImage.Width - 1, 1),
                    new PointF(newImage.Width - 3, ef.Height / 2 + 1),
                    new PointF(newImage.Width - 1, ef.Height + 2),
                    new PointF(2, ef.Height + 2)
                };

                using (var brush = new SolidBrush(_backgroundColor))
                {
                    graphics.FillPolygon(brush, points);
                }
                graphics.DrawPolygon(Pens.Black, points);

                using (var brush = new SolidBrush(_textColor))
                {
                    graphics.DrawString(_text, _textFont, brush, 5f, 2f);
                }
            }

            newImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            return newImage;
        }
    }
}