using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicEllipse : IGraphic
    {
        public GraphicEllipse(Point center, double lr, double sr, double rotateAngle, bool isClockwise = false)
        {
            if (lr <= 0 || sr <= 0)
                throw new ArgumentOutOfRangeException();

            _center = center;
            _lr = lr;
            _sr = sr;
            _rotateAngle = rotateAngle;
            _isReverse = isClockwise;
            _length = GraphicHelper.ComputeArcLength(_lr, _sr, 0, Math.PI * 2, Math.PI / 4);
        }

        public Point Center { get { return _center; } }
        private Point _center;

        public double LR { get { return _lr; } }
        private double _lr;

        public double SR { get { return _sr; } }
        private double _sr;

        public double RotateAngle { get { return _rotateAngle; } }
        private double _rotateAngle;

        public GraphicType Type { get { return GraphicType.Ellipse; } }

        public bool IsClosed { get { return true; } }

        public double Length { get { return _length; } }
        private double _length;

        public bool IsReverse { get { return _isReverse; } set { _isReverse = value; } }
        private bool _isReverse;

        public Point Start
        {
            get
            {
                var radian = Utilities.AngleToRadian(_rotateAngle);
                return _center + _lr * new Vector(Math.Cos(radian), Math.Sin(radian));
            }
        }

        public Point End { get { return Start; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            var newLR = _lr + distance;
            var newSR = _sr + distance;
            if (newLR > 0 && newSR > 0)
            {
                if (segments != null)
                    return new GraphicCompositeCurve(Spilt(segments, segmentLength).Select(graphic => graphic.Extend(distance, segmentLength, jointType)), false);
                return new GraphicEllipse(_center, newLR, newSR, _rotateAngle, _isReverse);
            }
            return null;
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            foreach (var segment in segments)
            {
                var startOffset = segment.StartOffset;
                var endOffset = segment.EndOffset;
                if (_isReverse)
                {
                    startOffset = _length - segment.EndOffset;
                    endOffset = _length - segment.StartOffset;
                }
                var startAngle = Utilities.RadianToAngle(GraphicHelper.GetParameter(_lr, _sr, 0, startOffset, segmentLength));
                var endAngle = Utilities.RadianToAngle(GraphicHelper.GetParameter(_lr, _sr, 0, endOffset, segmentLength));
                yield return new GraphicEllipticalArc(_center, _lr, _sr, startAngle, endAngle, _rotateAngle, _isReverse);
            }
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            var vector = (Vector)_center;
            var transform = GraphicHelper.CreateRotateMatrix(_center, _rotateAngle);
            var points = GraphicHelper.ToSegments(_lr, _sr, 0, Math.PI * 2, segmentLength);
            if (_isReverse)
                points.Reverse();
            return points.Select(p => (p + vector) * transform);
        }

        public Point GetPoint(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            if (_isReverse)
                length = Length - length;
            var parameter = GraphicHelper.GetParameter(_lr, _sr, 0, length, segmentLength);
            return _center + new Vector(_lr * Math.Cos(parameter), _sr * Math.Sin(parameter)) * GraphicHelper.CreateRotateMatrix(_center, _rotateAngle);
        }

        public Vector GetTangent(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            if (_isReverse)
                length = Length - length;
            var parameter = GraphicHelper.GetParameter(_lr, _sr, 0, length, segmentLength);
            var dir = new Vector(-_lr * Math.Sin(parameter), _sr * Math.Cos(parameter));
            if (_isReverse)
                dir = -dir;
            return dir;
        }
    }
}