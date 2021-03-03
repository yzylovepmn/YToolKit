using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Adjustment : IAdjustment
    {
        public IList<ChromoSome> Adjust(Population population, IList<ChromoSome> children, IList<ChromoSome> parents)
        {
            if (population == null || children == null || parents == null)
                throw new ArgumentNullException();

            return AdjustInternal(population, children, parents);
        }

        protected abstract IList<ChromoSome> AdjustInternal(Population population, IList<ChromoSome> children, IList<ChromoSome> parents);
    }
}