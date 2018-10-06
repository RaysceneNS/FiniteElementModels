using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    internal class Camera
    {
        private const float FIELD_OF_VIEW  = 60.0f;
        private const float NEAR = 10f;
        private const float FAR = 500f;
        private const float MODEL_DISTANCE = 100f;

        private int _viewportWidth;
        private int _viewportHeight;
        private float _modelX, _modelY, _modelZ;
        private float _modelExtent;
        private float _panPositionX, _panPositionY;
        private float _zoomWidth, _zoomHeight;

        internal void Reset()
        {
            _panPositionX = _viewportWidth / 2f;
            _panPositionY = _viewportHeight / 2f;
            _zoomWidth = _viewportWidth;
            _zoomHeight = _viewportHeight;
        }

        internal void ResizeViewport(int width, int height)
        {
            // zero width or height will have bad consequences
            if (height == 0 || width == 0)
                return;

            Gl.glViewport(0, 0, width, height);

            // if the viewport is being resized then try to maintain a good approximation of our pan and zoom against the new window dimensions
            if (_viewportHeight != 0 && _viewportWidth != 0)
            {
                float zoomWidth, zoomHeight;

                // construct a new zoom window that will give the appearance of being the same relative zoom and pan
                if (_zoomWidth / _viewportWidth < _zoomHeight / _viewportHeight)
                {
                    //constrain the minimum and maximum zoom extents
                    zoomHeight = Clamp(0.0001f,
                        _zoomWidth * (width / (float) _viewportWidth) * (height / (float) width), 10000f);
                    // maintain the aspect ratio of the viewport
                    zoomWidth = zoomHeight * (width / (float) height);
                }
                else
                {
                    //constrain the minimum and maximum zoom extents
                    zoomWidth = Clamp(0.0001f,
                        _zoomHeight * (height / (float) _viewportHeight) * (width / (float) height), 10000f);
                    // maintain the aspect ratio of the viewport
                    zoomHeight = zoomWidth * (height / (float) width);
                }

                // scale the pan position relative to it's old position
                _panPositionX = _panPositionX / _zoomWidth * zoomWidth;
                _panPositionY = _panPositionY / _zoomHeight * zoomHeight;

                // create the new window
                _zoomWidth = zoomWidth;
                _zoomHeight = zoomHeight;
            }

            // set the new dimensions, and consequently the aspect ratio and zoom factor.
            _viewportWidth = width;
            _viewportHeight = height;
        }

        /// <summary>
        ///     Define the model position to point the camera at, as well as the extent of the model located there
        /// </summary>
        internal void LookAtModel(float x, float y, float z, float extent)
        {
            _modelX = x;
            _modelY = y;
            _modelZ = z;
            _modelExtent = extent;
        }

        internal void Pan(float dx, float dy)
        {
            var currentZoomFactor = CurrentZoomFactor();
            _panPositionX += dx * currentZoomFactor;
            _panPositionY += dy * currentZoomFactor;
        }

        internal void Zoom(float amount)
        {
            var height = Clamp(0.0001f, _zoomHeight + amount * CurrentZoomFactor(), 10000f);
            var width = height * (_viewportWidth / (float) _viewportHeight);
            _zoomWidth = width;
            _zoomHeight = height;
        }

        internal void ZoomWindow(float panX, float panY, float rectWidth, float rectHeight)
        {
            if (rectWidth <=  0 || rectHeight <= 0)
                return;

            var currentZoomFactor = CurrentZoomFactor();
            float height, width;
            if (_viewportWidth / rectWidth < _viewportHeight / rectHeight)
            {
                height = Clamp(0.0001f, rectWidth * currentZoomFactor * (_viewportHeight / (float) _viewportWidth), 10000f);
                width = height * (_viewportWidth / (float) _viewportHeight);
            }
            else
            {
                width = Clamp(0.0001f, rectHeight * currentZoomFactor * (_viewportWidth / (float) _viewportHeight), 10000f);
                height = width * (_viewportHeight / (float) _viewportWidth);
            }

            var zoomCenterX = _zoomWidth / 2f;
            var zoomCenterY = _zoomHeight / 2f;

            _panPositionX += -zoomCenterX + panX * currentZoomFactor;
            _panPositionY += -zoomCenterY + panY * currentZoomFactor;

            _zoomWidth = width;
            _zoomHeight = height;
        }

        internal void TransformProjectionMatrix()
        {
            // set the zoom rect
            int[] viewport = {0, 0, _viewportWidth, _viewportHeight};
            Glu.gluPickMatrix(_panPositionX, _panPositionY, _zoomWidth, _zoomHeight, viewport);

            var aspectRatio = _viewportWidth / (float)_viewportHeight;
            Glu.gluPerspective(FIELD_OF_VIEW, aspectRatio, NEAR, FAR);
        }

        internal void TransformModelViewMatrix()
        {
            Gl.glTranslatef(0f, 0f, -MODEL_DISTANCE);
            var scaleToModelDistance = MODEL_DISTANCE / _modelExtent;
            Gl.glScalef(scaleToModelDistance, scaleToModelDistance, scaleToModelDistance);
            Gl.glTranslatef(-_modelX, -_modelY, -_modelZ);
        }

        private float CurrentZoomFactor()
        {
            return _zoomHeight / _viewportHeight;
        }

        private static float Clamp(float low, float val, float high)
        {
            if (val < low)
                return low;
            if (val > high)
                return high;
            return val;
        }
    }
}