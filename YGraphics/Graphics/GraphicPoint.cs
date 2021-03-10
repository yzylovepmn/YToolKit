using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicPoint : IGraphic
    {
        public GraphicPoint(Point p)
        {
            _point = p;
        }

        public Point Point { get { return _point; } }
        private Point _point;

        public GraphicType Type { get { return GraphicType.Point; } }

        public bool IsClosed { get { return false; } }

        public double Length { get { return 0; } }

        public bool IsReverse { get { return false; } set { } }

        public Point Start { get { return _point; } }

        public Point End { get { return _point; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            throw new InvalidOperationException();
        }
    }
}