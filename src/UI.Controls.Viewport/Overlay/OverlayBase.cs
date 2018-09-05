using System;
using System.Drawing;
using System.Drawing.Imaging;
using Tao.OpenGl;

namespace UI.Controls.Viewport.Overlay
{
    public abstract class OverlayBase : IDisposable
    {
        private Bitmap _image;
        private bool _isDirty;

        internal OverlayBase()
        {
            Visible = true;
            _isDirty = true;
        }

        public bool Visible { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void SetDirty()
        {
            _isDirty = true;
        }

        protected abstract Bitmap CreateImage();

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

                var data = _image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Gl.glDrawPixels(_image.Width, _image.Height, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, data.Scan0);
                }
                finally
                {
                    _image.UnlockBits(data);
                }
            }
            Gl.glPopAttrib();
        }

        protected virtual void Dispose(bool disposing)
        {
            _image?.Dispose();
        }
    }
}