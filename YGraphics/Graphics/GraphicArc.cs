using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicArc : IGraphic
    {
        public GraphicArc(Point center, double radius, double startAngle, double endAngle, bool isClockwise = false)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException();
            if (startAngle > endAngle)
                throw new ArgumentOutOfRangeException();
            if (endAngle - startAngle > 360)
                throw new ArgumentException();

            _center = center;
            _radius = radius;
            _startAngle = startAngle;
            _endAngle = endAngle;
            _isReverse = isClockwise;
        }

        public Point Center { get { return _center; } }
        private Point _center;

        public double Radius { get { return _radius; } }
        private double _radius;

        public double StartAngle { get { return _startAngle; } }
        private double _startAngle;

        public double EndAngle { get { return _endAngle; } }
        private double _endAngle;

        public GraphicType Type { get { return GraphicType.Arc; } }

        public bool IsClosed { get { return Utilities.IsClose(360, _endAngle - _startAngle); } }

        public double Length { get { return _radius * Utilities.AngleToRadian(_endAngle - _startAngle); } }

        public bool IsReverse { get { return _isReverse; } set { _isReverse = value; } }
        private bool _isReverse;

        public Point Start
        {
            get
            {
                var radian = Utilities.AngleToRadian(_isReverse ? _endAngle : _startAngle);
                return _center + _radius * new Vector(Math.Cos(radian), Math.Sin(radian));
            }
        }

        public Point End
        {
            get
            {
                var radian = Utilities.AngleToRadian(_isReverse ? _startAngle : _endAngle);
                return _center + _radius * new Vector(Math.Cos(radian), Math.Sin(radian));
            }
        }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            var newRadius = _radius + distance;
            if (newRadius > 0)
            {
                if (segments != null)
                    return new GraphicCompositeCurve(Spilt(segments, segmentLength).Select(graphic => graphic.Extend(distance, segmentLength, jointType)), false);
                return new GraphicArc(_center, newRadius, _startAngle, _endAngle, _isReverse);
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
                var startAngle = Utilities.RadianToAngle(startOffset / _radius) + _startAngle;
                var endAngle = Utilities.RadianToAngle(endOffset / _radius) + _startAngle;
                yield return new GraphicArc(_center, _radius, startAngle, endAngle, _isReverse);
            }
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            var points = GraphicHelper.ToSegments(_center, _radius, Utilities.AngleToRadian(_startAngle), Utilities.AngleToRadian(_endAngle), segmentLength);
            if (_isReverse)
                points.Reverse();
            return points;
        }
    }
}