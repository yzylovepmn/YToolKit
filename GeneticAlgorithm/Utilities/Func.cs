using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public delegate double Fitness(ChromoSome chromoSome);

    public delegate bool StopCondition(Population population);
}