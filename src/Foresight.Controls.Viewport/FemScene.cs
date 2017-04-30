﻿using System;
using System.Drawing;
using Core.Fem;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    public class FemScene : SceneObject
    {
        private readonly Model _model;
        private PlotMode _plotMode;
        private int _edgeDisplayList, _elementList, _meshList;
        private bool _displayListsCreated;
        private int _loadSymbolList, _constraintSymbolList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="symbolSize"></param>
        public FemScene(Model model, float symbolSize)
            : base()
        {
            this._plotMode = PlotMode.PerNode;
            _model = model;

            CreateSymbols(symbolSize);
        }

        /// <summary>
        /// Calculates the entity's bounding box.
        /// </summary>
        /// <returns></returns>
        public override AxisAlignedBox3 AxisAlignedBoundingBox()
        {
            return _model.AxisAlignedBoundingBox();
        }

        /// <summary>
        /// Creates the symbols.
        /// </summary>
        /// <param name="scale">The scale.</param>
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

        /// <summary>
        /// Determine the bounding rectangle for this mesh
        /// </summary>
        /// <returns></returns>
        internal override AxisAlignedBox2 DisplayBoundingRect()
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            // project the screen vertices into the vertice array
            foreach (var node in _model.Nodes)
            {
                var point = Project(node.X, node.Y, 0.0);

                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            return AxisAlignedBox2.FromExtents(minX, minY, maxX, maxY);
        }
        
        /// <summary>
        /// Draw method for implementors to override
        /// </summary>
        internal override void Draw()
        {
            Gl.glEnable(Gl.GL_LIGHTING);

            if (_model.IsSolved && this._displayListsCreated)
            {
                // draw the elements
                Gl.glCallList(this._elementList);

                //draw edges
                Gl.glLineWidth(2f);
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
                Gl.glLineWidth(1f);
                Gl.glColor3ub(0, 0, 0);
                Gl.glCallList(this._meshList);
            }
        }

        /// <summary>
        /// Paint the constraint sybols
        /// </summary>
        /// <param name="constraintSymbol">The constraint symbol.</param>
        private void DrawConstraints(int constraintSymbol)
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
                    Gl.glCallList(constraintSymbol);
                    Gl.glPopMatrix();
                }

                if (node.Constraint[1])
                {
                    Gl.glPushMatrix();
                    Gl.glTranslatef(node.X, node.Y, 0f);
                    Gl.glRotatef(270f, 1f, 0f, 0f); //rotate to the Y quaternion
                    Gl.glCallList(constraintSymbol);
                    Gl.glPopMatrix();
                }
            }
        }

        /// <summary>
        /// Paint the load symbols
        /// </summary>
        /// <param name="loadSymbol">The load symbol.</param>
        private void DrawLoads(int loadSymbol)
        {
            foreach (var node in this._model.Nodes)
            {
                if (!node.Loaded)
                    continue;
                Gl.glColor3ub(0xCC, 0x33, 0x0);

                var vector = new Vector3(node.Load[0], node.Load[1], 0f);
                Gl.glPushMatrix();
                Gl.glTranslatef(node.X, node.Y, 0f);
                Gl.glRotatef(vector.AngleOnXy(), 0f, 0f, 1f);
                Gl.glRotatef(vector.AngleFromXy() + 90f, 0f, -1f, 0f);
                Gl.glCallList(loadSymbol);
                Gl.glPopMatrix();
            }
        }

        internal override void DrawVertices()
        {
            if (this._model.Nodes == null)
                return;
            foreach (var node in this._model.Nodes)
            {
                Gl.glVertex2d(node.X, node.Y);
            }
        }

        /// <summary>
        /// Draw wireframe
        /// </summary>
        internal override void DrawWireframe()
        {
            Gl.glColor3ub(0, 0, 0);
            Gl.glCallList(this._meshList);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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
        }

        /// <summary>
        /// Compile the entity
        /// </summary>
        internal override void Compile()
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

        /// <summary>
        /// Updates the element list.
        /// </summary>
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

                Color c;
                switch (this._plotMode)
                {
                    case PlotMode.PerNode:
                        c = ramp[node1.ColorIndex];
                        Gl.glColor3ub(c.R, c.G, c.B);
                        Gl.glVertex2d(node1.X + node1.UX, node1.Y + node1.UY);

                        c = ramp[node2.ColorIndex];
                        Gl.glColor3ub(c.R, c.G, c.B);
                        Gl.glVertex2d(node2.X + node2.UX, node2.Y + node2.UY);

                        c = ramp[node3.ColorIndex];
                        Gl.glColor3ub(c.R, c.G, c.B);
                        Gl.glVertex2d(node3.X + node3.UX, node3.Y + node3.UY);
                        break;

                    case PlotMode.PerElement:
                        c = ramp[element.ColorIndex];
                        Gl.glColor3ub(c.R, c.G, c.B);
                        Gl.glVertex2d(node1.X + node1.UX, node1.Y + node1.UY);
                        Gl.glVertex2d(node2.X + node2.UX, node2.Y + node2.UY);
                        Gl.glVertex2d(node3.X + node3.UX, node3.Y + node3.UY);
                        break;
                }
            }
            Gl.glEnd();
            Gl.glEndList();
        }

        /// <summary>
        /// Updates the edge display list.
        /// </summary>
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
                        Gl.glVertex2d(edgeNode1.X + edgeNode1.UX, edgeNode1.Y + edgeNode1.UY);
                        Gl.glVertex2d(edgeNode2.X + edgeNode2.UX, edgeNode2.Y + edgeNode2.UY);
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
        
        /// <summary>
        /// Updates the mesh list.
        /// </summary>
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
        
        /// <summary>
        /// Gets or sets the plot mode.
        /// </summary>
        /// <scalar>The plot mode.</scalar>
        public PlotMode PlotMode
        {
            get
            {
                return this._plotMode;
            }
            set
            {
                this._plotMode = value;
            }
        }

        private static class PrimitiveFactory
        {
            /// <summary>
            ///     Create a cylinder
            /// </summary>
            /// <param name="firstDiameter"></param>
            /// <param name="secondDiameter"></param>
            /// <param name="length"></param>
            /// <param name="elems"></param>
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

            /// <summary>
            ///     Draw the constraint arrow
            /// </summary>
            /// <param name="scale"></param>
            public static void CreateConstraintArrow(float scale)
            {
                Gl.glPushMatrix();
                CreateCylinder(0.01f * scale, 2f * scale, 5f * scale, 16);
                Gl.glTranslatef(0f, 0f, 5f * scale);
                CreateRing(0.01f * scale, 2f * scale, 16);
                Gl.glPopMatrix();
            }

            /// <summary>
            ///     Draw the loading arrow
            /// </summary>
            /// <param name="scale"></param>
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

            /// <summary>
            ///     Create a circular plane
            /// </summary>
            /// <param name="insideDiameter"></param>
            /// <param name="outsideDiameter"></param>
            /// <param name="elems"></param>
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

    public enum PlotMode
    {
        PerElement,
        PerNode
    }
}