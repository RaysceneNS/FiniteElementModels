using System;
using Core.Fea;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    public class FeaScene : SceneObject, IDisposable
    {
        private readonly Model _model;
        private int _edgeDisplayList, _elementList, _meshList;
        private bool _displayListsCreated;
        private int _loadSymbolList, _constraintSymbolList;

        public FeaScene(Model model, float symbolSize = 0.1f)
            : base()
        {
            _model = model;
            CreateSymbols(symbolSize);
        }

        private void CreateSymbols(float scale)
        {
            // constraint arrows
            if (this._constraintSymbolList == 0)
                this._constraintSymbolList = Gl.glGenLists(1);
            Gl.glNewList(this._constraintSymbolList, Gl.GL_COMPILE);
            PrimitiveFactory.CreateConstraintArrow(scale);
            Gl.glEndList();

            //load arrows
            if (this._loadSymbolList == 0)
                this._loadSymbolList = Gl.glGenLists(1);
            Gl.glNewList(this._loadSymbolList, Gl.GL_COMPILE);
            PrimitiveFactory.CreateLoadArrow(scale);
            Gl.glEndList();
        }

        internal override void ModelExtents(out float minX, out float maxX, out float minY, out float maxY)
        {
            minX = float.MaxValue;
            minY = float.MaxValue;
            maxX = float.MinValue;
            maxY = float.MinValue;
            foreach (var node2D in _model.Nodes)
            {
                if (node2D.X < minX)
                    minX = node2D.X;
                if (node2D.X > maxX)
                    maxX = node2D.X;
                if (node2D.Y < minY)
                    minY = node2D.Y;
                if (node2D.Y > maxY)
                    maxY = node2D.Y;
            }
        }
        
        internal override void WindowExtents(out float vminX, out float vmaxX, out float vminY, out float vmaxY)
        {
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            var modelMatrix = new double[16];
            var projMatrix = new double[16];
            var viewport = new int[4];

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelMatrix);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            // project the screen vertices into the array
            foreach (var node in _model.Nodes)
            {
                if (Glu.gluProject(node.X, node.Y, 0, modelMatrix, projMatrix, viewport, out var x, out var y, out var _) == Gl.GL_FALSE)
                    throw new GLException("Call to gluProject() failed.");

                if (x < minX)
                    minX = x;
                if (x > maxX)
                    maxX = x;

                if (y < minY)
                    minY = y;
                if (y > maxY)
                    maxY = y;
            }

            vmaxX = (float) maxX;
            vmaxY = (float) maxY;
            vminX = (float) minX;
            vminY = (float) minY;
        }
        
        internal override void Draw()
        {
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            Gl.glEnable(Gl.GL_LIGHTING);

            if (_model.IsSolved && this._displayListsCreated)
            {
                // draw the elements
                Gl.glCallList(this._elementList);

                //draw edges
                Gl.glLineWidth(5f);
                Gl.glColor3ub(0, 0, 0);
                Gl.glCallList(this._edgeDisplayList);

                // draw the loadings and constraints
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEnable(Gl.GL_BLEND);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new [] { 0.5f, 0.5f, 0.5f, 1.0f });
                Gl.glMateriali(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, 32);
                this.DrawConstraints(this._constraintSymbolList);
                this.DrawLoads(this._loadSymbolList);
                Gl.glDisable(Gl.GL_BLEND);
                Gl.glDisable(Gl.GL_LIGHTING);
            }
            else
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
                Gl.glLineWidth(1f);
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glColor3ub(0, 0, 0);
                Gl.glCallList(this._meshList);
            }
        }

        private void DrawConstraints(int constraintSymbolSize)
        {
            foreach (var node in this._model.Nodes)
            {
                if (!node.Constrained)
                    continue;
                Gl.glColor3ub(0x0, 0xCC, 0x33);

                if (node.Constraint[0])
                {
                    Gl.glPushMatrix();
                    Gl.glTranslatef(node.X, node.Y, 0f);
                    Gl.glRotatef(90f, 0f, 1f, 0f); //rotate to the X quaternion
                    Gl.glCallList(constraintSymbolSize);
                    Gl.glPopMatrix();
                }

                if (node.Constraint[1])
                {
                    Gl.glPushMatrix();
                    Gl.glTranslatef(node.X, node.Y, 0f);
                    Gl.glRotatef(270f, 1f, 0f, 0f); //rotate to the Y quaternion
                    Gl.glCallList(constraintSymbolSize);
                    Gl.glPopMatrix();
                }
            }
        }

        private void DrawLoads(int loadSymbolSize)
        {
            foreach (var node in this._model.Nodes)
            {
                if (!node.Loaded)
                    continue;

                Gl.glColor3ub(0xCC, 0x33, 0x0);

                //rotate the load arrows
                var angleOnXy = (float) (Math.Atan2(node.Load[1], node.Load[0]) * 180.0 / Math.PI);
                var angleFromXy = (float) (Math.Atan2(0, Math.Sqrt(node.Load[0] * node.Load[0] + node.Load[1] * node.Load[1])) * 180.0 / Math.PI);

                Gl.glPushMatrix();
                Gl.glTranslatef(node.X, node.Y, 0f);
                Gl.glRotatef(angleOnXy, 0f, 0f, 1f);
                Gl.glRotatef(angleFromXy + 90f, 0f, -1f, 0f);
                Gl.glCallList(loadSymbolSize);
                Gl.glPopMatrix();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FeaScene()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (this._meshList != 0)
                Gl.glDeleteLists(this._meshList, 1);
            this._meshList = 0;

            if (this._elementList != 0)
                Gl.glDeleteLists(this._elementList, 1);
            this._elementList = 0;

            if (this._edgeDisplayList != 0)
                Gl.glDeleteLists(this._edgeDisplayList, 1);
            this._edgeDisplayList = 0;
        }

        public void Compile()
        {
            if (_model.IsSolved)
            {
                this._displayListsCreated = false;

                UpdateElementList();
                UpdateEdgeDisplayList();
                
                this._displayListsCreated = true;
            }

            UpdateMeshList();
        }

        private void UpdateElementList()
        {
            var ramp = ColorScale.Gradient;

            if (this._elementList == 0)
                this._elementList = Gl.glGenLists(1);

            Gl.glNewList(this._elementList, Gl.GL_COMPILE);

            Gl.glBegin(Gl.GL_TRIANGLES);

            //this is a 2d plane, the reflection normal is just z
            Gl.glNormal3f(0f, 0f, 1f);

            foreach (var element in _model.Elements)
            {
                var node1 = this._model.Nodes[element.NodeList[0]];
                var node2 = this._model.Nodes[element.NodeList[1]];
                var node3 = this._model.Nodes[element.NodeList[2]];

                var c = ramp[node1.ColorIndex];
                Gl.glColor3ub(c.R, c.G, c.B);
                Gl.glVertex2d(node1.X + node1.FreedomX, node1.Y + node1.FreedomY);

                c = ramp[node2.ColorIndex];
                Gl.glColor3ub(c.R, c.G, c.B);
                Gl.glVertex2d(node2.X + node2.FreedomX, node2.Y + node2.FreedomY);

                c = ramp[node3.ColorIndex];
                Gl.glColor3ub(c.R, c.G, c.B);
                Gl.glVertex2d(node3.X + node3.FreedomX, node3.Y + node3.FreedomY);

            }
            Gl.glEnd();
            Gl.glEndList();
        }

        private void UpdateEdgeDisplayList()
        {
            // create a lines for the edges
            if (this._edgeDisplayList == 0)
                this._edgeDisplayList = Gl.glGenLists(1);
            Gl.glNewList(this._edgeDisplayList, Gl.GL_COMPILE);
            Gl.glBegin(Gl.GL_LINES);
            if (_model.Edges != null)
            {
                foreach (var edge in _model.Edges)
                {
                    var edgeNode1 = this._model.Nodes[edge.V1];
                    var edgeNode2 = this._model.Nodes[edge.V2];

                    if (_model.IsSolved)
                    {
                        Gl.glVertex2d(edgeNode1.X + edgeNode1.FreedomX, edgeNode1.Y + edgeNode1.FreedomY);
                        Gl.glVertex2d(edgeNode2.X + edgeNode2.FreedomX, edgeNode2.Y + edgeNode2.FreedomY);
                    }
                    else
                    {
                        Gl.glVertex2d(edgeNode1.X, edgeNode1.Y);
                        Gl.glVertex2d(edgeNode2.X, edgeNode2.Y);
                    }
                }
            }
            Gl.glEnd();
            Gl.glEndList();
        }
        
        private void UpdateMeshList()
        {
            //draw the mesh elements
            if (this._meshList == 0)
                this._meshList = Gl.glGenLists(1);

            Gl.glNewList(this._meshList, Gl.GL_COMPILE);
            Gl.glBegin(Gl.GL_TRIANGLES);
            Gl.glColor3f(0f, 0f, 0f);
            Gl.glNormal3f(0f, 0f, 1f);
            foreach (var e in _model.Elements)
            {
                var node = this._model.Nodes[e.NodeList[0]];
                Gl.glVertex2d(node.X, node.Y);

                node = this._model.Nodes[e.NodeList[1]];
                Gl.glVertex2d(node.X, node.Y);

                node = this._model.Nodes[e.NodeList[2]];
                Gl.glVertex2d(node.X, node.Y);
            }
            Gl.glEnd();
            Gl.glEndList();
        }

        private static class PrimitiveFactory
        {
            private static void CreateCylinder(float firstDiameter, float secondDiameter, float length, int elems)
            {
                var thickness = secondDiameter - firstDiameter;
                var tangent = Math.Atan2(-thickness, length);

                Gl.glBegin(Gl.GL_QUAD_STRIP);
                for (var i = 0; i < elems; i++)
                {
                    var a = i * 2 * (float)Math.PI / elems;
                    var cosA = (float)Math.Cos(a);
                    var sinA = (float)Math.Sin(a);
                    var b = (i + 1) * 2 * (float)Math.PI / elems;
                    var cosB = (float)Math.Cos(b);
                    var sinB = (float)Math.Sin(b);

                    Gl.glNormal3f(cosA, sinA, (float)Math.Tan(tangent));
                    Gl.glVertex3f(cosA * secondDiameter, sinA * secondDiameter, length);
                    Gl.glNormal3f(cosA, sinA, (float)Math.Tan(tangent));
                    Gl.glVertex2f(cosA * firstDiameter, sinA * firstDiameter);
                    Gl.glNormal3f(cosB, sinB, (float)Math.Tan(tangent));
                    Gl.glVertex3f(cosB * secondDiameter, sinB * secondDiameter, length);
                    Gl.glNormal3f(cosB, sinB, (float)Math.Tan(tangent));
                    Gl.glVertex2f(cosB * firstDiameter, sinB * firstDiameter);
                }
                Gl.glEnd();
            }

            public static void CreateConstraintArrow(float scale)
            {
                Gl.glPushMatrix();
                CreateCylinder(0.01f * scale, 2f * scale, 5f * scale, 16);
                Gl.glTranslatef(0f, 0f, 5f * scale);
                CreateRing(0.01f * scale, 2f * scale, 16);
                Gl.glPopMatrix();
            }

            public static void CreateLoadArrow(float scale)
            {
                Gl.glPushMatrix();
                CreateCylinder(0.01f * scale, 2f * scale, 5f * scale, 16);
                Gl.glTranslatef(0f, 0f, 5f * scale);
                CreateRing(0.5f * scale, 2f * scale, 16);
                CreateCylinder(0.75f * scale, 0.75f * scale, 10f * scale, 16);
                Gl.glTranslatef(0f, 0f, 10f * scale);
                CreateRing(0.01f * scale, 0.75f * scale, 16);
                Gl.glPopMatrix();
            }

            private static void CreateRing(float insideDiameter, float outsideDiameter, int elems)
            {
                Gl.glBegin(Gl.GL_QUAD_STRIP);
                for (var i = 0; i < elems; i++)
                {
                    var a = i * 2 * (float)Math.PI / elems;
                    var cosA = (float)Math.Cos(a);
                    var sinA = (float)Math.Sin(a);
                    var b = (i + 1) * 2 * (float)Math.PI / elems;
                    var cosB = (float)Math.Cos(b);
                    var sinB = (float)Math.Sin(b);

                    Gl.glNormal3f(0f, 0f, 1f);
                    Gl.glVertex2f(cosA * insideDiameter, sinA * insideDiameter);
                    Gl.glVertex2f(cosA * outsideDiameter, sinA * outsideDiameter);
                    Gl.glVertex2f(cosB * insideDiameter, sinB * insideDiameter);
                    Gl.glVertex2f(cosB * outsideDiameter, sinB * outsideDiameter);
                }
                Gl.glEnd();
            }
        }
    }
}