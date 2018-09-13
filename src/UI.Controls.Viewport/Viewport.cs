using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    public class Viewport : Control
    {
        private readonly Camera _camera;
        private readonly Light _modelLight;
        private readonly GLContext _renderingContext;
        private ActionType _action;
        private DrawingModes _drawingMode;
        private Point2 _startPoint, _endPoint;

        public event EventHandler<EventArgs> ActionChanged;

        public Viewport()
        {
            InitializeComponent();
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, false);

            _renderingContext = new GLContext(Handle);
            _modelLight = new Light(Gl.GL_LIGHT0, 0.65f, 0.75f, 0.1f, new Point3(-50F, -150F, 100F));
            Labels = new List<LabelBase>();
            _drawingMode = DrawingModes.Shaded;
            _camera = new Camera();
            Legend = new Legend {Visible = false};
            
            SceneObjects = new SceneObjectCollection();
            SceneObjects.ListChanged += OnSceneListChanged;

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
                // compute the global bounds 
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


        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Legend Legend { get; }

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
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneObjectCollection SceneObjects { get; }

        [Browsable(false)]
        public List<LabelBase> Labels { get; }

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
            Gl.glClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Draw3D();
            Draw2D();
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
                // Setup the 2d orthogonal projection to match the viewport
                Gl.glLoadIdentity();
                Gl.glOrtho(0, width, 0, height, -200f, 200f);

                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();

                // turn off depth testing during the 2d drawing to ensure that the drawing is layered 
                // by the order in which we paint rather than the alpha value of the colors used
                Gl.glDisable(Gl.GL_DEPTH_TEST);

                foreach (var label in Labels)
                {
                    label.Draw();
                }

                if (Legend.Visible)
                    Legend.Draw(0, height - 200);
               
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

                    // set the position for every label in the scene, we use gluProject here to map from world space coordinates
                    // to screen space coordinates. We'll actually paint these when we switch to orthogonal projection later on
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

                if ((drawingModes & DrawingModes.WireFrame) == DrawingModes.WireFrame)
                {
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
                    Gl.glLineWidth(1f);
                    Gl.glDisable(Gl.GL_LIGHTING);
                    foreach (var entity in SceneObjects)
                    {
                        entity.DrawWireFrame();
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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _camera.Zoom(e.Delta / -4f);
            Invalidate();
            base.OnMouseWheel(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _endPoint = Point2.Origin;
            _startPoint = Point2.Origin;

            // only reset the cursor if we're not in box selection modes
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
                    case ActionType.Pan:
                        Cursor = Cursors.SizeAll;
                        break;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _action != ActionType.None)
            {
                switch (_action)
                {
                    case ActionType.Pan:
                        _endPoint = new Point2(e.X, e.Y);
                        _camera.Pan(_startPoint.X - _endPoint.X, (_startPoint.Y - _endPoint.Y) * -1);
                        _startPoint = _endPoint;
                        break;
                    case ActionType.Zoom:
                        _endPoint = new Point2(e.X, e.Y);
                        _camera.Zoom(_endPoint.Y - _startPoint.Y);
                        _startPoint = _endPoint;
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

            _camera.Reset();

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            _camera.TransformProjectionMatrix();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            _camera.TransformModelViewMatrix();

            var boundingRect = AxisAlignedBox2.Empty;
            foreach (var entity in SceneObjects)
            {
                var rect = entity.DisplayBoundingRect();

                if (boundingRect == AxisAlignedBox2.Empty)
                    boundingRect = rect;
                else if (rect != AxisAlignedBox2.Empty)
                    boundingRect = AxisAlignedBox2.Union(boundingRect, rect);
            }

            if (boundingRect != AxisAlignedBox2.Empty)
            {
                // add a margin around the model
                // zoom to the extents of the bounding rectangle
                _camera.ZoomWindow(AxisAlignedBox2.Inflate(boundingRect, 1f, 1f));
                Invalidate();
            }
        }

        public void TopView()
        {
//            _camera.TopView();
            Invalidate();
        }
    }
}