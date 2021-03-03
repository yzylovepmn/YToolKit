using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Mutations
{
    public class MultiMutation : Mutation
    {
        public MultiMutation(int count)
        {
            _count = count;
        }

        public int Count { get { return _count; } }
        private int _count;

        protected override void DoMutationInternal(ChromoSome chromoSome, double mutationProbability)
        {
            if (chromoSome.Length < _count) throw new ArgumentOutOfRangeException();
            if (Util.NextDouble() < mutationProbability)
            {
                var indice = Util.GetUniqueIndice(0, chromoSome.Length, _count);
                foreach (var index in indice)
                    chromoSome[index] = chromoSome[index].Mutation(chromoSome, index);
            }
        }
    }
}