using System;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    public abstract class LabelBase : IDisposable
    {
        private readonly Point3 _attachingPoint;
        private Point2 _position;
        private Bitmap _image;
        private bool _isDirty;
        
        internal LabelBase(Point3 attachingPoint)
        {
            _attachingPoint = attachingPoint;
            _isDirty = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                _image?.Dispose();
        }
        
        protected abstract Bitmap CreateImage();

        internal void UpdatePosition()
        {
            var modelMatrix = new double[16];
            var projMatrix = new double[16];
            var viewport = new int[4];

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            if (Glu.gluProject(_attachingPoint.X, _attachingPoint.Y, _attachingPoint.Z, modelMatrix, projMatrix, viewport, out var winX, out var winY, out _) == Gl.GL_FALSE)
                throw new GLException("Call to gluProject() failed.");

            _position = new Point2((float) winX, (float) winY);
        }

        internal void Draw()
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

                Gl.glRasterPos2i((int) _position.X, (int) _position.Y);
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
    }
}