using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class DoNothingAdjustment : Adjustment
    {
        protected override IList<ChromoSome> AdjustInternal(Population population, IList<ChromoSome> children, IList<ChromoSome> parents)
        {
            return children;
        }
    }
}