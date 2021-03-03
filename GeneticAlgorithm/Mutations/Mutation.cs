using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Mutation : IMutation
    {
        public void DoMutation(ChromoSome chromoSome, double mutationProbability)
        {
            if (chromoSome == null) throw new ArgumentNullException();
            DoMutationInternal(chromoSome, mutationProbability);
        }

        protected abstract void DoMutationInternal(ChromoSome chromoSome, double mutationProbability);
    }
}