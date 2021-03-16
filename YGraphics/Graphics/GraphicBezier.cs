using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicBezier : IGraphic
    {
        public GraphicBezier(int degree, IEnumerable<Point> controlPoints, bool isReverse = false)
        {
            if (controlPoints == null)
                throw new ArgumentNullException();

            _degree = degree;
            _controlPoints = controlPoints.ToArray();
            if (_controlPoints.Length < 2)
                throw new ArgumentException();
            _isReverse = isReverse;

            _length = GraphicHelper.ComputeArcLength(_controlPoints, _degree, 0, 1, 0.1);
        }

        public int Degree { get { return _degree; } }
        private int _degree;

        public IEnumerable<Point> ControlPoints { get { return _controlPoints; } }
        private Point[] _controlPoints;

        public GraphicType Type { get { return GraphicType.Bezier; } }

        public bool IsClosed { get { return _controlPoints[0] == _controlPoints[_controlPoints.Length - 1]; } }

        public double Length { get { return _length; } }
        private double _length;

        public bool IsReverse { get { return _isReverse; } set { _isReverse = value; } }
        private bool _isReverse;

        public Point Start { get { return _isReverse ? _controlPoints[_controlPoints.Length - 1] : _controlPoints[0]; } }

        public Point End { get { return _isReverse ? _controlPoints[0] : _controlPoints[_controlPoints.Length - 1]; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            return GraphicHelper.Extend(ToSegments(segmentLength).ToList(), distance, jointType, segments, IsClosed);
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            var points = ToSegments(segmentLength).ToList();
            return GraphicHelper.Spilt(points, _length, segments);
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            var points = GraphicHelper.ToSegments(_degree, _controlPoints, _length, segmentLength);
            if (_isReverse)
                points.Reverse();
            return points;
        }

        public Point GetPoint(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            if (_isReverse)
                length = Length - length;
            var parameter = GraphicHelper.GetParameter(_degree, _controlPoints, length, _length, segmentLength);
            return GraphicHelper.ComputePoint(_controlPoints, _degree, parameter);
        }

        public Vector GetTangent(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            if (_isReverse)
                length = Length - length;
            var parameter = GraphicHelper.GetParameter(_degree, _controlPoints, length, _length, segmentLength);
            var dir = GraphicHelper.ComputeVector(_controlPoints, _degree, parameter);
            if (_isReverse)
                dir = -dir;
            return dir;
        }
    }
}