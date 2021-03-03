using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Crossovers
{
    public class ArithmeticCrossover : UniformCrossover
    {
        public ArithmeticCrossover() : this(0.5, 0.5)
        {
        }

        public ArithmeticCrossover(double mixRadio, double crossProbability) : base(crossProbability)
        {
            if (mixRadio < 0 || mixRadio > 1)
                throw new ArgumentOutOfRangeException();
            _mixRadio = mixRadio;
        }

        public double MixRadio { get { return _mixRadio; } }
        private double _mixRadio;

        protected override IList<ChromoSome> CrossInternal(IList<ChromoSome> parents)
        {
            var parent1 = parents[0] as FloatChromoSome;
            var parent2 = parents[1] as FloatChromoSome;
            if (parent1 == null) return parents;

            var chromoSomeLength = parent1.Length;
            var child1 = parent1.Random() as FloatChromoSome;
            var child2 = parent2.Random() as FloatChromoSome;

            for (int i = 0; i < chromoSomeLength; i++)
            {
                var gene1 = (FloatGene)parent1[i];
                var gene2 = (FloatGene)parent2[i];
                var ret1 = gene1;
                var ret2 = gene2;
                if (Util.NextDouble() < _crossProbability)
                {
                    ret1 = new FloatGene(Util.Mix(gene1.Value, gene2.Value, _mixRadio));
                    ret2 = new FloatGene(Util.Mix(gene2.Value, gene1.Value, _mixRadio));
                }
                child1[i] = ret1;
                child2[i] = ret2;
            }

            return new List<ChromoSome>() { child1, child2 };
        }
    }
}