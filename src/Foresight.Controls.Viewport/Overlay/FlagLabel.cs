using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Core.Geometry;

namespace UI.Controls.Viewport.Overlay
{
    /// <summary>
    ///     Paints a text label on a small wavy bit of fabric suspended on a pole
    /// </summary>
    public class FlagLabel : Label
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FlagLabel" /> class.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="z">Z.</param>
        /// <param name="text">The text.</param>
        /// <param name="textFont">The text font.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public FlagLabel(float x, float y, float z, string text, Font textFont, Color textColor, Color backgroundColor)
            : base(new Point3(x, y, z), text, textFont, textColor, backgroundColor)
        {
        }

        protected override Bitmap CreateImage()
        {
            const int poleHeight = 24;
            const int margin = 8;

            SizeF ef;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                ef = graphics.MeasureString(Text, Font);
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

                // create the flag fabric shape
                PointF[] points =
                {
                    new PointF(2, 1),
                    new PointF(newImage.Width - 1, 1),
                    new PointF(newImage.Width - 3, ef.Height / 2 + 1),
                    new PointF(newImage.Width - 1, ef.Height + 2),
                    new PointF(2, ef.Height + 2)
                };

                // draw the flag fabric
                using (var brush = new SolidBrush(FillColor))
                {
                    graphics.FillPolygon(brush, points);
                }
                graphics.DrawPolygon(Pens.Black, points);

                // draw the text
                using (var brush = new SolidBrush(TextColor))
                {
                    graphics.DrawString(Text, Font, brush, 5f, 2f);
                }
            }

            // gl coordinates differ from gdi's
            newImage.RotateFlip(RotateFlipType.Rotate180FlipX);

            return newImage;
        }
    }
}