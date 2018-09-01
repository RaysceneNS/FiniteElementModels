using System.ComponentModel;
using System.Diagnostics;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    internal class Camera
    {
        private Point3 _modelCenter;
        private float _modelExtent;
        private Point2 _panPosition;
        private Size2 _zoomSize;

        internal Camera()
        {
            Orientation = new Quaternion();
            ModelDistance = 100f;
            DegreesOfFreedom = Axes.Xyz;
        }

        [Category("Camera (Perspective")]
        public float FieldOfView { get; set; } = 60.0f;

        [Category("Camera (Perspective")]
        public float Near { get; set; } = 10f;

        [Category("Camera (Perspective")]
        public float Far { get; set; } = 500f;


        internal void Reset()
        {
            ResetPan();
            ResetZoom();
        }

        internal void ResizeViewport(int width, int height)
        {
            Debug.Assert(width != 0 && height != 0);

            // zero width or height will have bad consequences
            if (height == 0 || width == 0)
                return;

            Gl.glViewport(0, 0, width, height);

            // if the viewport is being resized then try to maintain a good approximation of our pan and zoom against the new window dimensions
            if (ViewportHeight != 0 && ViewportWidth != 0)
            {
                float zoomWidth, zoomHeight;

                // construct a new zoom window that will give the appearance of being the same relative zoom and pan
                if (_zoomSize.Width / ViewportWidth < _zoomSize.Height / ViewportHeight)
                {
                    //constrain the minimum and maximum zoom extents
                    zoomHeight = MathCore.Clamp(0.0001f,
                        _zoomSize.Width * (width / (float) ViewportWidth) * (height / (float) width), 10000f);
                    // maintain the aspect ratio of the viewport
                    zoomWidth = zoomHeight * (width / (float) height);
                }
                else
                {
                    //constrain the minimum and maximum zoom extents
                    zoomWidth = MathCore.Clamp(0.0001f,
                        _zoomSize.Height * (height / (float) ViewportHeight) * (width / (float) height), 10000f);
                    // maintain the aspect ratio of the viewport
                    zoomHeight = zoomWidth * (height / (float) width);
                }

                // scale the pan position relative to it's old position
                _panPosition = new Point2(
                    _panPosition.X / _zoomSize.Width * zoomWidth,
                    _panPosition.Y / _zoomSize.Height * zoomHeight);

                // create the new window
                _zoomSize = new Size2(zoomWidth, zoomHeight);
            }

            // set the new dimensions, and consequently the aspect ratio and zoom factor.
            ViewportWidth = width;
            ViewportHeight = height;
        }

        /// <summary>
        ///     Define the model position to point the camera at, as well as the extent of the model located there
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="extent">The extent.</param>
        internal void LookAtModel(Point3 position, float extent)
        {
            _modelCenter = position;
            _modelExtent = extent;
        }

        internal void Pan(float dx, float dy)
        {
            var currentZoomFactor = CurrentZoomFactor;
            _panPosition = Point2.Offset(_panPosition, dx * currentZoomFactor, dy * currentZoomFactor);
        }

        internal void ResetPan()
        {
            _panPosition = new Point2(ViewportWidth / 2f, ViewportHeight / 2f);
        }

        internal void Zoom(float amount)
        {
            //constrain the minimum and maximum zoom extents
            var h = MathCore.Clamp(0.0001f, _zoomSize.Height + amount * CurrentZoomFactor, 10000f);
            // maintain the aspect ratio
            var w = _zoomSize.Height * (ViewportWidth / (float) ViewportHeight);
            _zoomSize = new Size2(w,h);
        }

        internal void ResetZoom()
        {
            _zoomSize = new Size2(ViewportWidth, ViewportHeight);
        }

        internal void ZoomWindow(AxisAlignedBox2 rect)
        {
            // can't zoom nothing..
            if (rect.IsEmpty)
                return;

            //get the zoom factor
            var currentZoomFactor = CurrentZoomFactor;

            float height, width;
            if (ViewportWidth / rect.Width < ViewportHeight / rect.Height)
            {
                //constrain the minimum and maximum zoom extents
                height = MathCore.Clamp(0.0001f,
                    rect.Width * currentZoomFactor * (ViewportHeight / (float) ViewportWidth), 10000f);
                // maintain the aspect ratio of the viewport
                width = height * (ViewportWidth / (float) ViewportHeight);
            }
            else
            {
                //constrain the minimum and maximum zoom extents
                width = MathCore.Clamp(0.0001f,
                    rect.Height * currentZoomFactor * (ViewportWidth / (float) ViewportHeight), 10000f);
                // maintain the aspect ratio of the viewport
                height = width * (ViewportHeight / (float) ViewportWidth);
            }

            // move the pan position
            _panPosition += -_zoomSize.Center + rect.Center * currentZoomFactor;

            // cut a window out of the current zoom window
            _zoomSize = new Size2(width, height);
        }

        internal void Rotate(Vector3 axis, float angle)
        {
            // zero out any axis rotations for pitch, yaw or roll if we're in any kind of freedom locking
            var x = axis.X;
            var y = axis.Y;
            var z = axis.Z;
            if (DegreesOfFreedom != Axes.Xyz)
            {
                if ((DegreesOfFreedom & Axes.X) == Axes.X)
                    x = 0;

                if ((DegreesOfFreedom & Axes.Y) == Axes.Y)
                    y = 0;

                if ((DegreesOfFreedom & Axes.Z) == Axes.Z)
                    z = 0;
            }

            //Since a unit quaternion represents an orientation in 3D space, the multiplication of two unit quaternions will result
            //in another unit quaternion that represents the combined rotation. Amazing- but true
            Orientation *= new Quaternion(new Vector3(x,y,z), angle).Normalize();
        }

        internal void IsoView()
        {
            Orientation = new Quaternion(new Vector3(0.7f, -0.7f, -0.2f), (float)System.Math.PI/4).Normalize();
        }

        internal void SideView()
        {
            Orientation = new Quaternion(new Vector3(0f, 1f, 0f), (float)System.Math.PI / -2).Normalize();
        }

        internal void TopView()
        {
            Orientation = new Quaternion(new Vector3(1f, 0f, 0f), (float)System.Math.PI / 2).Normalize();
        }

        internal void FrontView()
        {
            Orientation = new Quaternion(new Vector3(1f, 0f, 0f), 0f).Normalize();
        }

        internal virtual void TransformProjectionMatrix()
        {
            var aspectRatio = ViewportWidth / (float)ViewportHeight;
            Glu.gluPerspective(FieldOfView, aspectRatio, Near, Far);

            // set the zoom rect
            int[] viewport = {0, 0, ViewportWidth, ViewportHeight};
            Glu.gluPickMatrix(_panPosition.X, _panPosition.Y, _zoomSize.Width, _zoomSize.Height, viewport);
        }

        internal void TransformModelViewMatrix()
        {
            Gl.glTranslatef(0f, 0f, -ModelDistance);

            // adjust the scale of the model relative to the distance that the camera is from it
            var scaleToModelDistance = ModelDistance / _modelExtent;

            // move into the model and rotate according to the camera position
            Gl.glScalef(scaleToModelDistance, scaleToModelDistance, scaleToModelDistance);

            // rotate the model under the camera position
            var axis = Orientation.Axis;
            Gl.glRotatef(Orientation.Degrees, axis.X, axis.Y, axis.Z);
            Gl.glRotatef(-90f, 1f, 0f, 0f);

            // center the model
            Gl.glTranslatef(-_modelCenter.X, -_modelCenter.Y, -_modelCenter.Z);
        }

        private float CurrentZoomFactor
        {
            get { return _zoomSize.Height / ViewportHeight; }
        }

        internal Quaternion Orientation { get; private set; }

        internal Axes DegreesOfFreedom { get; }

        public int ViewportWidth { get; private set; }

        public int ViewportHeight { get; private set; }

        public float ModelDistance { get; }
    }
}