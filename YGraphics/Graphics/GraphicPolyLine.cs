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
            if (distance == 0) return this;
            return GraphicHelper.Extend(_points, distance, jointType, segments);
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            return GraphicHelper.Spilt(_points, _length, segments);
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            return GraphicHelper.ToSegments(_points, segmentLength);
        }
    }
}