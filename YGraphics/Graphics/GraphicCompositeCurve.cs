using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    [Serializable]
    public struct GraphicCompositeCurve : IGraphic
    {
        public GraphicCompositeCurve(IEnumerable<IGraphic> graphics, bool isContinuously)
        {
            if (graphics == null)
                throw new ArgumentNullException();

            _graphics = graphics.ToArray();
            if (_graphics.Length == 0)
                throw new ArgumentOutOfRangeException();

            _isContinuously = isContinuously;
        }

        public IEnumerable<IGraphic> Graphics { get { return _graphics; } }
        private IGraphic[] _graphics;

        public GraphicType Type { get { return GraphicType.CompositeCurve; } }

        public bool IsClosed { get { return _isContinuously && Start == End; } }

        public double Length { get { return _graphics.Sum(graphic => graphic.Length); } }

        public bool IsContinuously { get { return _isContinuously; } }
        private bool _isContinuously;

        public bool IsReverse
        {
            get { return false; }
            set
            {
                if (value)
                {
                    Array.Reverse(_graphics);
                    Array.ForEach(_graphics, (graphic) => graphic.IsReverse = !graphic.IsReverse);
                }
            }
        }

        public Point Start { get { return _graphics[0].Start; } }

        public Point End { get { return _graphics[_graphics.Length - 1].End; } }

        public IGraphic Extend(double distance, double segmentLength, JointType jointType, IEnumerable<Segment> segments = null)
        {
            if (distance == 0) return this;
            if (_isContinuously)
                return GraphicHelper.Extend(_ToSegments(segmentLength).ToList(), distance, jointType, segments);
            else
            {
                var graphics = _graphics.Select(graphic => graphic.Extend(distance, segmentLength, jointType));
                if (graphics.Any(graphic => graphic == null)) return null;
                return new GraphicCompositeCurve(graphics, _isContinuously);
            }
        }

        public IEnumerable<IGraphic> Spilt(IEnumerable<Segment> segments, double segmentLength)
        {
            if (_isContinuously)
            {
                foreach (var segment in segments)
                {
                    var graphics = new List<IGraphic>();
                    var startOffset = segment.StartOffset;
                    var endOffset = segment.EndOffset;
                    var currentOffset = 0d;
                    for (int i = 0; i < _graphics.Length; i++)
                    {
                        var graphic = _graphics[i];
                        var offset = currentOffset + graphic.Length;
                        if (currentOffset < endOffset && offset > startOffset)
                        {
                            if (startOffset <= currentOffset && endOffset >= offset)
                                graphics.Add(graphic);
                            else
                            {
                                var sf = Math.Max(currentOffset, startOffset) - currentOffset;
                                var ef = Math.Min(offset, endOffset) - currentOffset;
                                graphics.AddRange(graphic.Spilt(new List<Segment>() { new Segment(sf, ef - sf) }, segmentLength));
                            }
                        }
                        currentOffset = offset;
                    }
                    yield return new GraphicCompositeCurve(graphics, true);
                }
            }
            else throw new InvalidOperationException();
        }

        public IEnumerable<Point> ToSegments(double segmentLength)
        {
            if (_isContinuously)
            {
                var points = new List<Point>();
                foreach (var graphic in _graphics)
                {
                    if (points.Count == 0)
                        points.AddRange(graphic.ToSegments(segmentLength));
                    else points.AddRange(graphic.ToSegments(segmentLength).Skip(1));
                }
                return points;
            }
            else throw new InvalidOperationException();
        }

        private IEnumerable<Point> _ToSegments(double segmentLength)
        {
            var points = new List<Point>();
            foreach (var graphic in _graphics)
            {
                if (points.Count == 0)
                {
                    switch (graphic.Type)
                    {
                        case GraphicType.Line:
                            var line = (GraphicLine)graphic;
                            points.Add(line.Start);
                            points.Add(line.End);
                            break;
                        case GraphicType.PolyLine:
                            var polyLine = (GraphicPolyLine)graphic;
                            points.AddRange(polyLine.Points);
                            break;
                        default:
                            points.AddRange(graphic.ToSegments(segmentLength));
                            break;
                    }
                }
                else
                {
                    switch (graphic.Type)
                    {
                        case GraphicType.Line:
                            var line = (GraphicLine)graphic;
                            points.Add(line.End);
                            break;
                        case GraphicType.PolyLine:
                            var polyLine = (GraphicPolyLine)graphic;
                            points.AddRange(polyLine.Points.Skip(1));
                            break;
                        default:
                            points.AddRange(graphic.ToSegments(segmentLength).Skip(1));
                            break;
                    }
                }
            }
            return points;
        }
    }
}