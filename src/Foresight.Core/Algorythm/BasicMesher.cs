using System;
using System.Collections.Generic;
using System.Linq;
using Core.Fem;
using Core.Geometry;

namespace Core.Algorythm
{
    /// <summary>
    ///     Provides iterative meshing
    /// </summary>
    public class BasicMesher : DelaunayTriangulation
    {
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

        /// <summary>
        ///     Gets the length of the min edge.
        /// </summary>
        /// <value>The length of the min edge.</value>
        public double MinEdgeLength { get; private set; }

        /// <summary>
        ///     Gets the length of the max edge.
        /// </summary>
        /// <value>The length of the max edge.</value>
        public double MaxEdgeLength { get; private set; }

        /// <summary>
        ///     Gets the min element quality.
        /// </summary>
        /// <value>The min element quality.</value>
        public double MinElementQuality { get; private set; }
        
        /// <summary>
        ///     Gets the size of the element.
        /// </summary>
        /// <scalar>The size of the element.</scalar>
        public float ElementSize { get; }

        /// <summary>
        ///     Gets or sets the smoothing passes.
        /// </summary>
        /// <scalar>The smoothing passes.</scalar>
        public int SmoothingPasses { get; }

        /// <summary>
        ///     Gets or sets the smoothing mode.
        /// </summary>
        /// <scalar>The smoothing mode.</scalar>
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

            for (var i = firstVerticeIndex; i < Vertices.Count; i++)
            {
                // determine the edges of this mesh
                lines.Clear();
                var first = Faces.First;
                while (first != null)
                {
                    var face = first.Value;
                    var next = first.Next;
                    var flag = InCircle(Vertices[i], Vertices[face.v1], Vertices[face.v2], Vertices[face.v3]);

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

                        Faces.Remove(first);
                    }
                    first = next;
                }

                // add all the faces
                var firstNode = lines.First;
                while (firstNode != null)
                {
                    var meshNode = firstNode.Value;
                    Faces.AddLast(new TriangleFace(meshNode.V1, meshNode.V2, i));
                    firstNode = firstNode.Next;
                }

                //report progress of this iteration
                var percentProgress = (int)(100f * (i - firstVerticeIndex) / (Vertices.Count - firstVerticeIndex));
                if (percentProgress > lastPercent)
                {
                    lastPercent = percentProgress;
                    progressReport.Report(new TaskProgress{ProgressPercentage = percentProgress,Text = ("Meshing / Iteration " + iteration + " " + percentProgress + "%...") });
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
            if (Faces.First == null)
                return null;

            var lines = new List<MeshLine>();
            Improvement.Capacity = 10000;

            const float improveFactor = 0.86f;

            int numImprovements;
            var iteration = 0;
            do
            {
                lines.Clear();
                var first = Faces.First;
                while (first != null)
                {
                    var face = first.Value;

                    var v1 = face.v1;
                    var v2 = face.v2;
                    var v3 = face.v3;

                    var meshLine1 = new MeshLine(v1, v2, Vertices);
                    var meshLine2 = new MeshLine(v2, v3, Vertices);
                    var meshLine3 = new MeshLine(v3, v1, Vertices);

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
                        var points = meshLine.CreatePointsAlongLine(ElementSize, Vertices);

                        //test every hypothetical point generated along the line to see if it is a viable improvement 
                        foreach (var t in points)
                        {
                            var length = TestImprovement(ElementSize, t, Vertices[meshLine.V1],
                                Vertices[meshLine.V2]);
                            if (length > ElementSize * improveFactor)
                            {
                                Improvement.Add(t);
                                numImprovements++;
                            }
                        }
                    }
                }

                // add the improvements to the mesh now
                var verticeCount = Vertices.Count;
                Vertices.AddRange(Improvement);
                Improvement.Clear();

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

                var loopPointsCount = Loops.Sum(points => points.Count);

                for (var smoothingIteration = 0; smoothingIteration < SmoothingPasses; smoothingIteration++)
                {
                    for (var i = loopPointsCount; i < Vertices.Count; i++)
                    {
                        float x = 0;
                        float y = 0;
                        float factor = 0;

                        var triangleNode = Faces.First;
                        while (triangleNode != null)
                        {
                            var face = triangleNode.Value;
                            var v1 = face.v1;
                            var v2 = face.v2;
                            var v3 = face.v3;

                            if (v1 == i || v2 == i || v3 == i)
                            {
                                var p1 = Vertices[v1];
                                var p2 = Vertices[v2];
                                var p3 = Vertices[v3];

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
                        Vertices[i] = new Point2(x / factor, y / factor);

                        // report progress to date
                        var progress = (int)(100f * (i - loopPointsCount) / (Vertices.Count - loopPointsCount));
                        if (progress > lastProgress)
                        {
                            lastProgress = progress;

                            progressReport.Report(new TaskProgress{ProgressPercentage = progress, Text = "Smoothing / Iteration " + (smoothingIteration + 1) + " ..." });
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
            var mesh = new Model(Vertices.Count, Faces.Count);
            foreach (var tf in this.Vertices)
            {
                mesh.AddNode(new Node(tf.X, tf.Y));
            }
            if (this.Faces.First == null)
                return mesh;

            var firstFace = Faces.First;
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

            var triangleNode = Faces.First;
            while (triangleNode != null)
            {
                var face = triangleNode.Value;
                var v1 = face.v1;
                var v2 = face.v2;
                var v3 = face.v3;

                var p1 = Vertices[v1];
                var p2 = Vertices[v2];
                var p3 = Vertices[v3];

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

                var area = face.DetermineArea(Vertices);
                if (area < MinElementQuality)
                    MinElementQuality = area;

                triangleNode = triangleNode.Next;
            }
        }

        /// <summary>
        ///     Tests the improvement.
        /// </summary>
        /// <param name="elementSize">Size of the element.</param>
        /// <param name="pTest">The p test.</param>
        /// <param name="pStart">The p start.</param>
        /// <param name="pEnd">The p end.</param>
        /// <returns></returns>
        private double TestImprovement(float elementSize, Point2 pTest, Point2 pStart, Point2 pEnd)
        {
            Point2 point;
            float distance;
            var minDistance = float.MaxValue;

            foreach (var list in Loops)
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

            using (var enumerator = Improvement.GetEnumerator())
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

            /// <summary>
            ///     Initializes a new instance of the <see cref="MeshLine" /> class.
            /// </summary>
            /// <param name="v1">The v1.</param>
            /// <param name="v2">The v2.</param>
            public MeshLine(int v1, int v2)
            {
                _sqrLength = -1f;
                V1 = v1;
                V2 = v2;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="MeshLine" /> class.
            /// </summary>
            /// <param name="v1">The v1.</param>
            /// <param name="v2">The v2.</param>
            /// <param name="vertices">The vertices.</param>
            public MeshLine(int v1, int v2, IReadOnlyList<Point2> vertices)
            {
                V1 = v1;
                V2 = v2;
                _sqrLength = SqrDistance(vertices[v1], vertices[v2]);
            }

            /// <summary>
            ///     Gets the length of the SQR.
            /// </summary>
            /// <value>The length of the SQR.</value>
            public double SqrLength
            {
                get { return _sqrLength; }
            }

            /// <summary>
            ///     Gets the v1.
            /// </summary>
            /// <value>The v1.</value>
            public int V1 { get; }

            /// <summary>
            ///     Gets the v2.
            /// </summary>
            /// <value>The v2.</value>
            public int V2 { get; }

            /// <summary>
            ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            /// <exception cref="T:System.NullReferenceException">
            ///     The <paramref name="obj" /> parameter is null.
            /// </exception>
            public override bool Equals(object obj)
            {
                var node = obj as MeshLine;
                if (node == null)
                    return false;
                return V1 == node.V1 && V2 == node.V2 || V1 == node.V2 && V2 == node.V1;
            }

            /// <summary>
            ///     Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                return V1 ^ V2;
            }

            /// <summary>
            ///     Creates the points along line.
            /// </summary>
            /// <param name="unitLength">Length of the unit.</param>
            /// <param name="points">The points.</param>
            /// <returns></returns>
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

            /// <summary>
            ///     Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            ///     A <see cref="System.String" /> that represents this instance.
            /// </returns>
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