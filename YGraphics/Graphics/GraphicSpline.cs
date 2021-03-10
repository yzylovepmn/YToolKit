using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicSpline : IGraphic
    {
        public GraphicSpline(IEnumerable<Point> controlPoints, IEnumerable<double> knots, IEnumerable<double> weights, double startParameter, double endParameter, bool isReverse = false)
        {
            if (controlPoints == null || knots == null || weights == null)
                throw new ArgumentNullException();
            if (endParameter < startParameter)
                throw new ArgumentOutOfRangeException();

            _controlPoints = controlPoints.ToArray();
            _knots = knots.ToArray();
            _weights = weights.ToArray();
            _startParameter = startParameter;
            _endParameter = endParameter;
            _degree = _knots.Length - _controlPoints.Length - 1;
            _isReverse = isReverse;
            _length = 0;
            _start = new Point();
            _end = new Point();

            _Regular();
            _Init();
        }

        public int Degree { get { return _degree; } }
        private int _degree;

        public IEnumerable<Point> ControlPoints { get { return _controlPoints; } }
        private Point[] _controlPoints;

        public double[] Knots { get { return _knots; } }
        private double[] _knots;

        public double[] Weights { get { return _weights; } }
        private double[] _weights;

        public double StartParameter { get { return _startParameter; } }
        private double _startParameter;

        public double EndParameter { get { return _endParameter; } }
        private double _endParameter;

        public GraphicType Type { get { return GraphicType.Spline; } }

        public bool IsClosed { get { return _start == _end; } }

        public double Length { get { return _length; } }
        private double _length;

        public bool IsReverse { get { return _isReverse; } set { _isReverse = value; } }
        private bool _isReverse;

        public Point Start { get { return _isReverse ? _end : _start; } }
        private Point _start;

        public Point End { get { return _isReverse ? _start : _end; } }
        private Point _end;

        private void _Regular()
        {
            var b = -_knots[_degree];
            _startParameter += b;
            _endParameter += b;
            for (int i = 0; i < _knots.Length; i++)
                _knots[i] += b;

            b = _knots[_knots.Length - 1 - _degree];
            for (int i = 0; i < _knots.Length; i++)
                _knots[i] /= b;
            _startParameter /= b;
            _endParameter /= b;
        }

        private void _Init()
        {
            _length = GraphicHelper.ComputeArcLength(_degree, _knots, _controlPoints, _weights, _startParameter, _endParameter, 0.1);
            _start = GraphicHelper.ComputePoint(_degree, _knots, _controlPoints, _weights, _startParameter);
            _end = GraphicHelper.ComputePoint(_degree, _knots, _controlPoints, _weights, _endParameter);
        }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            if (distance == 0) return this;
            return GraphicHelper.Extend(ToSegments(segmentLength).ToList(), distance, jointType, segments);
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            var points = ToSegments(segmentLength).ToList();
            return GraphicHelper.Spilt(points, _length, segments);
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            var points = GraphicHelper.ToSegments(_degree, _controlPoints, _knots, _weights, _startParameter, _endParameter, _length, segmentLength);
            if (_isReverse)
                points.Reverse();
            return points;
        }
    }
}