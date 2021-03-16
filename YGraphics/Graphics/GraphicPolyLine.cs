using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicPolyLine : IGraphic
    {
        public GraphicPolyLine(IEnumerable<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            _points = GraphicHelper.FilterRepeat(points).ToList();
            if (_points.Count < 2)
                throw new ArgumentOutOfRangeException();

            _length = GraphicHelper.CalcLength(_points);
        }

        public IEnumerable<Point> Points { get { return _points; } }
        private List<Point> _points;

        public int PointCount { get { return _points.Count; } }

        public GraphicType Type { get { return GraphicType.PolyLine; } }

        public bool IsClosed { get { return Start == End; } }

        public double Length { get { return _length; } }
        private double _length;

        public bool IsReverse
        {
            get { return false; }
            set
            {
                if (value)
                    _points.Reverse();
            }
        }

        public Point Start { get { return _points[0]; } }

        public Point End { get { return _points[_points.Count - 1]; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            return GraphicHelper.Extend(_points, distance, jointType, segments, IsClosed);
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            return GraphicHelper.Spilt(_points, _length, segments);
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            return GraphicHelper.ToSegments(_points, segmentLength);
        }

        public Point GetPoint(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            var lastPoint = default(Point);
            var isFirst = true;
            var ret = End;
            foreach (var point in _points)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    var vec = point - lastPoint;
                    var subLength = vec.Length;
                    if (length > subLength)
                        length -= subLength;
                    else
                    {
                        vec.Normalize();
                        ret = lastPoint + vec * length;
                        break;
                    }
                }
                lastPoint = point;
            }
            return ret;
        }

        public Vector GetTangent(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            var lastPoint = default(Point);
            var isFirst = true;
            var ret = _points[_points.Count - 1] - _points[_points.Count - 2];
            foreach (var point in _points)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    var vec = point - lastPoint;
                    var subLength = vec.Length;
                    if (length > subLength)
                        length -= subLength;
                    else
                    {
                        ret = vec;
                        break;
                    }
                }
                lastPoint = point;
            }
            return ret;
        }
    }
}