using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGraphics
{
    public class Utilities
    {
        public static double AngleToRadian(double angle)
        {
            return angle * Math.PI / 180;
        }

        public static double RadianToAngle(double radian)
        {
            return radian * 180 / Math.PI;
        }

        public static bool IsClose(double value1, double value2, double epsilon = 1e-6)
        {
            return Math.Abs(value1 - value2) < epsilon;
        }

        public static bool IsZero(double value, double epsilon = 1e-6)
        {
            return IsClose(value, 0, epsilon);
        }

        public static bool InRange(double value, double min, double max)
        {
            return min <= value && value <= max;
        }

        public static void Clamp(ref double value, double min, double max)
        {
            if (max < min)
                throw new ArgumentOutOfRangeException();
            value = Math.Max(min, Math.Min(value, max));
        }
    }
}