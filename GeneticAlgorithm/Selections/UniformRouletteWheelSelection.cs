using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class UniformRouletteWheelSelection : RouletteWheelSelection
    {
        protected override IList<ChromoSome> DoSelectionInternal(int count, Generation generation)
        {
            var value = Util.NextDouble();
            var stride = 1.0 / count;
            var rouletteWheel = CalcRouletteWheel(generation.ChromoSomes);
            return DoSelectionFromRouletteWheel(count, generation.ChromoSomes, rouletteWheel, () => 
            {
                var retValue = value;
                value += stride;
                if (value > 1)
                    value -= 1;
                return retValue;
            });
        }
    }
}