using System;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    public abstract class SceneObject : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract AxisAlignedBox3 AxisAlignedBoundingBox();
        internal abstract AxisAlignedBox2 DisplayBoundingRect();

        ~SceneObject()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        internal abstract void Draw();
        internal virtual void DrawWireframe()
        {
        }

        protected static Point2 Project(double objX, double objY, double objZ)
        {
            var modelMatrix = new double[16];
            var projMatrix = new double[16];
            var viewport = new int[4];

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
            
            if (Glu.gluProject(objX, objY, objZ, modelMatrix, projMatrix, viewport, out var x, out var y, out var z) == Gl.GL_FALSE)
                throw new GLException("Call to gluProject() failed.");
            return new Point2((float) x, (float) y);
        }

        internal virtual void Compile()
        {
        }

        internal abstract void DrawVertices();
    }
}