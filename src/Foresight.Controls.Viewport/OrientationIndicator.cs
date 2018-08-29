using System;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    /// <summary>
    ///     paint indicator arrows to the screen that rotate according to the current display rotation matrix.
    /// </summary>
    public sealed class OrientationIndicator : IDisposable
    {
        private int _displayList;
        private RasterFont _glFont;
        private Glu.GLUquadric _quadric;
        private bool _initialized;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrientationIndicator" /> class.
        /// </summary>
        internal OrientationIndicator()
        {
            IsVisible = true;
            _initialized = false;
        }

        public bool IsVisible { get; set; }
        
        internal void Draw(Quaternion quaternion, Axes axes)
        {
            const float textDistance = 38f;

            var drawX = (axes & Axes.X) == Axes.X;
            var drawY = (axes & Axes.Y) == Axes.Y;
            var drawZ = (axes & Axes.Z) == Axes.Z;

            if (_initialized == false)
                Initialize();

            Gl.glPushAttrib(Gl.GL_ENABLE_BIT);
            {
                var rasterPosX = Point2.Origin;
                var rasterPosY = Point2.Origin;
                var rasterPosZ = Point2.Origin;

                // Setup the lights for this quaternion display
                Gl.glPushMatrix();
                {
                    // position the arrow in the bottom left of the viewport
                    Gl.glTranslatef(38f, 38f, 100f);

                    //use the cameras rotation to rotate this axis indicator in a matching fashion
                    var axis = quaternion.Axis;
                    Gl.glRotatef(quaternion.Degrees, axis.X, axis.Y, axis.Z);
                    Gl.glRotatef(-90f, 1f, 0f, 0f);

                    // eliminate back facing polygons
                    Gl.glCullFace(Gl.GL_BACK);
                    Gl.glEnable(Gl.GL_CULL_FACE);

                    // set the lighting for the arrows
                    Gl.glEnable(Gl.GL_LIGHTING);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);

                    Gl.glEnable(Gl.GL_LIGHT6);
                    Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_POSITION, new[] {-50f, 100f, 100f, 1f});
                    Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_AMBIENT, new[] {0.10f, 0.10f, 0.10f, 1f});
                    Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_DIFFUSE, new[] {0.75f, 0.75f, 0.75f, 1f});
                    Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_SPECULAR, new[] {1f, 1f, 1f, 1f});

                    Gl.glEnable(Gl.GL_LIGHT7);
                    Gl.glLightfv(Gl.GL_LIGHT7, Gl.GL_POSITION, new[] {100f, -25f, 100f, 1f});
                    Gl.glLightfv(Gl.GL_LIGHT7, Gl.GL_AMBIENT, new[] {0.10f, 0.10f, 0.10f, 1f});
                    Gl.glLightfv(Gl.GL_LIGHT7, Gl.GL_DIFFUSE, new[] {0.20f, 0.20f, 0.20f, 1f});

                    // set the material for the arrows
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new[] {0.8f, 0.8f, 0.8f, 1f});
                    Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, 32);
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL);

                    var textPoint = new Point3(textDistance, 0f, 0f);

                    if (drawX)
                    {
                        // draw the red X arrow
                        Gl.glPushMatrix();
                        {
                            Gl.glColor3f(1f, 0f, 0f);
                            Gl.glCallList(_displayList);
                            rasterPosX = Viewport.Project(textPoint);
                        }
                        Gl.glPopMatrix();
                    }

                    if (drawY)
                    {
                        // draw the green Y arrow
                        Gl.glPushMatrix();
                        {
                            Gl.glColor3f(0f, 1f, 0f);
                            Gl.glRotatef(90f, 0f, 0f, 1f);
                            Gl.glCallList(_displayList);
                            rasterPosY = Viewport.Project(textPoint);
                        }
                        Gl.glPopMatrix();
                    }

                    if (drawZ)
                    {
                        // draw the blue Z arrow
                        Gl.glPushMatrix();
                        {
                            Gl.glColor3f(0f, 0.5f, 1f);
                            Gl.glRotatef(-90f, 0f, 1f, 0f);
                            Gl.glCallList(_displayList);
                            rasterPosZ = Viewport.Project(textPoint);
                        }
                        Gl.glPopMatrix();
                    }

                    // draw a small black center joint for the arrows
                    Gl.glColor3ub(51, 51, 51);
                    Glu.gluSphere(_quadric, 4.5, 8, 8);
                }
                Gl.glPopMatrix();

                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glColor3ub(255, 255, 255);
                // paint the axis labels, we turn off the depth test to ensure that these labels are drawn on top of the coordinate arrows
                if (drawX)
                    _glFont.Print("X", rasterPosX);
                if (drawY)
                    _glFont.Print("Y", rasterPosY);
                if (drawZ)
                    _glFont.Print("Z", rasterPosZ);
            }
            Gl.glPopAttrib();
        }

        private void Initialize()
        {
            _quadric = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(_quadric, Glu.GLU_FILL);
            Glu.gluQuadricNormals(_quadric, Glu.GLU_FLAT);

            _glFont = new RasterFont();

            CreateArrow();

            _initialized = true;
        }

        private void CreateArrow()
        {
            const int sides = 12;
            const float arrowBaseLen = 16f;
            const float arrowBaseDiam = 1.5f;
            const float arrowTopDiam = 5f;

            // create the display lines for an arrow segment
            if (_displayList == 0)
                _displayList = Gl.glGenLists(1);

            // draw a single arrow using a cylinder topped with a disk & cone 
            Gl.glNewList(_displayList, Gl.GL_COMPILE);
            {
                Gl.glPushMatrix();
                {
                    Gl.glRotatef(90f, 0f, 1f, 0f);

                    // draw the arrow shaft
                    Gl.glBegin(Gl.GL_QUAD_STRIP);
                    for (var i = 0; i <= sides; i++)
                    {
                        var d = i * 2 * Math.PI / sides;
                        var cos = (float) Math.Cos(d);
                        var sin = (float) Math.Sin(d);

                        Gl.glNormal3f(cos, sin, 0f);
                        Gl.glVertex3f(cos * arrowBaseDiam, sin * arrowBaseDiam, arrowBaseLen);
                        Gl.glVertex3f(cos * arrowBaseDiam, sin * arrowBaseDiam, 0f);
                    }
                    Gl.glEnd();
                }
                Gl.glPopMatrix();

                Gl.glPushMatrix();
                {
                    Gl.glTranslatef(arrowBaseLen, 0f, 0f);
                    Gl.glRotatef(90f, 0f, 1f, 0f);

                    var tangent = (float) Math.Tan(Math.Atan2(-arrowTopDiam, arrowBaseLen));

                    // draw the arrow cone
                    Gl.glBegin(Gl.GL_TRIANGLE_FAN);

                    // pinnacle of cone is a shared vertex
                    Gl.glVertex3f(0f, 0f, arrowBaseLen);
                    for (var i = 0; i <= sides; i++)
                    {
                        var d = i * 2 * Math.PI / sides;
                        var cos = (float) Math.Cos(d);
                        var sin = (float) Math.Sin(d);

                        var normal = new Vector3(cos, sin, tangent).Normalize();

                        Gl.glNormal3f(normal.X, normal.Y, normal.Z);
                        Gl.glVertex3f(cos * arrowTopDiam, sin * arrowTopDiam, 0f);
                    }
                    Gl.glEnd();
                }
                Gl.glPopMatrix();

                Gl.glPushMatrix();
                {
                    Gl.glTranslatef(arrowBaseLen, 0f, 0f);
                    Gl.glRotatef(-90f, 0f, 1f, 0f);

                    // draw a disk between the base of the cone and the shaft
                    Gl.glBegin(Gl.GL_TRIANGLE_FAN);

                    Gl.glNormal3f(0f, 0f, 1f);
                    Gl.glVertex3f(0f, 0f, 0f);
                    for (var i = 0; i <= sides; i++)
                    {
                        var d = i * 2 * Math.PI / sides;
                        var cos = (float) Math.Cos(d);
                        var sin = (float) Math.Sin(d);

                        Gl.glVertex3f(cos * arrowTopDiam, sin * arrowTopDiam, 0f);
                    }
                    Gl.glEnd();
                }
                Gl.glPopMatrix();
            }
            Gl.glEndList();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _glFont?.Dispose();

                Glu.gluDeleteQuadric(_quadric);
            }
        }
    }
}