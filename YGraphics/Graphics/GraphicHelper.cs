using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace YGraphics
{
    public class GraphicHelper
    {
        internal static IEnumerable<IGraphic> Spilt(IList<Point> points, double length, IEnumerable<Segment> segments)
        {
            var spans = CalcSpans(points);
            foreach (var segment in segments)
            {
                var tuple1 = CalcTuple(Math.Min(length, segment.StartOffset), points, spans);
                var tuple2 = CalcTuple(Math.Min(length, segment.EndOffset), points, spans);
                var subPoints = new List<Point>();
                subPoints.Add(tuple1.Item1);
                subPoints.AddRange(points.Skip(tuple1.Item2).Take(tuple2.Item2 - tuple1.Item2));
                subPoints.Add(tuple2.Item1);
                yield return new GraphicPolyLine(subPoints);
            }
        }

        internal static Tuple<Point, int> CalcTuple(double offset, IList<Point> points, double[] spans)
        {
            for (int i = 0; i < spans.Length; i++)
            {
                var length = spans[i];
                if (offset > length)
                    offset -= length;
                else
                {
                    var p1 = points[i];
                    var p2 = points[i + 1];
                    var v = p2 - p1;
                    v.Normalize();
                    return new Tuple<Point, int>(p1 + v * offset, i + 1);
                }
            }
            return new Tuple<Point, int>(points[points.Count - 1], points.Count);
        }

        internal static double[] CalcSpans(IList<Point> points)
        {
            var spans = new double[points.Count - 1];
            var lastPoint = default(Point);
            var isFirst = true;
            var index = -1;
            foreach (var point in points)
            {
                if (isFirst)
                    isFirst = false;
                else spans[index] = (point - lastPoint).Length;
                lastPoint = point;
                index++;
            }
            return spans;
        }

        internal static List<Point> ToSegments(Point center, double radius, double startRadian, double endRadian, double segmentLength)
        {
            var delta = segmentLength / radius;
            var currentRadian = startRadian;
            var points = new List<Point>();
            while (currentRadian < endRadian)
            {
                points.Add(center + radius * new Vector(Math.Cos(currentRadian), Math.Sin(currentRadian)));
                currentRadian += delta;
            }
            points.Add(center + radius * new Vector(Math.Cos(endRadian), Math.Sin(endRadian)));
            return points;
        }

        internal static IEnumerable<Point> ToSegments(Point start, Point end, double segmentLength)
        {
            var v = end - start;
            var length = v.Length;
            v.Normalize();
            v *= segmentLength;
            var points = new List<Point>();
            points.Add(start);
            var currentLength = 0d;
            var currentPoint = start;
            while (true)
            {
                currentLength += segmentLength;
                currentPoint += v;
                if (!Utilities.IsClose(currentLength, length, segmentLength) && currentLength < length)
                    points.Add(currentPoint);
                else break;
            }
            points.Add(end);
            return points;
        }

        internal static IEnumerable<Point> ToSegments(IEnumerable<Point> points, double segmentLength)
        {
            var ret = new List<Point>();

            var lastPoint = default(Point);
            var isFirst = true;
            foreach (var point in points)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    var subPoints = ToSegments(lastPoint, point, segmentLength);
                    if (ret.Count == 0)
                        ret.AddRange(subPoints);
                    else ret.AddRange(subPoints.Skip(1));
                }
                lastPoint = point;
            }

            return ret;
        }

        internal static List<Point> ToSegments(double lr, double sr, double startRadian, double endRadian, double segmentLength)
        {
            var points = new List<Point>();
            var currentRadian = startRadian;
            while (currentRadian < endRadian)
            {
                points.Add(new Point(lr * Math.Cos(currentRadian), sr * Math.Sin(currentRadian)));
                currentRadian = ComputeArcTheta(lr, sr, currentRadian, segmentLength);
            }
            points.Add(new Point(lr * Math.Cos(endRadian), sr * Math.Sin(endRadian)));
            return points;
        }

        internal static List<Point> ToSegments(int degree, Point[] controlPoints, double length, double segmentLength)
        {
            var points = new List<Point>();
            var startParameter = 0d;
            var endParameter = 1d;
            var currentParameter = startParameter;
            while (currentParameter < endParameter)
            {
                points.Add(ComputePoint(controlPoints, degree, currentParameter));
                currentParameter = ComputeCurveParameter(controlPoints, degree, currentParameter, segmentLength, length);
            }
            points.Add(ComputePoint(controlPoints, degree, endParameter));
            return points;
        }

        internal static List<Point> ToSegments(int degree, Point[] controlPoints, double[] knots, double[] weights, double startParameter, double endParameter, double length, double segmentLength)
        {
            var points = new List<Point>();
            var currentParameter = startParameter;
            while (currentParameter < endParameter)
            {
                points.Add(ComputePoint(degree, knots, controlPoints, weights, currentParameter));
                currentParameter = ComputeCurveParameter(degree, knots, controlPoints, weights, currentParameter, segmentLength, length);
            }
            points.Add(ComputePoint(controlPoints, degree, endParameter));
            return points;
        }

        internal static double GetParameter(double lr, double sr, double startRadian, double length, double stride)
        {
            var currentRadian = startRadian;
            while (length > stride)
            {
                currentRadian = ComputeArcTheta(lr, sr, currentRadian, stride);
                length -= stride;
            }
            currentRadian = ComputeArcTheta(lr, sr, currentRadian, length);
            return currentRadian;
        }

        internal static double GetParameter(int degree, Point[] controlPoints, double s, double L, double stride)
        {
            var currentParameter = 0d;
            while (s > stride)
            {
                currentParameter = ComputeCurveParameter(controlPoints, degree, currentParameter, stride, L);
                s -= stride;
            }
            currentParameter = ComputeCurveParameter(controlPoints, degree, currentParameter, s, L);
            return currentParameter;
        }

        internal static double GetParameter(int degree, Point[] controlPoints, double[] knots, double[] weights, double startParameter, double s, double L, double stride)
        {
            var currentParameter = startParameter;
            while (s > stride)
            {
                currentParameter = ComputeCurveParameter(degree, knots, controlPoints, weights, currentParameter, stride, L);
                s -= stride;
            }
            currentParameter = ComputeCurveParameter(degree, knots, controlPoints, weights, currentParameter, s, L);
            return currentParameter;
        }

        internal static IGraphic Extend(IList<Point> points, double distance, JointType jointType, IEnumerable<Segment> segments)
        {
            var tuples = new List<Tuple<IGraphic, GeoLine?>>();
            var lastLine = default(GeoLine?);
            var lastOrigin = default(GeoLine?);
            var index = 0;
            foreach (var line in GetLines(points, distance))
            {
                if (lastLine.HasValue)
                {
                    var beforeOrigin = lastOrigin.Value;
                    var before = lastLine.Value;
                    var after = line;
                    var angle = before.AngleTo(after);
                    if (Utilities.IsZero(Math.Abs(angle)))
                        tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(before.P1, before.P2), beforeOrigin));
                    else
                    {
                        var flag = Utilities.IsClose(Math.Abs(angle), 180);
                        if (angle < 0 ^ distance > 0)
                        {
                            switch (jointType)
                            {
                                case JointType.Flat:
                                    {
                                        tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(before.P1, before.P2), beforeOrigin));
                                        tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(before.P2, after.P1), null));
                                    }
                                    break;
                                case JointType.Sharp:
                                    {
                                        if (flag) return null;
                                        var cp = before.Cross(after);
                                        tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(before.P1, cp.Value), beforeOrigin));
                                        after = new GeoLine(cp.Value, after.P2);
                                    }
                                    break;
                                case JointType.Rounded:
                                    {
                                        tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(before.P1, before.P2), beforeOrigin));
                                        var center = points[index];
                                        var radius = Math.Abs(distance);
                                        var startV = before.P2 - center;
                                        var endV = after.P1 - center;
                                        var startAngle = Vector.AngleBetween(new Vector(1, 0), startV) + 180;
                                        var endAngle = Vector.AngleBetween(new Vector(1, 0), endV) + 180;
                                        if (endAngle > startAngle)
                                            tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicArc(center, radius, startAngle, endAngle), null));
                                        else tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicArc(center, radius, endAngle, startAngle, true), null));
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (flag) return null;
                            var cp = before.Cross(after);
                            if (before.IsOnLine(cp.Value) && after.IsOnLine(cp.Value))
                            {
                                before = new GeoLine(before.P1, cp.Value);
                                after = new GeoLine(cp.Value, after.P2);
                                tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(before.P1, before.P2), beforeOrigin));
                            }
                            else return null;
                        }
                    }
                    lastLine = after;
                }
                else lastLine = line;
                lastOrigin = line;
                index++;
            }
            if (lastLine.HasValue)
                tuples.Add(new Tuple<IGraphic, GeoLine?>(new GraphicLine(lastLine.Value.P1, lastLine.Value.P2), lastOrigin));

            if (tuples.Count > 0)
            {
                if (segments != null)
                {
                    var graphics = new List<IGraphic>();
                    foreach (var segment in segments)
                    {
                        var subGraphics = new List<IGraphic>();
                        var currentOffset = 0d;
                        var startOffset = segment.StartOffset;
                        var endOffset = segment.EndOffset;
                        foreach (var tuple in tuples)
                        {
                            var item = tuple.Item2;
                            if (item.HasValue)
                            {
                                var line = item.Value;
                                var offset = currentOffset + line.Length;
                                if (currentOffset < endOffset && offset > startOffset)
                                {
                                    if (currentOffset >= startOffset && offset <= endOffset)
                                        subGraphics.Add(tuple.Item1);
                                    else
                                    {
                                        var newLine = (GraphicLine)tuple.Item1;
                                        var sp = newLine.Start;
                                        var ep = newLine.End;
                                        if (startOffset > currentOffset)
                                        {
                                            var p = line.GetPoint(startOffset - currentOffset);
                                            if (newLine.IsOnLine(p))
                                                sp = p;
                                        }
                                        if (endOffset < offset)
                                        {
                                            var p = line.GetPoint(endOffset - currentOffset);
                                            if (newLine.IsOnLine(p))
                                                ep = p;
                                        }
                                        subGraphics.Add(new GraphicLine(sp, ep));
                                    }
                                }
                                currentOffset = offset;
                            }
                            else subGraphics.Add(tuple.Item1);
                        }
                        graphics.Add(new GraphicCompositeCurve(subGraphics, true));
                    }
                    return new GraphicCompositeCurve(graphics, false);
                }
                else return new GraphicCompositeCurve(tuples.Select(graphic => graphic.Item1), true);
            }
            return null;
        }

        internal static IEnumerable<GeoLine> GetLines(IEnumerable<Point> points, double distance)
        {
            var lastPoint = default(Point);
            var isFirst = true;
            foreach (var point in points)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    var start = lastPoint;
                    var end = point;
                    var vec = GetVector(end - start) * distance;
                    start += vec;
                    end += vec;
                    yield return new GeoLine(start, end);
                }
                lastPoint = point;
            }
        }

        internal static Vector GetVector(Vector vec)
        {
            vec = new Vector(vec.Y, -vec.X);
            vec.Normalize();
            return vec;
        }

        public static void CalcABC(Point p1, Point p2, out double A, out double B, out double C)
        {
            var deltaY = p2.Y - p1.Y;
            var deltaX = p2.X - p1.X;
            var k = deltaY / deltaX;
            if (double.IsNaN(k))
            {
                A = 0;
                B = 0;
                C = 0;
            }
            else if (double.IsInfinity(k))
            {
                A = 1;
                B = 0;
                C = -p1.X;
            }
            else
            {
                var b = p1.Y - k * p1.X;
                A = k;
                B = -1;
                C = b;
                if (A < 0)
                {
                    A = -A;
                    B = -B;
                    C = -C;
                }
            }
        }

        public static IEnumerable<Point> FilterRepeat(IEnumerable<Point> points)
        {
            var lastPoint = default(Point);
            var isFirst = true;
            foreach (var point in points)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    if (point == lastPoint)
                        continue;
                }
                lastPoint = point;
                yield return point;
            }
        }

        public static Matrix CreateRotateMatrix(Point center, double rotateAngle)
        {
            var mat = new Matrix();
            mat.RotateAt(rotateAngle, center.X, center.Y);
            return mat;
        }

        public static bool IsContinuously(IEnumerable<IGraphic> graphics, double epsilon)
        {
            var lastGraphic = default(IGraphic);
            foreach (var graphic in graphics)
            {
                if (lastGraphic == null)
                    lastGraphic = graphic;
                else
                {
                    if ((graphic.Start - lastGraphic.End).Length > epsilon)
                        return false;
                    lastGraphic = graphic;
                }
            }
            return true;
        }

        public static double CalcLength(IEnumerable<Point> points)
        {
            var length = 0d;

            var lastPoint = default(Point);
            var isFirst = true;
            foreach (var point in points)
            {
                if (isFirst)
                    isFirst = false;
                else length += (point - lastPoint).Length;
                lastPoint = point;
            }

            return length;
        }

        #region Guass-lengendre n = 5, 代数精度为11
        private static double[] _table_w = new double[] { 0.5688888888888889, 0.4786286704993665, 0.4786286704993665, 0.2369268850561891, 0.2369268850561891 };// Weights
        private static double[] _table_x = new double[] { 0, -0.5384693101056831, 0.5384693101056831, -0.9061798459386640, 0.9061798459386640 };// Guass-Points
        private static double Epsilon = 0.0000001;
        #endregion

        #region Spline
        public static double ComputeArcLength(int degree, double[] knots, Point[] cps, double start, double end, double span)
        {
            var length = 0.0;
            var range = end - start;
            var cnt = (int)Math.Floor(range / span);
            var rem = range - cnt * span;

            var curValue = start;
            for (int i = 0; i < cnt; i++)
            {
                var toValue = curValue + span;
                length += ComputeArcLength(degree, knots, cps, start, toValue);
                curValue = toValue;
            }
            if (rem > 0)
                length += ComputeArcLength(cps, degree, curValue, end);

            return length;
        }

        public static double ComputeArcLength(int degree, double[] knots, Point[] cps, double[] weights, double start, double end, double span)
        {
            var length = 0.0;
            var range = end - start;
            var cnt = (int)Math.Floor(range / span);
            var rem = range - cnt * span;

            var curValue = start;
            for (int i = 0; i < cnt; i++)
            {
                var toValue = curValue + span;
                length += ComputeArcLength(degree, knots, cps, weights, start, toValue);
                curValue = toValue;
            }
            if (rem > 0)
                length += ComputeArcLength(degree, knots, cps, weights, curValue, end);

            return length;
        }

        public static double ComputeArcLength(int degree, double[] knots, Point[] cps, double start, double end)
        {
            var len = 0.0;
            var t1 = (end + start) / 2;
            var t2 = (end - start) / 2;
            for (int i = 0; i < 5; i++)
            {
                var vec = ComputeVector(degree, 1, knots, cps, (double)(t2 * _table_x[i] + t1));
                len += vec.Length * _table_w[i];
            }

            return len * t2;
        }

        public static double ComputeArcLength(int degree, double[] knots, Point[] cps, double[] weights, double start, double end)
        {
            var len = 0.0;
            var t1 = (end + start) / 2;
            var t2 = (end - start) / 2;
            for (int i = 0; i < 5; i++)
            {
                var vec = ComputeVector(degree, 1, knots, cps, weights, (double)(t2 * _table_x[i] + t1));
                len += vec.Length * _table_w[i];
            }

            return len * t2;
        }

        public static double ComputeCurveParameter(int degree, double[] knots, Point[] cps, double startU, double s, double L, int iterate = 5)
        {
            var low = startU;
            var high = 1d;
            var u = s / L + startU;
            if (u > high)
                u = high;

            for (int i = 0; i < iterate; i++)
            {
                var delta = ComputeArcLength(degree, knots, cps, startU, u) - s;
                if (Math.Abs(delta) < Epsilon)
                    return u;

                var df = ComputeVector(degree, 1, knots, cps, u).Length;
                var uNext = u - delta / df;
                if (delta > 0)
                {
                    high = u;
                    if (uNext <= low)
                        u = (low + high) / 2;
                    else u = uNext;
                }
                else
                {
                    low = u;
                    if (uNext >= high)
                        u = (low + high) / 2;
                    else u = uNext;
                }
            }
            return u;
        }

        public static double ComputeCurveParameter(int degree, double[] knots, Point[] cps, double[] weights, double startU, double s, double L, int iterate = 5)
        {
            var low = startU;
            var high = 1d;
            var u = s / L + startU;
            if (u > high)
                u = high;

            for (int i = 0; i < iterate; i++)
            {
                var delta = ComputeArcLength(degree, knots, cps, weights, startU, u) - s;
                if (Math.Abs(delta) < Epsilon)
                    return u;

                var df = ComputeVector(degree, 1, knots, cps, weights, u).Length;
                var uNext = u - delta / df;
                if (delta > 0)
                {
                    high = u;
                    if (uNext <= low)
                        u = (low + high) / 2;
                    else u = uNext;
                }
                else
                {
                    low = u;
                    if (uNext >= high)
                        u = (low + high) / 2;
                    else u = uNext;
                }
            }
            return u;
        }

        public static Point ComputePoint(int degree, double[] knots, Point[] controlPoints, double[] weights, double u)
        {
            if (u > 1) throw new ArgumentOutOfRangeException();
            var p = new Point();
            if (weights != null && weights.Length > 0)
                p = (Point)ComputeVector(degree, 0, knots, controlPoints, weights, u);
            else p = (Point)ComputeVector(degree, 0, knots, controlPoints, u);
            return p;
        }

        public static Vector ComputeVector(int degree, int rank, double[] knots, Point[] cps, double u)
        {
            int i = degree;
            while (knots[i + 1] < u) i++;

            var vec = new Vector();
            var w = 0d;
            for (int j = i - degree + rank; j <= i; j++)
            {
                var scale = _GetBaseFuncValue(knots, u, j, degree - rank);
                var v = _GetDI(degree, rank, j, knots, cps);
                vec += v * scale;
                w += scale;
            }

            return vec / w;
        }

        public static Vector ComputeVector(int degree, int rank, double[] knots, Point[] cps, double[] weights, double u)
        {
            int i = degree;
            while (knots[i + 1] < u) i++;

            var vec = new Vector();
            var w = 0d;
            for (int j = i - degree + rank; j <= i; j++)
            {
                var scale = _GetBaseFuncValue(knots, u, j, degree - rank) * weights[j];
                var v = _GetDI(degree, rank, j, knots, cps);
                vec += v * scale;
                w += scale;
            }

            return vec / w;
        }

        private static double _GetBaseFuncValue(double[] knots, double u, int pbase, int rank)
        {
            if (rank > 0)
            {
                return _GetRatioLeft(knots, u, pbase, rank) * _GetBaseFuncValue(knots, u, pbase, rank - 1)
                    + _GetRatioRight(knots, u, pbase, rank) * _GetBaseFuncValue(knots, u, pbase + 1, rank - 1);
            }
            else
            {
                if (u >= knots[pbase] && u <= knots[pbase + 1]) return 1;
                return 0;
            }
        }

        private static double _GetRatioLeft(double[] knots, double u, int pbase, int rank)
        {
            double up = u - knots[pbase];
            double down = knots[pbase + rank] - knots[pbase];
            if (Utilities.IsZero(down)) return 0;
            return up / down;
        }

        private static double _GetRatioRight(double[] knots, double u, int pbase, int rank)
        {
            double up = knots[pbase + rank + 1] - u;
            double down = knots[pbase + rank + 1] - knots[pbase + 1];
            if (Utilities.IsZero(down)) return 0;
            return up / down;
        }

        private static Vector _GetDI(int k, int rank, int j, double[] knots, Point[] cps)
        {
            var vec = new Vector();

            if (rank == 0)
                vec = (Vector)cps[j];
            else vec = (k - rank + 1) * (_GetDI(k, rank - 1, j, knots, cps) - _GetDI(k, rank - 1, j - 1, knots, cps)) / (knots[j + k + 1 - rank] - knots[j]);

            return vec;
        }
        #endregion

        #region Bezier
        public static double ComputeArcLength(Point[] cps, int degree, double start, double end, double span)
        {
            var length = 0.0;
            var range = end - start;
            var cnt = (int)Math.Floor(range / span);
            var rem = range - cnt * span;

            var curValue = start;
            for (int i = 0; i < cnt; i++)
            {
                var toValue = curValue + span;
                length += ComputeArcLength(cps, degree, curValue, toValue);
                curValue = toValue;
            }
            if (rem > 0)
                length += ComputeArcLength(cps, degree, curValue, end);

            return length;
        }

        public static Point ComputePoint(Point[] controlPoints, int degree, double u)
        {
            return _CalcValue(controlPoints, degree, 0, u);
        }

        public static Vector ComputeVector(Point[] cps, int degree, double u)
        {
            return degree * (_CalcValue(cps, degree - 1, 1, u) - _CalcValue(cps, degree - 1, 0, u));
        }

        public static double ComputeArcLength(Point[] cps, int degree, double start, double end)
        {
            var len = 0.0;
            var t1 = (end + start) / 2;
            var t2 = (end - start) / 2;
            for (int i = 0; i < 5; i++)
            {
                var vec = ComputeVector(cps, degree, t2 * _table_x[i] + t1);
                len += vec.Length * _table_w[i];
            }

            return len * t2;
        }

        public static double ComputeCurveParameter(Point[] cps, int degree, double startU, double s, double L, int iterate = 5)
        {
            var low = startU;
            var high = 1d;
            var u = s / L;
            Utilities.Clamp(ref u, low, high);

            for (int i = 0; i < iterate; i++)
            {
                var delta = ComputeArcLength(cps, degree, startU, u) - s;
                if (Math.Abs(delta) < Epsilon)
                    return u;

                var df = ComputeVector(cps, degree, u).Length;
                var uNext = u - delta / df;
                if (delta > 0)
                {
                    high = u;
                    if (uNext <= low)
                        u = (low + high) / 2;
                    else u = uNext;
                }
                else
                {
                    low = u;
                    if (uNext >= high)
                        u = (low + high) / 2;
                    else u = uNext;
                }
            }
            return u;
        }

        private static Point _CalcValue(Point[] controlPoints, int degree, int index, double u)
        {
            if (degree == 0)
                return controlPoints[index];
            else return _Combine(_CalcValue(controlPoints, degree - 1, index, u), _CalcValue(controlPoints, degree - 1, index + 1, u), u);
        }

        private static Point _Combine(Point p1, Point p2, double u)
        {
            var u1 = 1 - u;
            var u2 = u;
            return new Point(u1 * p1.X + u2 * p2.X, u1 * p1.Y + u2 * p2.Y);
        }
        #endregion

        #region Ellipse Arc
        public static double ComputeArcLength(double lr, double sr, double startRadian, double endRadian, double span)
        {
            var length = 0.0;
            var range = endRadian - startRadian;
            var cnt = (int)Math.Floor(range / span);
            var rem = range - cnt * span;

            var curRadian = startRadian;
            for (int i = 0; i < cnt; i++)
            {
                var toRadian = curRadian + span;
                length += ComputeArcLength(lr, sr, curRadian, toRadian);
                curRadian = toRadian;
            }
            if (rem > 0)
                length += ComputeArcLength(lr, sr, curRadian, endRadian);

            return length;
        }

        /// <summary>
        /// 求椭圆弧长
        /// </summary>
        /// <param name="lr">长轴</param>
        /// <param name="sr">短轴</param>
        /// <param name="startRadian">起始参数角</param>
        /// <param name="endRadian">结束参数角</param>
        /// <returns></returns>
        public static double ComputeArcLength(double lr, double sr, double startRadian, double endRadian)
        {
            var len = 0.0;
            var t1 = (endRadian + startRadian) / 2;
            var t2 = (endRadian - startRadian) / 2;
            var dlr = lr * lr;
            var dsr = sr * sr;
            for (int i = 0; i < 5; i++)
            {
                var xi = t2 * _table_x[i] + t1;
                len += _CalcArcFunc(dlr, dsr, xi) * _table_w[i];
            }

            return len * t2;
        }

        /// <summary>
        /// 计算 从指定参数角开始增加指定弧长后 的椭圆弧长参数角
        /// </summary>
        /// <param name="lr">长轴</param>
        /// <param name="sr">短轴</param>
        /// <param name="startRadian">起始参数角</param>
        /// <param name="s">增加的弧长</param>
        /// <param name="iterate">迭代次数</param>
        /// <returns></returns>
        public static double ComputeArcTheta(double lr, double sr, double startRadian, double s, int iterate = 5)
        {
            var dlr = lr * lr;
            var dsr = sr * sr;
            var u = s / Math.Max(lr, sr) + startRadian;
            var low = startRadian;
            var high = startRadian + s / Math.Min(lr, sr);
            for (int i = 0; i < iterate; i++)
            {
                var delta = ComputeArcLength(lr, sr, startRadian, u) - s;
                if (Math.Abs(delta) < Epsilon)
                    return u;

                var df = _CalcArcFunc(dlr, dsr, u);
                var uNext = u - delta / df;
                if (delta > 0)
                {
                    high = u;
                    if (uNext <= low)
                        u = (low + high) / 2;
                    else u = uNext;
                }
                else
                {
                    low = u;
                    if (uNext >= high)
                        u = (low + high) / 2;
                    else u = uNext;
                }
            }
            return u;
        }

        private static double _CalcArcFunc(double dlr, double dsr, double theta)
        {
            var c = Math.Cos(theta);
            var s = Math.Sin(theta);
            return Math.Sqrt(dlr * s * s + dsr * c * c);
        }
        #endregion
    }
}