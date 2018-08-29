using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Core.Geometry;

namespace UI.Controls.Viewport.Overlay
{
    public class Label : LabelBase
    {
        private readonly Font _font;
        private readonly string _text;
        private readonly Color _textColor;
        private readonly Color _backgroundColor;

        internal Label(Point3 p, string text, Font textFont, Color textColor, Color backgroundColor)
            : base(p)
        {
            _textColor = textColor;
            _backgroundColor = backgroundColor;
            _text = text;
            _font = textFont;
        }

        protected override Bitmap CreateImage()
        {
            if (_text.Length == 0)
                return null;

            SizeF ef;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                ef = graphics.MeasureString(_text, _font);
            }

            // paint the text 
            var newImage = new Bitmap((int) Math.Ceiling(ef.Width), (int) Math.Ceiling(ef.Height));
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                if (_backgroundColor != Color.Transparent)
                    using (var brush = new SolidBrush(_backgroundColor))
                    {
                        graphics.FillRectangle(brush, 0, 0, ef.Width, ef.Height);
                    }

                using (var brush = new SolidBrush(_textColor))
                {
                    graphics.DrawString(_text, _font, brush, 0f, 0f);
                }
            }

            // gl coordinates differ from gdi's
            newImage.RotateFlip(RotateFlipType.Rotate180FlipX);

            // swap buffers
            return newImage;
        }
    }
}