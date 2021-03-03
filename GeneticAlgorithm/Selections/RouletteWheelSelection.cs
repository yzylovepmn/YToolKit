using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class RouletteWheelSelection : Selection
    {
        protected static IList<double> CalcRouletteWheel(IList<ChromoSome> chromoSomes)
        {
            var rouletteWheel = new List<double>();
            var totalValue = chromoSomes.Sum(chromoSome => chromoSome.Fitness.Value);

            var currentValue = 0.0;
            for (int i = 0; i < chromoSomes.Count - 1; i++)
            {
                currentValue += chromoSomes[i].Fitness.Value / totalValue;
                rouletteWheel.Add(currentValue);
            }
            rouletteWheel.Add(1);
            return rouletteWheel;
        }

        protected virtual IList<ChromoSome> DoSelectionFromRouletteWheel(int count, IList<ChromoSome> chromoSomes, IList<double> rouletteWheel, Func<double> getValue)
        {
            var selected = new List<ChromoSome>();

            for (int i = 0; i < count; i++)
            {
                var value = getValue();
                var index = Util.GetIndex(rouletteWheel, value, (sv, tv) => sv >= tv);
                selected.Add(chromoSomes[index]);
            }

            return selected;
        }

        protected override IList<ChromoSome> DoSelectionInternal(int count, Generation generation)
        {
            var rouletteWheel = CalcRouletteWheel(generation.ChromoSomes);
            return DoSelectionFromRouletteWheel(count, generation.ChromoSomes, rouletteWheel, () => Util.NextDouble());
        }
    }
}