using System.Diagnostics;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    internal class Camera
    {
        private const float FIELD_OF_VIEW  = 60.0f;
        private const float NEAR = 10f;
        private const float FAR = 500f;

        private int ViewportWidth { get; set; }
        private int ViewportHeight { get; set; }
        private float ModelDistance { get; }

        private Point3 _modelCenter;
        private float _modelExtent;
        private Point2 _panPosition;
        private Size2 _zoomSize;

        internal Camera()
        {
            ModelDistance = 100f;
        }

        internal void Reset()
        {
            _panPosition = new Point2(ViewportWidth / 2f, ViewportHeight / 2f);
            _zoomSize = new Size2(ViewportWidth, ViewportHeight);
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

        internal void Zoom(float amount)
        {
            var height = MathCore.Clamp(0.0001f, _zoomSize.Height + amount * CurrentZoomFactor, 10000f);
            var width = _zoomSize.Height * (ViewportWidth / (float) ViewportHeight);
            _zoomSize = new Size2(width,height);
        }

        internal void ZoomWindow(AxisAlignedBox2 rect)
        {
            if (rect.IsEmpty)
                return;

            var currentZoomFactor = CurrentZoomFactor;
            float height, width;
            if (ViewportWidth / rect.Width < ViewportHeight / rect.Height)
            {
                height = MathCore.Clamp(0.0001f, rect.Width * currentZoomFactor * (ViewportHeight / (float) ViewportWidth), 10000f);
                width = height * (ViewportWidth / (float) ViewportHeight);
            }
            else
            {
                width = MathCore.Clamp(0.0001f, rect.Height * currentZoomFactor * (ViewportWidth / (float) ViewportHeight), 10000f);
                height = width * (ViewportHeight / (float) ViewportWidth);
            }
            _panPosition += -_zoomSize.Center + rect.Center * currentZoomFactor;
            _zoomSize = new Size2(width, height);
        }

        internal void TransformProjectionMatrix()
        {
            var aspectRatio = ViewportWidth / (float)ViewportHeight;
            Glu.gluPerspective(FIELD_OF_VIEW, aspectRatio, NEAR, FAR);

            // set the zoom rect
            int[] viewport = {0, 0, ViewportWidth, ViewportHeight};
            Glu.gluPickMatrix(_panPosition.X, _panPosition.Y, _zoomSize.Width, _zoomSize.Height, viewport);
        }

        internal void TransformModelViewMatrix()
        {
            Gl.glTranslatef(0f, 0f, -ModelDistance);
            var scaleToModelDistance = ModelDistance / _modelExtent;
            Gl.glScalef(scaleToModelDistance, scaleToModelDistance, scaleToModelDistance);
            Gl.glTranslatef(-_modelCenter.X, -_modelCenter.Y, -_modelCenter.Z);
        }

        private float CurrentZoomFactor
        {
            get { return _zoomSize.Height / ViewportHeight; }
        }
    }
}