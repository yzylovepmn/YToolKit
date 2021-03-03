using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class UniformCrossover : Crossover
    {
        public UniformCrossover() : this(0.5)
        {
        }

        public UniformCrossover(double crossProbability) : base(2, 2)
        {
            if (crossProbability < 0 || crossProbability > 1)
                throw new ArgumentOutOfRangeException();
            _crossProbability = crossProbability;
        }

        public double CrossProbability { get { return _crossProbability; } }
        protected double _crossProbability;

        public override int MinChromoSomeLength { get { return 1; } }

        protected override IList<ChromoSome> CrossInternal(IList<ChromoSome> parents)
        {
            var parent1 = parents[0];
            var parent2 = parents[1];
            var chromoSomeLength = parent1.Length;
            var child1 = parent1.Random();
            var child2 = parent2.Random();

            for (int i = 0; i < chromoSomeLength; i++)
            {
                if (Util.NextDouble() < _crossProbability)
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                }
                else
                {
                    child1[i] = parent1[i];
                    child2[i] = parent2[i];
                }
            }

            return new List<ChromoSome>() { child1, child2 };
        }
    }
}