using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public interface IGene
    {
        object Value { get; }

        IGene Clone();

        IGene Mutation(ChromoSome chromoSome, int index);
    }
}