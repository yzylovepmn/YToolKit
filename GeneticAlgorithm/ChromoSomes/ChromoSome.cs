using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class ChromoSome
    {
        protected ChromoSome()
        {
        }

        protected ChromoSome(IEnumerable<IGene> genes)
        {
            if (genes == null) throw new ArgumentNullException();
            _genes = genes.ToArray();
            if (_genes.Length == 0) throw new ArgumentOutOfRangeException("genes number must be larger than zero");
        }

        public int Length { get { return _genes.Length; } }

        public IEnumerable<IGene> Genes { get { return _genes; } }
        protected IGene[] _genes;

        public IGene this[int index]
        {
            get { return _genes[index]; }
            set 
            {
                if (index < 0 || index >= _genes.Length)
                    throw new ArgumentOutOfRangeException();
                _genes[index] = value;
                _fitness = null;
            }
        }

        public double? Fitness { get { return _fitness; } set { _fitness = value; } }
        protected double? _fitness;

        public abstract ChromoSome Clone();

        public abstract ChromoSome Random();

        public void Verify()
        {
            foreach (var gene in _genes)
                if (gene.Value == null)
                    throw new ArgumentNullException();
        }

        public void ReplaceGenes(int startIndex, IGene[] genes)
        {
            if (startIndex < 0 || startIndex >= _genes.Length)
                throw new ArgumentOutOfRangeException();
            if (startIndex + genes.Length - 1 >= _genes.Length)
                throw new ArgumentOutOfRangeException();

            Array.Copy(genes, 0, _genes, startIndex, genes.Length);
            _fitness = null;
        }
    }
}