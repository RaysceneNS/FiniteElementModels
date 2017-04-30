using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport.Overlay
{
    public abstract class LabelBase : OverlayBase
    {
        private Point3 _attachingPoint;
        private Point2 _position;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabelBase" /> class.
        /// </summary>
        /// <param name="p">The p.</param>
        internal LabelBase(Point3 p)
        {
            _attachingPoint = p;
        }

        /// <summary>
        ///     Updates the pos.
        /// </summary>
        internal void UpdatePosition()
        {
            double winX;
            double winY;
            double winZ;

            var modelMatrix = new double[16];
            var projMatrix = new double[16];
            var viewport = new int[4];

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            if (Glu.gluProject(_attachingPoint.X, _attachingPoint.Y, _attachingPoint.Z, modelMatrix, projMatrix,
                    viewport, out winX, out winY, out winZ) == Gl.GL_FALSE)
                throw new GLException("Call to gluProject() failed.");

            _position = new Point2((float) winX, (float) winY);
        }

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        internal void Draw()
        {
            Draw((int) _position.X, (int) _position.Y);
        }
    }
}