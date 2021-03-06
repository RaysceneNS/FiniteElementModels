using System;
using System.Collections.Generic;

namespace Core.Algorithm
{
    /// <summary>
    /// Welds the geometric primitives line segment and arc into a continuous poly shape
    /// This class is mainly used to provide input geometry to the other methods
    /// </summary>
    public class LoopBuilder
    {
        private readonly List<bool> _reverts;
        private readonly List<object> _orderedEntities;

        public LoopBuilder()
        {
            _reverts = new List<bool>();
            _orderedEntities = new List<object>();
        }

        /// <summary>
        /// Return the area of the supplied points
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static float CalculatePseudoArea(List<Point2> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            var area = 0f;
            var length = points.Count;
            for (var i = 0; i < length; i++)
            {
                var i2 = (i + 1) % length;
                area += points[i].X * points[i2].Y;
                area -= points[i].Y * points[i2].X;
            }
            area /= 2f;
            return area;
        }

        public LoopBuilder AddLineSegment(float x1, float y1, float x2, float y2)
        {
            AddEntity(new LineSegment2(x1, y1, x2, y2));
            return this;
        }

        public LoopBuilder AddArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            AddEntity(new Arc2(xCenter, yCenter, radius, startAngle, endAngle));
            return this;
        }

        public LoopBuilder AddCircle(float xCenter, float yCenter, float radius)
        {
            AddEntity(new Arc2(xCenter, yCenter, radius, 0, 360));
            return this;
        }

        public LoopBuilder AddRectangle(float xCenter, float yCenter, float width, float height)
        {
            AddLineSegment(xCenter - width, yCenter - height, xCenter + width, yCenter - height); //bl -> br
            AddLineSegment(xCenter + width, yCenter - height, xCenter + width, yCenter + height); // br -> tr
            AddLineSegment(xCenter + width, yCenter + height, xCenter - width, yCenter + height); // tr -> tl
            AddLineSegment(xCenter - width, yCenter + height, xCenter - width, yCenter - height); // tl -> bl
            return this;
        }

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

        private static Point2 GetStartPoint(object entity)
        {
            var entityType = entity.GetType();

            if (entityType == typeof(LineSegment2))
            {
                var line = (LineSegment2)entity;
                return line.Vertex1;
            }
            if (entityType == typeof(Arc2))
            {
                var arc = (Arc2)entity;
                return arc.Start();
            }
            return new Point2();
        }

        private static Point2 GetEndPoint(object entity)
        {
            var entityType = entity.GetType();

            if (entityType == typeof(LineSegment2))
            {
                var line = (LineSegment2)entity;
                return line.Vertex2;
            }
            if (entityType == typeof(Arc2))
            {
                var arc = (Arc2)entity;
                return arc.End();
            }
            return new Point2();
        }

        /// <summary>
        ///     Makes the loop.
        /// </summary>
        /// <param name="clockwise">if set to <c>true</c> [clockwise].</param>
        /// <param name="maxDistance">The max distance.</param>
        /// <returns></returns>
        public IEnumerable<Point2> Build(bool clockwise = true, float maxDistance = 1)
        {
            // construct a point set from all the entities
            var points = new List<Point2>();
            for (var i = 0; i < _orderedEntities.Count; i++)
            {
                var entity = _orderedEntities[i];
                if (entity is LineSegment2 segment2)
                    points.AddRange(GetPoints(segment2, maxDistance, _reverts[i]));
                if (entity is Arc2 arc2)
                    points.AddRange(GetPoints(arc2, maxDistance, _reverts[i]));
            }
            //close the shape
            points.Add(points[0]);

            // determine whether this shape is wound clockwise or counter-clockwise
            var area = CalculatePseudoArea(points);
            if (area < 0f && clockwise || area > 0f && !clockwise)
                points.Reverse();

            return points;
        }

        private static IEnumerable<Point2> GetPoints(LineSegment2 entity, float elementSize, bool revert)
        {
            var numElements = (int)Math.Ceiling(entity.Length / elementSize);
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

        private static IEnumerable<Point2> GetPoints(Arc2 arc, float elementSize, bool revert)
        {
            return arc.GetPoints(elementSize, revert);
        }

        private static float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180.0);
        }

        private struct Arc2
        {
            private readonly float _endAngle, _startAngle;
            private readonly float _radius;
            private readonly float _yCenter;
            private readonly float _xCenter;

            public Arc2(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
            {
                _radius = radius;
                _startAngle = startAngle;
                _endAngle = endAngle;
                _xCenter = xCenter;
                _yCenter = yCenter;
            }

            public Point2 End()
            {
                double rads = ToRadians(_endAngle);
                var p = new Point2((float)Math.Cos(rads) * _radius, (float)Math.Sin(rads) * _radius);
                return new Point2(p.X + _xCenter, p.Y + _yCenter);
            }

            public Point2 Start()
            {
                double rads = ToRadians(_startAngle);
                var p = new Point2((float)Math.Cos(rads) * _radius, (float)Math.Sin(rads) * _radius);
                return new Point2(p.X + _xCenter, p.Y + _yCenter);
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

            public IEnumerable<Point2> GetPoints(float elementSize, bool reverse)
            {
                var radStartAngle = ToRadians(_startAngle);
                var radAngle = ToRadians(DeltaAngle());
                var arcLength = _radius * radAngle;
                var elements = (int)Math.Ceiling(Math.Abs(arcLength) / elementSize);
                
                var points = new Point2[elements];
                if (reverse)
                {
                    for (var i = elements; i > 0; i--)
                    {
                        double elementAngle = radStartAngle + i * radAngle / elements;

                        if (elementAngle > Math.PI * 2)
                            elementAngle -= Math.PI * 2;

                        points[elements - i] = new Point2(
                            _xCenter + (float)(Math.Cos(elementAngle) * _radius),
                            _yCenter + (float)(Math.Sin(elementAngle) * _radius));
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
                            _xCenter + (float)(Math.Cos(elementAngle) * _radius),
                            _yCenter + (float)(Math.Sin(elementAngle) * _radius));
                    }
                }
                return points;
            }
        }
        
        private struct LineSegment2
        {
            public LineSegment2(float x1, float y1, float x2, float y2)
            {
                Vertex1 = new Point2(x1, y1);
                Vertex2 = new Point2(x2, y2);
            }

            public Point2 Vertex1 { get; }
            public Point2 Vertex2 { get; }

            public float Length
            {
                get
                {
                    var dx = Vertex1.X - Vertex2.X;
                    var dy = Vertex1.Y - Vertex2.Y;
                    return (float)Math.Sqrt(dx * dx + dy * dy);
                }
            }
        }
    }
}