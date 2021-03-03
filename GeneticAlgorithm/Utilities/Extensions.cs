using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public static class Extensions
    {
        public static void Verify(this IEnumerable<ChromoSome> chromoSomes)
        {
            foreach (var chromoSome in chromoSomes)
                chromoSome.Verify();
        }
    }
}