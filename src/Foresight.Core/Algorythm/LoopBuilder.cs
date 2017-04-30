using System;
using System.Collections.Generic;
using Core.Geometry;
using Core.MathLib;

namespace Core.Algorythm
{
    /// <summary>
    ///     Welds the geometric primitives line segment and arc into a continuous polyshape
    ///     This class is mainly used to provide input geometry to the other methods
    /// </summary>
    public class LoopBuilder
    {
        private readonly List<bool> _reverts;
        private readonly List<object> _orderedEntities;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoopBuilder" /> class.
        /// </summary>
        public LoopBuilder()
        {
            _reverts = new List<bool>();
            _orderedEntities = new List<object>();
        }

        /// <summary>
        ///     Return the area of the supplied points
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static float CalculatePsuedoArea(Point2[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            var area = 0f;
            var length = points.Length;
            for (var i = 0; i < length; i++)
            {
                var i2 = (i + 1) % length;
                area += points[i].X * points[i2].Y;
                area -= points[i].Y * points[i2].X;
            }
            area /= 2f;
            return area;
        }

        /// <summary>
        ///     Adds the primitive.
        /// </summary>
        public void AddLineSegment (float x1, float y1, float x2, float y2)
        {
            AddEntity(new LineSegment2(x1, y1, x2, y2));
        }

        /// <summary>
        ///     Adds the primitive.
        /// </summary>
        public void AddArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            AddEntity(new Arc2(xCenter, yCenter, radius, startAngle, endAngle));
        }

        /// <summary>
        ///     Adds the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void AddEntity(object entity)
        {
            if (_orderedEntities.Count == 0)
            {
                _reverts.Add(false);
                _orderedEntities.Add(entity);
            }
            else
            {
                // move through all the entities ordering the segments up to form a continuous loop
                var lastEntity = _orderedEntities[_orderedEntities.Count - 1];

                if (GetStartPoint(entity) == GetEndPoint(lastEntity))
                {
                    _orderedEntities.Add(entity);
                    _reverts.Add(false);
                }
                else if (GetEndPoint(entity) == GetStartPoint(lastEntity))
                {
                    _orderedEntities.Add(entity);
                    _reverts.Add(true);
                }
                else if (GetStartPoint(entity) == GetStartPoint(lastEntity))
                {
                    _orderedEntities.Add(entity);
                    _reverts.Add(false);
                }
                else if (GetEndPoint(entity) == GetEndPoint(lastEntity))
                {
                    _orderedEntities.Add(entity);
                    _reverts.Add(true);
                }
            }
        }

        /// <summary>
        /// Gets the start point.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private static Point2 GetStartPoint(object entity)
        {
            var entityType = entity.GetType();

            if (entityType == typeof(LineSegment2))
            {
                var line = (LineSegment2) entity;
                return line.Vertex1;
            }
            if (entityType == typeof(Arc2))
            {
                var arc = (Arc2) entity;
                return arc.Start;
            }
            return new Point2();
        }

        /// <summary>
        ///     Gets the end point.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private static Point2 GetEndPoint(object entity)
        {
            var entityType = entity.GetType();

            if (entityType == typeof(LineSegment2))
            {
                var line = (LineSegment2) entity;
                return line.Vertex2;
            }
            if (entityType == typeof(Arc2))
            {
                var arc = (Arc2) entity;
                return arc.End;
            }
            return new Point2();
        }

        /// <summary>
        ///     Makes the loop.
        /// </summary>
        /// <param name="clockwise">if set to <c>true</c> [clockwise].</param>
        /// <param name="maxDistance">The max distance.</param>
        /// <returns></returns>
        public Point2[] Build(bool clockwise, float maxDistance)
        {
            // construct a pointset from all the entities
            var points = new List<Point2>();
            for (var i = 0; i < _orderedEntities.Count; i++)
            {
                var entity = _orderedEntities[i];
                if (entity is LineSegment2)
                    points.AddRange(GetPoints((LineSegment2) entity, maxDistance, _reverts[i]));
                if (entity is Arc2)
                    points.AddRange(GetPoints((Arc2) entity, maxDistance, _reverts[i]));
            }
            //close the shape
            points.Add(points[0]);

            // determine whether this shape is wound clockwise or counter-clockwise
            var area = CalculatePsuedoArea(points.ToArray());
            if (area < 0f && clockwise || area > 0f && !clockwise)
                points.Reverse();

            return points.ToArray();
        }

        /// <summary>
        ///     Gets the points.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="elementSize">Size of the element.</param>
        /// <param name="revert">if set to <c>true</c> [revert].</param>
        /// <returns></returns>
        private static IEnumerable<Point2> GetPoints(LineSegment2 entity, float elementSize, bool revert)
        {
            var numElements = (int) Math.Ceiling(entity.Length / elementSize);
            var points = new Point2[numElements];
            var start = entity.Vertex1;
            var end = entity.Vertex2;

            if (revert)
            {
                for (var i = numElements; i > 0; i--)
                {
                    points[numElements - i] = new Point2(start.X + i * (end.X - start.X) / numElements, 
                                                         start.Y + i * (end.Y - start.Y) / numElements);
                }
            }
            else
            {
                for (var i = 0; i < numElements; i++)
                {
                    points[i] = new Point2(start.X + i * (end.X - start.X) / numElements,
                                           start.Y + i * (end.Y - start.Y) / numElements);
                }
            }
            return points;
        }

        /// <summary>
        ///     Gets the points.
        /// </summary>
        /// <param name="arc">The arc.</param>
        /// <param name="elementSize">Size of the element.</param>
        /// <param name="revert">if set to <c>true</c> [revert].</param>
        /// <returns></returns>
        private static IEnumerable<Point2> GetPoints(Arc2 arc, float elementSize, bool revert)
        {
            return arc.GetPoints(elementSize, revert);
        }
        
        private struct Arc2
        {
            private readonly float _endAngle, _startAngle;
            private Matrix4X4 _matrix;
            private readonly float _radius;

            /// <summary>
            /// Create an arc
            /// </summary>
            /// <param name="xCenter">The x center.</param>
            /// <param name="yCenter">The y center.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="startAngle">The start angle.</param>
            /// <param name="endAngle">The end angle.</param>
            public Arc2(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
            {
                _radius = radius;
                _startAngle = startAngle;
                _endAngle = endAngle;
                _matrix = Matrix4X4.CreateTranslation(xCenter, yCenter, 0);
            }
            
            /// <summary>
            ///     Returnt he end point for this arc
            /// </summary>
            public Point2 End
            {
                get
                {
                    double rads = Degrees.ToRadians(_endAngle);
                    var p = new Point2((float)Math.Cos(rads) * _radius, (float)Math.Sin(rads) * _radius);
                    return (Point2)(_matrix * (Point3)p);
                }
            }

            /// <summary>
            ///     Gets the angle between start and end in degrees.
            /// </summary>
            /// <value>The angle.</value>
            private float DeltaAngle()
            {
                var angle = _endAngle - _startAngle;
                if (angle < 0f)
                    angle = 360f - _startAngle + _endAngle;
                return angle;
            }
            
            /// <summary>
            ///     Return the start point
            /// </summary>
            public Point2 Start
            {
                get
                {
                    double rads = Degrees.ToRadians(_startAngle);
                    var p = new Point2((float)Math.Cos(rads) * _radius, (float)Math.Sin(rads) * _radius);
                    return (Point2)(_matrix * (Point3)p);
                }
            }

            /// <summary>
            /// Gets the points.
            /// </summary>
            /// <param name="elementSize">Size of the element.</param>
            /// <param name="reverse">if set to <c>true</c> [reverse].</param>
            /// <returns></returns>
            public Point2[] GetPoints(float elementSize, bool reverse)
            {
                var radStartAngle = Degrees.ToRadians(_startAngle);
                var radAngle = Degrees.ToRadians(DeltaAngle());
                var arcLength = _radius * radAngle;
                var elements = (int)Math.Ceiling(Math.Abs(arcLength) / elementSize);
                var center = (Point2) _matrix.ExtractPosition();

                var points = new Point2[elements];
                if (reverse)
                {
                    for (var i = elements; i > 0; i--)
                    {
                        double elementAngle = radStartAngle + i * radAngle / elements;

                        if (elementAngle > Math.PI * 2)
                            elementAngle -= Math.PI * 2;

                        points[elements - i] = new Point2(
                            center.X + (float)(Math.Cos(elementAngle) * _radius),
                            center.Y + (float)(Math.Sin(elementAngle) * _radius));
                    }
                }
                else
                {
                    for (var i = 0; i < elements; i++)
                    {
                        double elementAngle = radStartAngle + i * radAngle / elements;

                        if (elementAngle > Math.PI * 2)
                            elementAngle -= Math.PI * 2;

                        points[i] = new Point2(
                            center.X + (float)(Math.Cos(elementAngle) * _radius),
                            center.Y + (float)(Math.Sin(elementAngle) * _radius));
                    }
                }
                return points;
            }
        }

        private struct LineSegment2
        {
            private readonly Point2 _vertex1, _vertex2;
            
            /// <summary>
            ///     Initializes a new instance of the <see cref="LineSegment2" /> struct.
            /// </summary>
            /// <param name="x1">The x1.</param>
            /// <param name="y1">The y1.</param>
            /// <param name="x2">The x2.</param>
            /// <param name="y2">The y2.</param>
            public LineSegment2(float x1, float y1, float x2, float y2)
            {
                _vertex1 = new Point2(x1, y1);
                _vertex2 = new Point2(x2, y2);
            }
            
            /// <summary>
            ///     Gets the vertex1.
            /// </summary>
            /// <value>The vertex1.</value>
            public Point2 Vertex1
            {
                get { return _vertex1; }
            }

            /// <summary>
            ///     Gets the vertex2.
            /// </summary>
            /// <value>The vertex2.</value>
            public Point2 Vertex2
            {
                get { return _vertex2; }
            }

            /// <summary>
            ///     Gets the length.
            /// </summary>
            /// <value>The length.</value>
            public float Length
            {
                get { return Point2.Distance(_vertex1, _vertex2); }
            }
        }
    }
}