using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicLine : IGraphic
    {
        public GraphicLine(Point p1, Point p2)
        {
            if (p1 == p2)
                throw new ArgumentException();

            _p1 = p1;
            _p2 = p2;
        }

        public Point P1 { get { return _p1; } }
        private Point _p1;

        public Point P2 { get { return _p2; } }
        private Point _p2;

        public GraphicType Type { get { return GraphicType.Line; } }

        public bool IsClosed { get { return false; } }

        public double Length { get { return (_p1 - _p2).Length; } }

        public bool IsReverse
        { 
            get { return false; }
            set
            {
                if (value)
                {
                    var p = _p1;
                    _p1 = _p2;
                    _p2 = p;
                }
            }
        }

        public Point Start { get { return _p1; } }

        public Point End { get { return _p2; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            var start = Start;
            var end = End;
            var vec = GraphicHelper.GetVector(end - start) * distance;
            IGraphic graphic = new GraphicLine(start + vec, end + vec);
            if (segments != null)
                graphic = new GraphicCompositeCurve(graphic.Spilt(segments, segmentLength), false);
            return graphic;
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            var start = Start;
            var end = End;
            var v = end - start;
            v.Normalize();
            foreach (var segment in segments)
                yield return new GraphicLine(start + segment.StartOffset * v, start + segment.EndOffset * v);
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            return GraphicHelper.ToSegments(_p1, _p2, segmentLength);
        }

        public Point GetPoint(double length, double segmentLength)
        {
            if (length < 0 || length > Length) throw new ArgumentOutOfRangeException();
            var dir = GetTangent(length, segmentLength);
            dir.Normalize();
            return _p1 + length * dir;
        }

        public Vector GetTangent(double length, double segmentLength)
        {
            return _p2 - _p1;
        }

        internal bool IsOnLine(Point linePoint)
        {
            return new Rect(_p1, _p2).Contains(linePoint);
        }
    }
}