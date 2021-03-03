using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class StochasticTournamentSelection : RouletteWheelSelection
    {
        protected override IList<ChromoSome> DoSelectionFromRouletteWheel(int count, IList<ChromoSome> chromoSomes, IList<double> rouletteWheel, Func<double> getValue)
        {
            var selected = new List<ChromoSome>();

            for (int i = 0; i < count; i++)
            {
                var value1 = getValue();
                var value2 = getValue();
                var index1 = Util.GetIndex(rouletteWheel, value1, (sv, tv) => sv >= tv);
                var chromoSome1 = chromoSomes[index1];
                var index2 = Util.GetIndex(rouletteWheel, value2, (sv, tv) => sv >= tv);
                var chromoSome2 = chromoSomes[index2];
                if (chromoSome1.Fitness.Value > chromoSome2.Fitness.Value)
                    selected.Add(chromoSome1);
                else selected.Add(chromoSome2);
            }

            return selected;
        }
    }
}