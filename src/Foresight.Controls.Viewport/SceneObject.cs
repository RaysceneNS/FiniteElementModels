using System;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    public abstract class SceneObject : IDisposable
    {
        /// <summary>
        ///     Dispose of this entities resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Calculates the entity's bounding box.
        /// </summary>
        /// <returns></returns>
        public abstract AxisAlignedBox3 AxisAlignedBoundingBox();

        /// <summary>
        ///     Determine the bounding rectangle for this mesh
        /// </summary>
        /// <returns></returns>
        internal abstract AxisAlignedBox2 DisplayBoundingRect();

        /// <summary>
        ///     Releases unmanaged resources and performs other cleanup operations before the
        ///     <see cref="SceneObject" /> is reclaimed by garbage collection.
        /// </summary>
        ~SceneObject()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Draw method for implementors to override
        /// </summary>
        internal abstract void Draw();

        
        /// <summary>
        /// Draw wireframe
        /// </summary>
        internal virtual void DrawWireframe()
        {
        }

        /// <summary>
        ///     Maps object coordinates to window coordinates
        /// </summary>
        /// <param name="objX">The obj X.</param>
        /// <param name="objY">The obj Y.</param>
        /// <param name="objZ">The obj Z.</param>
        /// <returns></returns>
        protected static Point2 Project(double objX, double objY, double objZ)
        {
            var modelMatrix = new double[16];
            var projMatrix = new double[16];
            var viewport = new int[4];

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            double x, y, z;
            if (Glu.gluProject(objX, objY, objZ, modelMatrix, projMatrix, viewport, out x, out y, out z) == Gl.GL_FALSE)
                throw new GLException("Call to gluProject() failed.");
            return new Point2((float) x, (float) y);
        }

        /// <summary>
        ///     Compile the entity
        /// </summary>
        internal virtual void Compile()
        {
        }

        /// <summary>
        ///     Draws the vertices.
        /// </summary>
        internal abstract void DrawVertices();
    }
}