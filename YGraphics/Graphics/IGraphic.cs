using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    public interface IGraphic
    {
        GraphicType Type { get; }

        bool IsClosed { get; }

        double Length { get; }

        bool IsReverse { get; set; }

        Point Start { get; }

        Point End { get; }

        IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null);

        IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength);

        IEnumerable<Point> ToSegments(double segmentLength);

        Point GetPoint(double length, double segmentLength);

        Vector GetTangent(double length, double segmentLength);
    }
}