using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport.Overlay
{
    public abstract class LabelBase : OverlayBase
    {
        private readonly Point3 _attachingPoint;
        private Point2 _position;

        internal LabelBase(Point3 p)
        {
            _attachingPoint = p;
        }

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
            Draw((int) _position.X, (int) _position.Y);
        }
    }
}