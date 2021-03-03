using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public sealed class MultiCrossover : Crossover
    {
        public MultiCrossover(int[] indice) : base(2, 2)
        {
            if (indice == null) throw new ArgumentNullException();
            if (indice.Any(index => index <= 0)) throw new ArgumentOutOfRangeException();
            _indice = indice;
            Array.Sort(_indice);
            if (Util.CheckHasRepeat(_indice))
                throw new ArgumentException();
        }

        private int[] _indice;

        public override int MinChromoSomeLength { get { return _indice.Length + 1; } }

        protected override IList<ChromoSome> CrossInternal(IList<ChromoSome> parents)
        {
            var parent1 = parents[0];
            var parent2 = parents[1];
            var chromoSomeLength = parent1.Length;

            if (_indice.Any(index => index >= chromoSomeLength))
                throw new ArgumentOutOfRangeException();

            var child1 = parent1.Random();
            var child2 = parent2.Random();
            var flag = false;
            var currentIndex = 0;
            for (int i = 0; i < chromoSomeLength; i++)
            {
                var apply = false;
                if (currentIndex < _indice.Length)
                {
                    if (i < _indice[currentIndex])
                        apply = true;
                    else
                    {
                        currentIndex++;
                        flag = !flag;
                        i--;
                        continue;
                    }
                }
                else apply = true;

                if (apply)
                {
                    if (!flag)
                    {
                        child1[i] = parent1[i];
                        child2[i] = parent2[i];
                    }
                    else
                    {
                        child1[i] = parent2[i];
                        child2[i] = parent1[i];
                    }
                }
            }

            return new List<ChromoSome>() { child1, child2 };
        }
    }
}