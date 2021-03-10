using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGraphics
{
    public struct Segment
    {
        public Segment(double startOffset, double length)
        {
            if (startOffset < 0 || length < 0)
                throw new ArgumentOutOfRangeException();

            _startOffset = startOffset;
            _length = length;
        }

        public double StartOffset { get { return _startOffset; } }
        private double _startOffset;

        public double Count { get { return _length; } }
        private double _length;

        public double EndOffset { get { return _startOffset + _length; } }
    }
}