using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Core.Geometry;

namespace UI.Controls.Viewport.Overlay
{
    public class Label : LabelBase
    {
        private Font _font;
        private string _text;
        private Color _textColor, _backgroundColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="text">The text.</param>
        /// <param name="textFont">The text _font.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        internal Label(Point3 p, string text, Font textFont, Color textColor, Color backgroundColor)
            : base(p)
        {
            _textColor = textColor;
            _backgroundColor = backgroundColor;
            _text = text;
            _font = textFont;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                SetDirty();
            }
        }

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                SetDirty();
            }
        }

        public Color FillColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                SetDirty();
            }
        }

        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                SetDirty();
            }
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