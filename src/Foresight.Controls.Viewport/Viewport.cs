using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows.Forms;
using Core.Geometry;
using Core.MathLib;
using Tao.OpenGl;
using UI.Controls.Viewport.Overlay;

namespace UI.Controls.Viewport
{
    public class Viewport : Control
    {
        private readonly Camera _camera;
        private readonly Light _modelLight;
        private readonly OrientationIndicator _orientationIndicator;
        private readonly GLContext _renderingContext;
        private readonly Cursor _rotateCursor;
        private readonly Cursor _zoomCursor;
        private ActionType _action;
        private DrawingModes _drawingMode;
        private Vector3 _rotationPosition;
        private Point2 _startPoint, _endPoint;
        private readonly List<LabelBase> _labels;
        private readonly Legend _legend;

        public event EventHandler<EventArgs> ActionChanged;

        public Viewport()
        {
            InitializeComponent();
            
            // set the control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, false);

            //create the open gl context
            _renderingContext = new GLContext(Handle);
            
            // create a light for the model
            _modelLight = new Light(Gl.GL_LIGHT0, 0.65f, 0.75f, 0.1f, new Point3(-50F, -150F, 100F));

            _labels = new List<LabelBase>();
            _drawingMode = DrawingModes.Shaded;

            // default to a perspective based camera
            _camera = new CameraPerspective();

            // create the legend, the legend is situated near the top left of the viewport 
            _legend = new Legend {Visible = false};
            
            //Create Custom Cursors
            _zoomCursor = new Cursor(typeof(Viewport), "zoom.cur");
            _rotateCursor = new Cursor(typeof(Viewport), "rotate.cur");

            SceneObjects = new SceneObjectCollection();
            SceneObjects.ListChanged += OnSceneListChanged;

            _orientationIndicator = new OrientationIndicator
            {
                IsVisible = false
            };

            // enter any custom configuration for the current context
            SetupGL();
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                var CS_VREDRAW = 0x0001;
                var CS_HREDRAW = 0x0002;
                var CS_OWNDC = 0x0020;
                var WS_EX_DLGMODALFRAME = 0x00000001;
                var WS_EX_CLIENTEDGE = 0x00000200;
                var WS_BORDER = 0x00800000;

                var createParams = base.CreateParams;
                createParams.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;
                createParams.ExStyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE);
                createParams.Style &= ~(WS_BORDER | WS_EX_DLGMODALFRAME);
                createParams.Style |= WS_BORDER;
                return createParams;
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "Viewport";
            ResumeLayout(true);
        }

        private static void SetupGL()
        {
            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_COLOR_CONTROL, Gl.GL_SEPARATE_SPECULAR_COLOR);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glFrontFace(Gl.GL_CCW);
            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, 1);

            // set the ability to specify vertex colors using glColor commands
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);

            // nice line & point smoothing
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _renderingContext?.Dispose();
                SceneObjects?.Clear();
                Legend?.Dispose();
                _rotateCursor?.Dispose();
                _zoomCursor?.Dispose();
                _orientationIndicator?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.Clear(BackColor);
            }
            else
            {
                if (_renderingContext.DeviceContext == IntPtr.Zero)
                    throw new GLContextException("No device context available!");
                if (_renderingContext.RenderingContext == IntPtr.Zero)
                    throw new GLContextException("No rendering context available!");

                // set the rendering context as current
                _renderingContext.MakeCurrent();

                //paint the scene using the current context
                DrawAll();

                // our rendering is double buffered so swap those buffers now
                _renderingContext.SwapBuffers();

                //check for errors after rendering the scene
                var errorCode = Gl.glGetError(); // this incurs 0.7sec
                if (errorCode == Gl.GL_NO_ERROR)
                    return;
                switch (errorCode)
                {
                    case Gl.GL_INVALID_ENUM:
                        throw new GLException(
                            "GL_INVALID_ENUM - An unacceptable value has been specified for an enumerated argument.");
                    case Gl.GL_INVALID_VALUE:
                        throw new GLException(
                            "GL_INVALID_VALUE - An unacceptable value is specified for an enumerated argument. The offending function is ignored, having no side effect other than to set the error flag.");
                    case Gl.GL_INVALID_OPERATION:
                        throw new GLException(
                            "GL_INVALID_OPERATION - glPushMatrix was called between a call to glBegin and the corresponding call to glEnd.");
                    case Gl.GL_STACK_OVERFLOW:
                        throw new GLException(
                            "GL_STACK_OVERFLOW - glPushMatrix was called while the current matrix stack was full.");
                    case Gl.GL_STACK_UNDERFLOW:
                        throw new GLException(
                            "GL_STACK_UNDERFLOW - glPopMatrix was called while the current matrix stack contained only a single matrix.");
                    case Gl.GL_OUT_OF_MEMORY:
                        throw new GLException(
                            "GL_OUT_OF_MEMORY - There is not enough memory left to execute the function. The state of OpenGL is undefined, except for the state of the error flags, after this error is recorded.");
                    default:
                        throw new GLException("Undefined GL error.");
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _renderingContext.MakeCurrent();

            if (Height != 0 && Width != 0)
                _camera.ResizeViewport(Width, Height);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        private void OnSceneListChanged(object sender, EventArgs e)
        {
            //Rescale Model
            if (SceneObjects.Count != 0)
            {
                // compute the global bounds by unioning all the scene object bounding volumes together into one super volume
                var boundingBox = AxisAlignedBox3.Empty;
                foreach (var entity in SceneObjects)
                {
                    var rect = entity.AxisAlignedBoundingBox();
                    if (boundingBox == AxisAlignedBox3.Empty)
                        boundingBox = rect;
                    else if (rect != AxisAlignedBox3.Empty)
                        boundingBox = AxisAlignedBox3.Union(boundingBox, rect);
                }

                if (boundingBox != AxisAlignedBox3.Empty)
                    _camera.LookAtModel(boundingBox.Center, boundingBox.Extent);
            }
            else
            {
                //default to looking at the origin if nothing else is around
                _camera.LookAtModel(Point3.Origin, 100f);
            }
        }

        public bool OrientationIndicatorVisible
        {
            get { return _orientationIndicator.IsVisible; }
            set { _orientationIndicator.IsVisible = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Legend Legend
        {
            get { return _legend; }
        }
        
        [Browsable(false)]
        public ActionType Action
        {
            get { return _action; }
            set
            {
                // apply the action type.
                if (_action != value)
                {
                    _action = value;
                    ActionChanged?.Invoke(this, new EventArgs());
                }

                //set the action cursor
                switch (_action)
                {
                    case ActionType.ZoomWindow:
                        Cursor = Cursors.Cross;
                        break;
                    default:
                        Cursor = Cursors.Arrow;
                        break;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneObjectCollection SceneObjects { get; }

        [Browsable(false)]
        public List<LabelBase> Labels
        {
            get { return _labels; }
        }

        [Description("Drawing mode.")]
        [Category("Appearance")]
        public DrawingModes DrawingMode
        {
            get { return _drawingMode; }
            set
            {
                _drawingMode = value;
                Invalidate();
            }
        }
        
        private void DrawAll()
        {
            // Clear Screen And Depth Buffer
            Gl.glClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            // draw the 3d entities
            Draw3D();

            // draw the 2d portions of the scene this includes the zoom boxes, legend, progressbar and labels
            Draw2D();
            
            // flush 'er down
            Gl.glFlush();
        }

        private void Draw2D()
        {
            var width = Width;
            var height = Height;

            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            {
                // Setup the 2d ortho project to match the viewport
                Gl.glLoadIdentity();
                Gl.glOrtho(0, width, 0, height, -200f, 200f);

                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();

                //paint the orientation arrow widget
                if (_orientationIndicator.IsVisible)
                {
                    _orientationIndicator.Draw(_camera.Orientation, _camera.DegreesOfFreedom);
                }

                // turn off depth testing during the 2d drawing to ensure that the drawing is layered 
                // by the order in which we paint rather than the alpha value of the colors used
                Gl.glDisable(Gl.GL_DEPTH_TEST);

                //paint the labels
                foreach (var label in Labels)
                {
                    if (label.Visible)
                        label.Draw();
                }

                // draw the legend in the top left region
                if (Legend.Visible)
                    Legend.Draw(0, height - 200);
                
                // draw selection boxes		
                if (MouseButtons == MouseButtons.Left)
                    switch (_action)
                    {
                        case ActionType.ZoomWindow:
                            DrawZoomWindowBox(new Point2(_endPoint.X, height - _endPoint.Y),
                                new Point2(_startPoint.X, height - _startPoint.Y));
                            break;
                    }

                Gl.glMatrixMode(Gl.GL_PROJECTION);
            }
            Gl.glPopMatrix();
        }

        private void Draw3D()
        {
            Gl.glPushAttrib(Gl.GL_ENABLE_BIT);
            {
                Gl.glEnable(Gl.GL_DEPTH_TEST);
                Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);

                // set the 3d projection
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glPushMatrix();
                {
                    Gl.glLoadIdentity();
                    _camera.TransformProjectionMatrix();

                    // set the 3d model space
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();

                    // we need to enable this after the identity matrix is set or else the light position will be off
                    _modelLight.SwitchOn();
                    _camera.TransformModelViewMatrix();

                    // set the position for every label in the scene, we use gluproject here to map from world space coordinates
                    // to screen space coordinates. We'll actually paint these when we switch to ortho projection later on
                    foreach (var label in Labels)
                    {
                        label.UpdatePosition();
                    }

                    // draw all the entities now
                    DrawSceneObjects(_drawingMode);
                    _modelLight.SwitchOff();

                    Gl.glMatrixMode(Gl.GL_PROJECTION);
                }
                Gl.glPopMatrix();
            }
            Gl.glPopAttrib();
        }

        private void DrawSceneObjects(DrawingModes drawingModes)
        {
            Gl.glPushAttrib(Gl.GL_ENABLE_BIT);
            {
                Gl.glEnable(Gl.GL_NORMALIZE);

                // Set Default Material
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new[] { 0.1f, 0.1f, 0.1f, 1.0f });
                Gl.glMateriali(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, 32);

                if ((drawingModes & DrawingModes.Vertices) == DrawingModes.Vertices)
                {
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_POINT);
                    Gl.glPointSize(1f);
                    Gl.glDisable(Gl.GL_LIGHTING);
                    foreach (var entity in SceneObjects)
                    {
                        entity.DrawVertices();
                    }
                }

                if ((drawingModes & DrawingModes.Wireframe) == DrawingModes.Wireframe)
                {
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
                    Gl.glLineWidth(1f);
                    Gl.glDisable(Gl.GL_LIGHTING);
                    foreach (var entity in SceneObjects)
                    {
                        entity.DrawWireframe();
                    }
                }

                if ((drawingModes & DrawingModes.Shaded) == DrawingModes.Shaded)
                {
                    Gl.glEnable(Gl.GL_LIGHTING);
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                    foreach (var entity in SceneObjects)
                    {
                        entity.Draw();
                    }
                }
            }
            Gl.glPopAttrib();
        }

        private static void DrawZoomWindowBox(Point2 p1, Point2 p2)
        {
            Gl.glPushAttrib(Gl.GL_ENABLE_BIT);
            {
                Gl.glDisable(Gl.GL_DEPTH_TEST);
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glLineWidth(1f);

                // draw a box in XOR mode
                Gl.glEnable(Gl.GL_COLOR_LOGIC_OP);
                Gl.glLogicOp(Gl.GL_XOR);
                Gl.glColor3ub(0xFF, 0xFF, 0xFF);
                Gl.glBegin(Gl.GL_LINE_LOOP);
                {
                    Gl.glVertex3f(p1.X, p1.Y, 0);
                    Gl.glVertex3f(p2.X, p1.Y, 0);
                    Gl.glVertex3f(p2.X, p2.Y, 0);
                    Gl.glVertex3f(p1.X, p2.Y, 0);
                }
                Gl.glEnd();
            }
            Gl.glPopAttrib();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float amount = e.Delta / -4f;
            _camera.Zoom(amount);
            Invalidate();

            base.OnMouseWheel(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_action == ActionType.ZoomWindow && _startPoint != _endPoint)
            {
                var p1 = new Point2(_startPoint.X, Height - _startPoint.Y);
                var p2 = new Point2(_endPoint.X, Height - _endPoint.Y);
                _camera.ZoomWindow(new AxisAlignedBox2(p1, p2));
                Invalidate();
            }

            _endPoint = Point2.Origin;
            _startPoint = Point2.Origin;

            // only reset the cursor if we're not in box selection modes
            if (Action != ActionType.ZoomWindow)
                Cursor = Cursors.Default;

            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Focus();

                switch (_action)
                {
                    case ActionType.Zoom:
                        Cursor = _zoomCursor;
                        break;
                    case ActionType.Pan:
                        Cursor = Cursors.SizeAll;
                        break;
                    case ActionType.Rotate:
                        Cursor = _rotateCursor;
                        break;
                }

                // set the start point
                if (_action == ActionType.Rotate)
                    _rotationPosition = Quaternion.ProjectToTrackball(e.X, e.Y, Width, Height);
                else
                    _startPoint = _endPoint = new Point2(e.X, e.Y);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _action != ActionType.None)
            {
                switch (_action)
                {
                    case ActionType.ZoomWindow:
                        _endPoint = new Point2(e.X, e.Y);
                        Invalidate();
                        break;
                    case ActionType.Pan:
                        _endPoint = new Point2(e.X, e.Y);
                        _camera.Pan(_startPoint.X - _endPoint.X, (_startPoint.Y - _endPoint.Y) * -1);
                        _startPoint = _endPoint;
                        break;
                    case ActionType.Rotate:
                        var currentRotation = Quaternion.ProjectToTrackball(e.X, e.Y, Width, Height);
                        var diff = currentRotation - _rotationPosition;
                        var magnitude = (float) diff.Magnitude;
                        if (magnitude > 1E-06)
                        {
                            _camera.Rotate(Vector3.Cross(_rotationPosition, currentRotation).Normalize(), magnitude);
                        }
                        _rotationPosition = currentRotation;
                        break;
                    case ActionType.Zoom:
                        _endPoint = new Point2(e.X, e.Y);
                        _camera.Zoom(_endPoint.Y - _startPoint.Y);
                        _startPoint = _endPoint;
                        break;
                    default:
                        if (_action == ActionType.ZoomWindow)
                        {
                            _endPoint = new Point2(e.X, e.Y);
                        }
                        break;
                }

                Invalidate();
            }

            base.OnMouseMove(e);
        }

        public void ZoomExtents()
        {
            if (SceneObjects.Count <= 0)
                return;

            // reset the pan and zoom 
            _camera.Reset();

            // set the project matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            _camera.TransformProjectionMatrix();

            // set the modelview matrix
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            _camera.TransformModelViewMatrix();

            // calculate the model display extents
            var boundingRect = AxisAlignedBox2.Empty;
            foreach (var entity in SceneObjects)
            {
                var rect = entity.DisplayBoundingRect();

                if (boundingRect == AxisAlignedBox2.Empty)
                    boundingRect = rect;
                else if (rect != AxisAlignedBox2.Empty)
                    boundingRect = AxisAlignedBox2.Union(boundingRect, rect);
            }

            // all objects should be able to produce 2d bounding rects
            Debug.Assert(boundingRect != AxisAlignedBox2.Empty);

            if (boundingRect != AxisAlignedBox2.Empty)
            {
                // add a margin around the model
                // zoom to the extents of the bounding rectangle
                _camera.ZoomWindow(AxisAlignedBox2.Inflate(boundingRect, 1f, 1f));
                Invalidate();
            }
        }

        public void Zoom(float amount)
        {
            _camera.Zoom(amount);
        }

        public void ResetZoom()
        {
            _camera.ResetZoom();
        }

        public void Pan(float dx, float dy)
        {
            _camera.Pan(dx, dy);
            Invalidate();
        }

        public void ResetPan()
        {
            _camera.ResetPan();
            Invalidate();
        }

        public void Rotate(Vector3 axis, float radians)
        {
            _camera.Rotate(axis, radians);
            Invalidate();
        }
        
        public void TopView()
        {
            _camera.TopView();
            Invalidate();
        }

        public void FrontView()
        {
            _camera.FrontView();
            Invalidate();
        }

        public void SideView()
        {
            _camera.SideView();
            Invalidate();
        }

        public void IsoView()
        {
            _camera.IsoView();
            Invalidate();
        }

        internal static Point2 Project(Point3 p)
        {
            var modelMatrix = new double[16];
            var projMatrix = new double[16];
            var viewport = new int[4];

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            if (Glu.gluProject(p.X, p.Y, p.Z, modelMatrix, projMatrix, viewport, out var winX, out var winY, out _) == Gl.GL_FALSE)
                throw new GLException("Call to gluProject() failed.");

            return new Point2((float) winX, (float) winY);
        }
    }
}