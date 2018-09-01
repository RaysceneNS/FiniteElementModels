using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Fem;
using Core.Geometry;

namespace Core.Algorythm
{
    /// <summary>
    /// Provides iterative meshing
    /// </summary>
    public class BasicMesher
    {
        private const double EPSILON = 1E-06;
        private readonly LinkedList<TriangleFace> _faces = new LinkedList<TriangleFace>();
        private readonly List<Point2> _improvement = new List<Point2>();
        private readonly List<List<Point2>> _loops = new List<List<Point2>>();
        private readonly List<Point2> _vertices = new List<Point2>();

        /// <summary>
        /// Creates the mesher object to perform iterative meshing of a surface
        /// </summary>
        /// <param name="elementSize">Size of the element.</param>
        /// <param name="smoothingPasses">The smoothing passes.</param>
        public BasicMesher(float elementSize = 1, int smoothingPasses = 3)
        {
            ElementSize = elementSize;
            SmoothingPasses = smoothingPasses;
            SmoothingMode = SmoothingMode.Area;
        }

        private void TriangulateInternal()
        {
            LinkedListNode<TriangleFace> first;
            LinkedListNode<TriangleFace> next;

            // clear the mesh
            _vertices.Clear();
            _faces.Clear();

            // add all the data from the loops
            foreach (var current in _loops)
            {
                if (current.Count > 0)
                {
                    var pointList = current.GetRange(0, current.Count - 1);
                    _vertices.AddRange(pointList);
                }
            }

            // add all the data from the improvement lines
            foreach (var tf in _improvement)
            {
                _vertices.Add(tf);
            }

            var verticeCount = _vertices.Count - 1;
            var minXValue = float.MaxValue;
            var maxXValue = float.MinValue;
            var minYValue = float.MaxValue;
            var maxYValue = float.MinValue;

            foreach (var p in _vertices)
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

            _vertices.Add(new Point2(midXValue - 2f * largerRange, midYValue - largerRange));
            _vertices.Add(new Point2(midXValue, midYValue + 2f * largerRange));
            _vertices.Add(new Point2(midXValue + 2f * largerRange, midYValue - largerRange));
            _faces.AddLast(new TriangleFace(verticeCount + 1, verticeCount + 2, verticeCount + 3));
            var edges = new LinkedList<TriangleEdge>();

            var index = 0;
            while (index < _vertices.Count && index <= verticeCount)
            {
                edges.Clear();
                first = _faces.First;
                do
                {
                    var face = first.Value;

                    next = first.Next;
                    var flag = InCircle(_vertices[index], _vertices[face.v1], _vertices[face.v2], _vertices[face.v3]);
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

                        _faces.Remove(first);
                    }

                    first = next;
                } while (first != null);

                var edgeNode = edges.First;
                while (edgeNode != null)
                {
                    var edge = edgeNode.Value;
                    _faces.AddLast(new TriangleFace(edge.v1, edge.v2, index));
                    edgeNode = edgeNode.Next;
                }

                index++;
            }

            // remove any faces where v1, v2 or v3 exceeed verticecount
            first = _faces.First;
            do
            {
                var face = first.Value;
                next = first.Next;
                if (face.v1 > verticeCount || face.v2 > verticeCount || face.v3 > verticeCount)
                    _faces.Remove(first);

                first = next;
            } while (first != null);


            _vertices.RemoveRange(_vertices.Count - 3, 3);
            if (_faces.Count <= 0 || _loops.Count <= 0)
                return;

            first = _faces.First;
            while (first != null)
            {
                var face = first.Value;
                first = first.Next;

                var v1 = face.v1;
                var v2 = face.v2;
                var v3 = face.v3;

                var p1 = _vertices[v1];
                var p1X = p1.X;
                var p1Y = p1.Y;

                var p2 = _vertices[v2];
                var p2X = p2.X;
                var p2Y = p2.Y;

                var p3 = _vertices[v3];
                var p3X = p3.X;
                var p3Y = p3.Y;

                var newPoint = new Point2((p1X + p2X + p3X) / 3f, (p1Y + p2Y + p3Y) / 3f);
                if (!InsidePolygons(newPoint, _loops))
                    _faces.Remove(face);
            }
        }

        /// <summary>
        ///     Return TRUE if the point (xp,yp) lies inside the circumcircle
        ///     made up by points (x1,y1) (x2,y2) (x3,y3)
        ///     NOTE: a point enabled the edge1 is inside the circumcircle
        /// </summary>
        private static bool InCircle(Point2 vertex, Point2 p1, Point2 p2, Point2 p3)
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
        /// tests each point in the lines of lists of points to see if point is within
        /// </summary>
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
        /// Test the number of times a ray project from the test point crosses the edges that form the polygon
        /// Assumes that the polygon is closed
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
                else
                {
                    if (p2.Y > pTest.Y && (p2.X - p1.X) * (pTest.Y - p1.Y) - (pTest.X - p1.X) * (p2.Y - p1.Y) >= 0f)
                        num++;
                }
            }
            return num;
        }

        private static float SqrDistance(Point2 a, Point2 b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }

        public BasicMesher AddLoop(IEnumerable<Point2> loop)
        {
            _loops.Add(new List<Point2>(loop));
            return this;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct TriangleFace
        {
            internal readonly int v1;
            internal readonly int v2;
            internal readonly int v3;

            internal TriangleFace(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }

            public float DetermineArea(IReadOnlyList<Point2> points)
            {
                var a = points[v1];
                var b = points[v2];
                var c = points[v3];

                var len1 = (float)Math.Sqrt(SqrDistance(a, b));
                var len2 = (float)Math.Sqrt(SqrDistance(b, c));
                var len3 = (float)Math.Sqrt(SqrDistance(c, a));

                return (len2 + len3 - len1) * (len3 + len1 - len2) * (len1 + len2 - len3) / (len1 * len2 * len3);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TriangleEdge
        {
            internal readonly int v1;
            internal readonly int v2;

            internal TriangleEdge(int v1, int v2)
            {
                this.v1 = v1;
                this.v2 = v2;
            }
        }

        /// <summary>
        ///     Gets the length of the min edge.
        /// </summary>
        public double MinEdgeLength { get; private set; }

        /// <summary>
        ///     Gets the length of the max edge.
        /// </summary>
        public double MaxEdgeLength { get; private set; }

        /// <summary>
        ///     Gets the min element quality.
        /// </summary>
        public double MinElementQuality { get; private set; }

        /// <summary>
        ///     Gets the size of the element.
        /// </summary>
        public float ElementSize { get; }

        /// <summary>
        ///     Gets or sets the smoothing passes.
        /// </summary>
        public int SmoothingPasses { get; }

        /// <summary>
        ///     Gets or sets the smoothing mode.
        /// </summary>
        public SmoothingMode SmoothingMode { get; }

        /// <summary>
        ///     Iterates of the mesh data
        /// </summary>
        /// <param name="firstVerticeIndex">First index of the vertice.</param>
        /// <param name="iteration">The iteration.</param>
        /// <param name="progressReport"></param>
        /// <returns></returns>
        private bool IterateMesh(int firstVerticeIndex, int iteration, IProgress<TaskProgress> progressReport)
        {
            var lines = new LinkedList<MeshLine>();
            var lastPercent = 0;

            for (var i = firstVerticeIndex; i < _vertices.Count; i++)
            {
                // determine the edges of this mesh
                lines.Clear();
                var first = _faces.First;
                while (first != null)
                {
                    var face = first.Value;
                    var next = first.Next;
                    var flag = InCircle(_vertices[i], _vertices[face.v1], _vertices[face.v2], _vertices[face.v3]);

                    if (flag)
                    {
                        var node = lines.Find(new MeshLine(face.v2, face.v1));
                        if (node != null)
                            lines.Remove(node);
                        else
                            lines.AddLast(new MeshLine(face.v1, face.v2));

                        node = lines.Find(new MeshLine(face.v3, face.v2));
                        if (node != null)
                            lines.Remove(node);
                        else
                            lines.AddLast(new MeshLine(face.v2, face.v3));

                        node = lines.Find(new MeshLine(face.v1, face.v3));
                        if (node != null)
                            lines.Remove(node);
                        else
                            lines.AddLast(new MeshLine(face.v3, face.v1));

                        _faces.Remove(first);
                    }
                    first = next;
                }

                // add all the faces
                var firstNode = lines.First;
                while (firstNode != null)
                {
                    var meshNode = firstNode.Value;
                    _faces.AddLast(new TriangleFace(meshNode.V1, meshNode.V2, i));
                    firstNode = firstNode.Next;
                }

                //report progress of this iteration
                var percentProgress = (int)(100f * (i - firstVerticeIndex) / (_vertices.Count - firstVerticeIndex));
                if (percentProgress > lastPercent)
                {
                    lastPercent = percentProgress;
                    progressReport.Report(new TaskProgress { ProgressPercentage = percentProgress, Text = ("Meshing / Iteration " + iteration + " " + percentProgress + "%...") });
                }

                //cancel if required...
                //if (backgroundWorker.CancellationPending)
                //{
                //    eventArgs.Cancel = true;
                //    return false;
                //}
            }

            progressReport.Report(new TaskProgress { ProgressPercentage = 100, Text = ("Meshing / Iteration " + iteration + " 100%...") });
            return true;
        }

        /// <summary>
        /// Asks the mesher to interatively build the mesh, generating improvements in every pass
        /// </summary>
        /// <returns></returns>
        public Model TriangulateIteratively(IProgress<TaskProgress> progressReport)
        {
            var elementSizeSquared = ElementSize * ElementSize;

            TriangulateInternal();

            // can'transform continue if no faces
            if (_faces.First == null)
                return null;

            var lines = new List<MeshLine>();
            _improvement.Capacity = 10000;

            const float improveFactor = 0.86f;

            int numImprovements;
            var iteration = 0;
            do
            {
                lines.Clear();
                var first = _faces.First;
                while (first != null)
                {
                    var face = first.Value;

                    var v1 = face.v1;
                    var v2 = face.v2;
                    var v3 = face.v3;

                    var meshLine1 = new MeshLine(v1, v2, _vertices);
                    var meshLine2 = new MeshLine(v2, v3, _vertices);
                    var meshLine3 = new MeshLine(v3, v1, _vertices);

                    if (meshLine1.SqrLength > elementSizeSquared && !lines.Contains(meshLine1))
                        lines.Add(meshLine1);

                    if (meshLine2.SqrLength > elementSizeSquared && !lines.Contains(meshLine2))
                        lines.Add(meshLine2);

                    if (meshLine3.SqrLength > elementSizeSquared && !lines.Contains(meshLine3))
                        lines.Add(meshLine3);

                    first = first.Next;
                }

                // determine if any improvements can be made
                numImprovements = 0;
                if (lines.Count > 0)
                {
                    foreach (var meshLine in lines)
                    {
                        // see if this line can be broken down into smaller elements
                        var points = meshLine.CreatePointsAlongLine(ElementSize, _vertices);

                        //test every hypothetical point generated along the line to see if it is a viable improvement 
                        foreach (var t in points)
                        {
                            var length = TestImprovement(ElementSize, t, _vertices[meshLine.V1],
                                _vertices[meshLine.V2]);
                            if (length > ElementSize * improveFactor)
                            {
                                _improvement.Add(t);
                                numImprovements++;
                            }
                        }
                    }
                }

                // add the improvements to the mesh now
                var verticeCount = _vertices.Count;
                _vertices.AddRange(_improvement);
                _improvement.Clear();

                // iterate over this mesh to join the vertices up based on delauny triangulation
                if (!IterateMesh(verticeCount, iteration + 1, progressReport))
                    return null;

                iteration++;
            }
            while (numImprovements > 0 && iteration < SmoothingPasses);


            // make smoothing passes enabled this mesh
            if (SmoothingPasses > 0)
            {
                var lastProgress = 0;

                var loopPointsCount = _loops.Sum(points => points.Count);

                for (var smoothingIteration = 0; smoothingIteration < SmoothingPasses; smoothingIteration++)
                {
                    for (var i = loopPointsCount; i < _vertices.Count; i++)
                    {
                        float x = 0;
                        float y = 0;
                        float factor = 0;

                        var triangleNode = _faces.First;
                        while (triangleNode != null)
                        {
                            var face = triangleNode.Value;
                            var v1 = face.v1;
                            var v2 = face.v2;
                            var v3 = face.v3;

                            if (v1 == i || v2 == i || v3 == i)
                            {
                                var p1 = _vertices[v1];
                                var p2 = _vertices[v2];
                                var p3 = _vertices[v3];

                                switch (SmoothingMode)
                                {
                                    case SmoothingMode.Points:
                                        if (v1 != i)
                                        {
                                            x += p1.X;
                                            y += p1.Y;
                                            factor++;
                                        }
                                        if (v2 != i)
                                        {
                                            x += p2.X;
                                            y += p2.Y;
                                            factor++;
                                        }
                                        if (v3 != i)
                                        {
                                            x += p3.X;
                                            y += p3.Y;
                                            factor++;
                                        }
                                        break;

                                    case SmoothingMode.Area:
                                        var centroid = new Point2((p1.X + p2.X + p3.X) / 3f, (p1.Y + p2.Y + p3.Y) / 3f);
                                        var area = Area2D(p1, p2, p3);
                                        if (v1 != i)
                                        {
                                            x += area * centroid.X;
                                            y += area * centroid.Y;
                                            factor += area;
                                        }
                                        if (v2 != i)
                                        {
                                            x += area * centroid.X;
                                            y += area * centroid.Y;
                                            factor += area;
                                        }
                                        if (v3 != i)
                                        {
                                            x += area * centroid.X;
                                            y += area * centroid.Y;
                                            factor += area;
                                        }
                                        break;
                                }
                            }
                            triangleNode = triangleNode.Next;
                        }

                        // add the vertice 
                        _vertices[i] = new Point2(x / factor, y / factor);

                        // report progress to date
                        var progress = (int)(100f * (i - loopPointsCount) / (_vertices.Count - loopPointsCount));
                        if (progress > lastProgress)
                        {
                            lastProgress = progress;

                            progressReport.Report(new TaskProgress { ProgressPercentage = progress, Text = "Smoothing / Iteration " + (smoothingIteration + 1) + " ..." });
                        }

                        //if (worker.CancellationPending)
                        //{
                        //    doWorkEventArgs.Cancel = true;
                        //    return false;
                        //}
                    }
                }
            }

            CalculateQuality();

            //now returns model
            return BuildModel();
        }

        /// <summary>
        ///     Calculate the signed area of the triangle
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns>
        ///     a positive area if all points wind to the right (clockwise),
        ///     0 if all points are coincident,
        ///     a negative value if points wind counter clockwise
        /// </returns>
        private static float Area2D(Point2 p1, Point2 p2, Point2 p3)
        {
            return (p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y)) / 2f;
        }

        private Model BuildModel()
        {
            var mesh = new Model(_vertices.Count, _faces.Count);
            foreach (var tf in this._vertices)
            {
                mesh.AddNode(new Node(tf.X, tf.Y));
            }
            if (this._faces.First == null)
                return mesh;

            var firstFace = _faces.First;
            while (firstFace != null)
            {
                var triangleFace = firstFace.Value;
                mesh.AddElement(new Element(triangleFace.v1, triangleFace.v3, triangleFace.v2));
                firstFace = firstFace.Next;
            }
            return mesh;
        }

        private void CalculateQuality()
        {
            // Determine the quality of this mesh
            MinEdgeLength = float.MaxValue;
            MaxEdgeLength = float.MinValue;
            MinElementQuality = float.MaxValue;

            var triangleNode = _faces.First;
            while (triangleNode != null)
            {
                var face = triangleNode.Value;
                var v1 = face.v1;
                var v2 = face.v2;
                var v3 = face.v3;

                var p1 = _vertices[v1];
                var p2 = _vertices[v2];
                var p3 = _vertices[v3];

                var dist12 = SqrDistance(p1, p2);
                var dist23 = SqrDistance(p2, p3);
                var dist31 = SqrDistance(p3, p1);

                if (dist12 < MinEdgeLength)
                    MinEdgeLength = dist12;

                if (dist23 < MinEdgeLength)
                    MinEdgeLength = dist23;

                if (dist31 < MinEdgeLength)
                    MinEdgeLength = dist31;

                if (dist12 > MaxEdgeLength)
                    MaxEdgeLength = dist12;

                if (dist23 > MaxEdgeLength)
                    MaxEdgeLength = dist23;

                if (dist31 > MaxEdgeLength)
                    MaxEdgeLength = dist31;

                var area = face.DetermineArea(_vertices);
                if (area < MinElementQuality)
                    MinElementQuality = area;

                triangleNode = triangleNode.Next;
            }
        }
        
        private double TestImprovement(float elementSize, Point2 pTest, Point2 pStart, Point2 pEnd)
        {
            Point2 point;
            float distance;
            var minDistance = float.MaxValue;

            foreach (var list in _loops)
            {
                using (var enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        point = enumerator.Current;
                        if (Math.Abs(point.X - pTest.X) < elementSize && Math.Abs(point.Y - pTest.Y) < elementSize)
                        {
                            distance = SqrDistance(pTest, point);
                            if (distance < minDistance && point != pStart && point != pEnd)
                                minDistance = distance;
                        }
                    }
                }
            }

            using (var enumerator = _improvement.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    point = enumerator.Current;
                    if (Math.Abs(point.X - pTest.X) < elementSize && Math.Abs(point.Y - pTest.Y) < elementSize)
                    {
                        distance = SqrDistance(pTest, point);
                        if (distance < minDistance && point != pStart && point != pEnd)
                            minDistance = distance;
                    }
                }
            }

            return Math.Sqrt(minDistance);
        }

        private class MeshLine
        {
            private readonly float _sqrLength;

            public MeshLine(int v1, int v2)
            {
                _sqrLength = -1f;
                V1 = v1;
                V2 = v2;
            }

            public MeshLine(int v1, int v2, IReadOnlyList<Point2> vertices)
            {
                V1 = v1;
                V2 = v2;
                _sqrLength = SqrDistance(vertices[v1], vertices[v2]);
            }

            public double SqrLength
            {
                get { return _sqrLength; }
            }

            public int V1 { get; }
            public int V2 { get; }

            public override bool Equals(object obj)
            {
                if (!(obj is MeshLine node))
                    return false;
                return V1 == node.V1 && V2 == node.V2 || V1 == node.V2 && V2 == node.V1;
            }

            public override int GetHashCode()
            {
                return V1 ^ V2;
            }

            public Point2[] CreatePointsAlongLine(double unitLength, IReadOnlyList<Point2> points)
            {
                var p1 = points[V1];
                var p2 = points[V2];

                var num = (int)Math.Ceiling(Math.Sqrt(_sqrLength) / unitLength);

                var p = new Point2[num - 1];
                for (var i = 1; i < num; i++)
                {
                    p[i - 1] = new Point2(p1.X + i * (p2.X - p1.X) / num, p1.Y + i * (p2.Y - p1.Y) / num);
                }
                return p;
            }

            public override string ToString()
            {
                return string.Concat("v1:", V1, " v2:", V2, " len:", _sqrLength);
            }
        }
    }

    public class TaskProgress
    {
        public int ProgressPercentage { get; set; }
        public string Text { get; set; }
    }

    public enum SmoothingMode
    {
        Points,
        Area
    }
}