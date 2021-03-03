using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class FloatChromoSome : ChromoSome
    {
        public FloatChromoSome(double[] mins, double[] maxs) : base()
        {
            if (mins.Length != maxs.Length || mins.Length == 0)
                throw new ArgumentException("Argument length is error");

            _mins = new double[mins.Length];
            _maxs = new double[maxs.Length];
            Array.Copy(mins, _mins, _mins.Length);
            Array.Copy(maxs, _maxs, _maxs.Length);

            _genes = _GenerateGenes().Cast<IGene>().ToArray();
        }

        public FloatChromoSome(double[] geneValues, double[] mins, double[] maxs) : base(geneValues.Select(value => new FloatGene(value)).Cast<IGene>())
        {
            if (geneValues.Length != mins.Length || geneValues.Length != maxs.Length)
                throw new ArgumentException("Argument length is error");
            _mins = new double[mins.Length];
            _maxs = new double[maxs.Length];
            Array.Copy(mins, _mins, _mins.Length);
            Array.Copy(maxs, _maxs, _maxs.Length);
        }

        private double[] _mins;
        private double[] _maxs;

        private IEnumerable<FloatGene> _GenerateGenes()
        {
            for (int i = 0; i < _mins.Length; i++)
                yield return new FloatGene(Util.NextDouble(_mins[i], _maxs[i]));
        }

        public override ChromoSome Clone()
        {
            return new FloatChromoSome(_genes.Select(gene => (double)gene.Value).ToArray(), _mins, _maxs);
        }

        public override ChromoSome Random()
        {
            return new FloatChromoSome(_mins, _maxs);
        }

        public double GetValue(int index)
        {
            return Util.NextDouble(_mins[index], _maxs[index]);
        }
    }
}