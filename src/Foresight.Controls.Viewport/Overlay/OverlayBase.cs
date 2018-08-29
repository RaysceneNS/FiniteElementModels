using System;
using System.Drawing;
using System.Drawing.Imaging;
using Tao.OpenGl;

namespace UI.Controls.Viewport.Overlay
{
    /// <summary>
    ///     A bitmap based raster overlay that will be painted on the scene
    /// </summary>
    public abstract class OverlayBase : IDisposable
    {
        private Bitmap _image;
        private bool _isDirty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayBase" /> class.
        /// </summary>
        internal OverlayBase()
        {
            Visible = true;
            _isDirty = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Sets the dirty flag.
        /// </summary>
        internal void SetDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        ///     Creates the image.
        /// </summary>
        /// <returns>the image</returns>
        protected abstract Bitmap CreateImage();

        /// <summary>
        ///     Draw the image
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        internal void Draw(int x, int y)
        {
            if (_isDirty)
            {
                _image = CreateImage();
                _isDirty = false;
            }

            if (_image == null)
                return;
            Gl.glPushAttrib(Gl.GL_ENABLE_BIT | Gl.GL_COLOR_BUFFER_BIT);
            {
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glEnable(Gl.GL_BLEND); //blending is required to respect the source Image alpha values

                Gl.glRasterPos2i(x, y);
                var rectangle = new Rectangle(0, 0, _image.Width, _image.Height);

                var bitmapdata = _image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Gl.glDrawPixels(_image.Width, _image.Height, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0);
                }
                finally
                {
                    _image.UnlockBits(bitmapdata);
                }
            }
            Gl.glPopAttrib();
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            _image?.Dispose();
        }
    }
}