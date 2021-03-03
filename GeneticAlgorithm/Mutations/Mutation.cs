using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Mutation : IMutation
    {
        public void Mutate(ChromoSome chromoSome, double mutationProbability)
        {
            if (chromoSome == null) throw new ArgumentNullException();
            MutateInternal(chromoSome, mutationProbability);
        }

        protected abstract void MutateInternal(ChromoSome chromoSome, double mutationProbability);
    }
}