using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Util
    {
        private static Random _random;

        static Util()
        {
            _random = new Random(Environment.TickCount);
        }

        public static double NextDouble(double min, double max)
        {
            var span = max - min;
            return min + _random.NextDouble() * span;
        }

        public static double NextDouble()
        {
            return _random.NextDouble();
        }

        public static int Next()
        {
            return _random.Next();
        }

        public static int Next(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static int[] GetUniqueIndice(int min, int max, int count)
        {
            var span = max - min;
            if (count > span) throw new ArgumentOutOfRangeException();
            var uniqueValues = Enumerable.Range(min, span).ToList();

            var indice = new int[count];
            for (int i = 0; i < count; i++)
            {
                var index = Next(0, uniqueValues.Count);
                indice[i] = uniqueValues[index];
                uniqueValues.RemoveAt(index);
            }
            return indice;
        }

        public static int GetIndex(IEnumerable<double> source, double value, Func<double, double, bool> func)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (func(item, value))
                    return index;
                index++;
            }
            return -1;
        }

        public static bool CheckHasRepeat(int[] source)
        {
            if (source == null) throw new ArgumentNullException();
            if (source.Length < 2)
                return false;
            var last = source[0];
            for (int i = 1; i < source.Length; i++)
            {
                var current = source[i];
                if (last == current)
                    return true;
                last = current;
            }
            return false;
        }

        public static double Mix(double v1, double v2, double mixRadio)
        {
            return v1 * mixRadio + v2 * (1 - mixRadio);
        }
    }
}