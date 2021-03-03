using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class BestReservationAdjustment : Adjustment
    {
        protected override IList<ChromoSome> AdjustInternal(Population population, IList<ChromoSome> children, IList<ChromoSome> parents)
        {
            var count = population.MinPopulationCount - children.Count;
            if (count > 0)
            {
                foreach (var parent in parents.OrderByDescending(p => p.Fitness.Value).Take(count))
                    children.Add(parent);
            }

            return children;
        }
    }
}