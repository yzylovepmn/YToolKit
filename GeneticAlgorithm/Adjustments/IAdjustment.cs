using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public interface IAdjustment
    {
        IList<ChromoSome> Adjust(Population population, IList<ChromoSome> children, IList<ChromoSome> parents);
    }
}