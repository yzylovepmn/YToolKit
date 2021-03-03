using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public interface ISelection
    {
        IList<ChromoSome> DoSelection(int count, Generation generation);
    }
}