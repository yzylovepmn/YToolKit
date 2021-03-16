using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicEllipticalArc : IGraphic
    {
        public GraphicEllipticalArc(Point center, double lr, double sr, double startAngle, double endAngle, double rotateAngle, bool isClockwise = false)
        {
            if (lr < 0 || sr < 0)
                throw new ArgumentOutOfRangeException();
            if (startAngle > endAngle)
                throw new ArgumentOutOfRangeException();
            if (endAngle - startAngle > 360)
                throw new ArgumentException();

            _center = center;
            _lr = lr;
            _sr = sr;
            _startAngle = startAngle;
            _endAngle = endAngle;
            _rotateAngle = rotateAngle;
            _isReverse = isClockwise;
            _length = GraphicHelper.ComputeArcLength(_lr, _sr, Utilities.AngleToRadian(_startAngle), Utilities.AngleToRadian(_endAngle), Math.PI / 4);
        }

        public Point Center { get { return _center; } }
        private Point _center;

        public double LR { get { return _lr; } }
        private double _lr;

        public double SR { get { return _sr; } }
        private double _sr;

        public double StartAngle { get { return _startAngle; } }
        private double _startAngle;

        public double EndAngle { get { return _endAngle; } }
        private double _endAngle;

        public double RotateAngle { get { return _rotateAngle; } }
        private double _rotateAngle;

        public GraphicType Type { get { return GraphicType.EllipticalArc; } }

        public bool IsClosed { get { return Utilities.IsClose(360, _endAngle - _startAngle); } }

        public double Length { get { return _length; } }
        private double _length;

        public bool IsReverse { get { return _isReverse; } set { _isReverse = value; } }
        private bool _isReverse;

        public Point Start
        {
            get
            {
                var radian = Utilities.AngleToRadian(_isReverse ? _endAngle : _startAngle);
                var start = _center +  new Vector(_lr * Math.Cos(radian), _sr * Math.Sin(radian));
                return start * GraphicHelper.CreateRotateMatrix(_center, _rotateAngle);
            }
        }

        public Point End
        {
            get
            {
                var radian = Utilities.AngleToRadian(_isReverse ? _startAngle : _endAngle);
                var end = _center + new Vector(_lr * Math.Cos(radian), _sr * Math.Sin(radian));
                return end * GraphicHelper.CreateRotateMatrix(_center, _rotateAngle);
            }
        }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            var newLR = _lr + distance;
            var newSR = _sr + distance;
            if (newLR > 0 && newSR > 0)
            {
                if (segments != null)
                    return new GraphicCompositeCurve(Spilt(segments, segmentLength).Select(graphic => graphic.Extend(distance, segmentLength, jointType)), false);
                return new GraphicEllipticalArc(_center, newLR, newSR, _startAngle, _endAngle, _rotateAngle, _isReverse);
            }
            return null;
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            var startRadian = Utilities.AngleToRadian(_startAngle);
            foreach (var segment in segments)
            {
                var startOffset = segment.StartOffset;
                var endOffset = segment.EndOffset;
                if (_isReverse)
                {
                    startOffset = _length - segment.EndOffset;
                    endOffset = _length - segment.StartOffset;
                }
                var startAngle = Utilities.RadianToAngle(GraphicHelper.GetParameter(_lr, _sr, startRadian, startOffset, segmentLength));
                var endAngle = Utilities.RadianToAngle(GraphicHelper.GetParameter(_lr, _sr, startRadian, endOffset, segmentLength));
                yield return new GraphicEllipticalArc(_center, _lr, _sr, startAngle, endAngle, _rotateAngle, _isReverse);
            }
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            var vector = (Vector)_center;
            var transform = GraphicHelper.CreateRotateMatrix(_center, _rotateAngle);
            var points = GraphicHelper.ToSegments(_lr, _sr, Utilities.AngleToRadian(_startAngle), Utilities.AngleToRadian(_endAngle), segmentLength);
            if (_isReverse)
                points.Reverse();
            return points.Select(p => (p + vector) * transform);
        }

        public Point GetPoint(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            if (_isReverse)
                length = Length - length;
            var parameter = GraphicHelper.GetParameter(_lr, _sr, Utilities.AngleToRadian(_startAngle), length, segmentLength);
            return _center + new Vector(_lr * Math.Cos(parameter), _sr * Math.Sin(parameter)) * GraphicHelper.CreateRotateMatrix(_center, _rotateAngle);
        }

        public Vector GetTangent(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            if (_isReverse)
                length = Length - length;
            var parameter = GraphicHelper.GetParameter(_lr, _sr, Utilities.AngleToRadian(_startAngle), length, segmentLength);
            var dir = new Vector(-_lr * Math.Sin(parameter), _sr * Math.Cos(parameter));
            if (_isReverse)
                dir = -dir;
            return dir;
        }
    }
}