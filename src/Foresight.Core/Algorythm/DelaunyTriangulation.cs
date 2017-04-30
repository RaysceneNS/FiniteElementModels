using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Geometry;

namespace Core.Algorythm
{
    public class DelaunayTriangulation
    {
        private const double EPSILON = 1E-06;
        internal readonly LinkedList<TriangleFace> Faces = new LinkedList<TriangleFace>();
        internal readonly List<Point2> Improvement = new List<Point2>();
        internal readonly List<List<Point2>> Loops = new List<List<Point2>>();
        internal readonly List<Point2> Vertices = new List<Point2>();

        /// <summary>
        /// Triangulates the loops.
        /// </summary>
        /// <returns></returns>
        protected void TriangulateInternal()
        {
            LinkedListNode<TriangleFace> first;
            LinkedListNode<TriangleFace> next;

            // clear the mesh
            Vertices.Clear();
            Faces.Clear();

            // add all the data from the loops
            foreach (var current in Loops)
            {
                if (current.Count > 0)
                {
                    var pointList = current.GetRange(0, current.Count - 1);
                    Vertices.AddRange(pointList);
                }
            }

            // add all the data from the improvement lines
            foreach (var tf in Improvement)
            {
                Vertices.Add(tf);
            }

            var verticeCount = Vertices.Count - 1;
            var minXValue = float.MaxValue;
            var maxXValue = float.MinValue;
            var minYValue = float.MaxValue;
            var maxYValue = float.MinValue;

            foreach (var p in Vertices)
            {
                if (p.X < minXValue)
                    minXValue = p.X;

                if (p.X > maxXValue)
                    maxXValue = p.X;

                if (p.Y < minYValue)
                    minYValue = p.Y;

                if (p.Y > maxYValue)
                    maxYValue = p.Y;
            }

            var xRange = maxXValue - minXValue;
            var yRange = maxYValue - minYValue;
            var largerRange = xRange <= yRange ? yRange : xRange;

            var midXValue = (maxXValue + minXValue) / 2f;
            var midYValue = (maxYValue + minYValue) / 2f;

            Vertices.Add(new Point2(midXValue - 2f * largerRange, midYValue - largerRange));
            Vertices.Add(new Point2(midXValue, midYValue + 2f * largerRange));
            Vertices.Add(new Point2(midXValue + 2f * largerRange, midYValue - largerRange));
            Faces.AddLast(new TriangleFace(verticeCount + 1, verticeCount + 2, verticeCount + 3));
            var edges = new LinkedList<TriangleEdge>();

            var index = 0;
            while (index < Vertices.Count && index <= verticeCount)
            {
                edges.Clear();
                first = Faces.First;
                do
                {
                    var face = first.Value;

                    next = first.Next;
                    var flag = InCircle(Vertices[index], Vertices[face.v1], Vertices[face.v2], Vertices[face.v3]);
                    if (flag)
                    {
                        var node3 = edges.Find(new TriangleEdge(face.v2, face.v1));
                        if (node3 == null)
                            edges.AddLast(new TriangleEdge(face.v1, face.v2));
                        else
                            edges.Remove(node3);
                        node3 = edges.Find(new TriangleEdge(face.v3, face.v2));

                        if (node3 == null)
                            edges.AddLast(new TriangleEdge(face.v2, face.v3));
                        else
                            edges.Remove(node3);

                        node3 = edges.Find(new TriangleEdge(face.v1, face.v3));
                        if (node3 == null)
                            edges.AddLast(new TriangleEdge(face.v3, face.v1));
                        else
                            edges.Remove(node3);

                        Faces.Remove(first);
                    }
                    first = next;
                } while (first != null);

                var edgeNode = edges.First;
                while (edgeNode != null)
                {
                    var edge = edgeNode.Value;
                    Faces.AddLast(new TriangleFace(edge.v1, edge.v2, index));
                    edgeNode = edgeNode.Next;
                }
                index++;
            }

            // remove any faces where v1, v2 or v3 exceeed verticecount
            first = Faces.First;
            do
            {
                var face = first.Value;
                next = first.Next;
                if (face.v1 > verticeCount || face.v2 > verticeCount || face.v3 > verticeCount)
                    Faces.Remove(first);

                first = next;
            } while (first != null);


            Vertices.RemoveRange(Vertices.Count - 3, 3);
            if (Faces.Count > 0 && Loops.Count > 0)
            {
                first = Faces.First;
                while (first != null)
                {
                    var face = first.Value;
                    first = first.Next;

                    var v1 = face.v1;
                    var v2 = face.v2;
                    var v3 = face.v3;

                    var p1 = Vertices[v1];
                    var p1X = p1.X;
                    var p1Y = p1.Y;

                    var p2 = Vertices[v2];
                    var p2X = p2.X;
                    var p2Y = p2.Y;

                    var p3 = Vertices[v3];
                    var p3X = p3.X;
                    var p3Y = p3.Y;

                    var newPoint = new Point2((p1X + p2X + p3X) / 3f, (p1Y + p2Y + p3Y) / 3f);
                    if (!InsidePolygons(newPoint, Loops))
                        Faces.Remove(face);
                }
            }
        }

        /// <summary>
        ///     Return TRUE if the point (xp,yp) lies inside the circumcircle
        ///     made up by points (x1,y1) (x2,y2) (x3,y3)
        ///     NOTE: a point enabled the edge1 is inside the circumcircle
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns></returns>
        protected static bool InCircle(Point2 vertex, Point2 p1, Point2 p2, Point2 p3)
        {
            double m1;
            double m2;
            double mx1;
            double mx2;
            double my1;
            double my2;
            double xc;
            double yc;

            if (Math.Abs(p1.Y - p2.Y) < EPSILON && Math.Abs(p2.Y - p3.Y) < EPSILON)
                return false;

            if (Math.Abs(p2.Y - p1.Y) < EPSILON)
            {
                m2 = -(p3.X - p2.X) / (p3.Y - p2.Y);
                mx2 = (p2.X + p3.X) / 2f;
                my2 = (p2.Y + p3.Y) / 2f;
                xc = (p2.X + p1.X) / 2f;
                yc = m2 * (xc - mx2) + my2;
            }
            else if (Math.Abs(p3.Y - p2.Y) < EPSILON)
            {
                m1 = -(p2.X - p1.X) / (p2.Y - p1.Y);
                mx1 = (p1.X + p2.X) / 2f;
                my1 = (p1.Y + p2.Y) / 2f;
                xc = (p3.X + p2.X) / 2f;
                yc = m1 * (xc - mx1) + my1;
            }
            else
            {
                m1 = -(p2.X - p1.X) / (p2.Y - p1.Y);
                m2 = -(p3.X - p2.X) / (p3.Y - p2.Y);
                mx1 = (p1.X + p2.X) / 2f;
                mx2 = (p2.X + p3.X) / 2f;
                my1 = (p1.Y + p2.Y) / 2f;
                my2 = (p2.Y + p3.Y) / 2f;
                xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                yc = m1 * (xc - mx1) + my1;
            }

            var dx = p2.X - xc;
            var dy = p2.Y - yc;
            var rsqr = dx * dx + dy * dy;

            dx = vertex.X - xc;
            dy = vertex.Y - yc;
            var drsqr = dx * dx + dy * dy;

            return !(drsqr > rsqr);
        }

        /// <summary>
        ///     tests each point in the lines of lists of points to see if point is within
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="pointList">The point list.</param>
        /// <returns></returns>
        private static bool InsidePolygons(Point2 p, IEnumerable<List<Point2>> pointList)
        {
            // must test every polygon to see if all inside polygon calls add to one.
            var count = 0;
            foreach (var list in pointList)
            {
                count += InsidePolygon(p, list);
            }
            return count == 1;
        }

        /// <summary>
        ///     Test the number of times a ray project from the test point crosses the edges that form the polygon
        ///     Assumes that the polygon is closed
        /// </summary>
        /// <param name="pTest">The p test.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns></returns>
        private static int InsidePolygon(Point2 pTest, List<Point2> polygon)
        {
            if (polygon == null)
                throw new ArgumentNullException(nameof(polygon));

            var num = 0;
            for (var i = 0; i < polygon.Count - 1; i++)
            {
                var p1 = polygon[i];
                var p2 = polygon[i + 1];

                if (p1.Y > pTest.Y) // a downward crossing
                {
                    if (p2.Y <= pTest.Y && (p2.X - p1.X) * (pTest.Y - p1.Y) - (pTest.X - p1.X) * (p2.Y - p1.Y) <= 0f)
                        num--;
                }
                else // p1.Y <= pTest.Y
                {
                    if (p2.Y > pTest.Y && (p2.X - p1.X) * (pTest.Y - p1.Y) - (pTest.X - p1.X) * (p2.Y - p1.Y) >= 0f)
                        num++;
                }
            }
            return num;
        }

        /// <summary>
        ///     Computes the distance from point a to point b
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">first point</param>
        /// <returns></returns>
        protected static float SqrDistance(Point2 a, Point2 b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }
        
        /// <summary>
        ///     Adds the loop to this triangulator.
        /// </summary>
        /// <param name="loop">The loop.</param>
        public void AddLoop(IEnumerable<Point2> loop)
        {
            Loops.Add(new List<Point2>(loop));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TrianglePoint
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="TrianglePoint" /> struct.
            /// </summary>
            /// <param name="x">X.</param>
            /// <param name="y">Y.</param>
            public TrianglePoint(float x, float y)
            {
                this.X = x;
                this.Y = y;
            }

            /// <summary>
            ///     Gets or sets the X.
            /// </summary>
            /// <value>The X.</value>
            public float X { get; }

            /// <summary>
            ///     Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            public float Y { get; }

            /// <summary>
            ///     Implements the operator *.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="v">The v.</param>
            /// <returns>The result of the operator.</returns>
            public static TrianglePoint operator *(float value, TrianglePoint v)
            {
                return new TrianglePoint(value * v.X, value * v.Y);
            }

            /// <summary>
            ///     Implements the operator *.
            /// </summary>
            /// <param name="u">The u.</param>
            /// <param name="v">The v.</param>
            /// <returns>The result of the operator.</returns>
            public static float operator *(TrianglePoint u, TrianglePoint v)
            {
                return u.X * v.X + u.Y * v.Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TriangleFace
        {
            internal readonly int v1;
            internal readonly int v2;
            internal readonly int v3;

            /// <summary>
            ///     Initializes a new instance of the <see cref="TriangleFace" /> struct.
            /// </summary>
            /// <param name="v1">The v1.</param>
            /// <param name="v2">The v2.</param>
            /// <param name="v3">The v3.</param>
            internal TriangleFace(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }

            /// <summary>
            ///     Determines the area.
            /// </summary>
            /// <param name="points">The points.</param>
            /// <returns></returns>
            public float DetermineArea(List<Point2> points)
            {
                var a = points[v1];
                var b = points[v2];
                var c = points[v3];

                var len1 = (float) Math.Sqrt(SqrDistance(a, b));
                var len2 = (float) Math.Sqrt(SqrDistance(b, c));
                var len3 = (float) Math.Sqrt(SqrDistance(c, a));

                return (len2 + len3 - len1) * (len3 + len1 - len2) * (len1 + len2 - len3) / (len1 * len2 * len3);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TriangleEdge
        {
            internal readonly int v1;
            internal readonly int v2;

            /// <summary>
            ///     Initializes a new instance of the <see cref="TriangleEdge" /> struct.
            /// </summary>
            /// <param name="v1">The v1.</param>
            /// <param name="v2">The v2.</param>
            internal TriangleEdge(int v1, int v2)
            {
                this.v1 = v1;
                this.v2 = v2;
            }
        }
    }
}