﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicCicle : IGraphic
    {
        public GraphicCicle(Point center, double radius, bool isClockwise = false)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException();

            _center = center;
            _radius = radius;
            _isReverse = isClockwise;
        }

        public Point Center { get { return _center; } }
        private Point _center;

        public double Radius { get { return _radius; } }
        private double _radius;

        public GraphicType Type { get { return GraphicType.Cicle; } }

        public bool IsClosed { get { return true; } }

        public double Length { get { return 2 * _radius * Math.PI; } }

        public bool IsReverse { get { return _isReverse; } set { _isReverse = value; } }
        private bool _isReverse;

        public Point Start { get { return _center + new Vector(_radius, 0); } }

        public Point End { get { return Start; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            var newRadius = _radius + distance;
            if (newRadius > 0)
            {
                if (segments != null)
                    return new GraphicCompositeCurve(Spilt(segments, segmentLength).Select(graphic => graphic.Extend(distance, segmentLength, jointType)), false);
                return new GraphicCicle(_center, newRadius, _isReverse);
            }
            return null;
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            var length = Length;
            foreach (var segment in segments)
            {
                var startOffset = segment.StartOffset;
                var endOffset = segment.EndOffset;
                if (_isReverse)
                {
                    startOffset = length - segment.EndOffset;
                    endOffset = length - segment.StartOffset;
                }
                var startAngle = Utilities.RadianToAngle(startOffset / _radius);
                var endAngle = Utilities.RadianToAngle(endOffset / _radius);
                yield return new GraphicArc(_center, _radius, startAngle, endAngle, _isReverse);
            }
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            var points = GraphicHelper.ToSegments(_center, _radius, 0, Math.PI * 2, segmentLength);
            if (_isReverse)
                points.Reverse();
            return points;
        }

        public Point GetPoint(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            var radian = length / _radius;
            if (_isReverse)
                radian = -radian;
            return _center + _radius * new Vector(Math.Cos(radian), Math.Sin(radian));
        }

        public Vector GetTangent(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            var radian = length / _radius;
            if (_isReverse)
                radian = -radian;
            var dir = new Vector(-Math.Sin(radian), Math.Cos(radian));
            if (_isReverse)
                dir = -dir;
            return dir;
        }
    }
}