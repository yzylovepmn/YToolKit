using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public interface ICrossover
    {
        int ParentCount { get; }

        int ChildrenCount { get; }

        int MinChromoSomeLength { get; }

        IList<ChromoSome> Cross(IList<ChromoSome> parents);
    }
}