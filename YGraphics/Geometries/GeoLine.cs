using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGraphics
{
    public struct GeoLine
    {
        public GeoLine(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
            _direction = p2 - p1;
            Length = _direction.Length;
            _direction.Normalize();
            GraphicHelper.CalcABC(p1, p2, out A, out B, out C);
        }

        public bool IsPoint { get { return P1 == P2; } }

        public Point P1;
        public Point P2;

        public double A;
        public double B;
        public double C;

        public double Length;
        private Vector _direction;

        public bool IsOnLine(Point linePoint)
        {
            return new Rect(P1, P2).Contains(linePoint);
        }

        public Vector GetDirection()
        {
            return _direction;
        }

        public Point GetPoint(double length)
        {
            return P1 + _direction * length;
        }

        public double AngleTo(GeoLine line)
        {
            return Vector.AngleBetween(_direction, line._direction);
        }

        public Point? Cross(GeoLine line)
        {
            var crossPoint = default(Point?);

            var d = A * line.B - B * line.A;
            if (!Utilities.IsZero(d))
            {
                var p = new Point();
                p.X = (line.C * B - C * line.B) / d;
                p.Y = -(line.C * A - C * line.A) / d;
                crossPoint = p;
            }

            return crossPoint;
        }
    }
}