using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Crossover : ICrossover
    {
        protected Crossover(int parentCount, int childrenCount)
        {
            if (parentCount <= 0 || childrenCount <= 0)
                throw new ArgumentOutOfRangeException();
            _parentCount = parentCount;
            _childrenCount = childrenCount;
        }

        public int ParentCount { get { return _parentCount; } }
        private int _parentCount;

        public int ChildrenCount { get { return _childrenCount; } }
        private int _childrenCount;

        public abstract int MinChromoSomeLength { get; }

        public IList<ChromoSome> Cross(IList<ChromoSome> parents)
        {
            if (parents == null) throw new ArgumentNullException();
            if (parents.Count != _parentCount) throw new ArgumentOutOfRangeException();
            if (parents[0].Length < MinChromoSomeLength) throw new ArgumentOutOfRangeException();

            return CrossInternal(parents);
        }

        protected abstract IList<ChromoSome> CrossInternal(IList<ChromoSome> parents);
    }
}