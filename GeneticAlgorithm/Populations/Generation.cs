using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Generation
    {
        public Generation(int generationID, IEnumerable<ChromoSome> chromoSomes)
        {
            _generationID = generationID;
            _chromoSomes = chromoSomes.ToList();
        }

        public int GenerationID { get { return _generationID; } }
        private int _generationID;

        public IList<ChromoSome> ChromoSomes { get { return _chromoSomes; } }
        private List<ChromoSome> _chromoSomes;

        public ChromoSome BestChromoSome { get { return _bestChromoSome; } protected set { _bestChromoSome = value; } }
        private ChromoSome _bestChromoSome;

        public void EndUpdate(Population population)
        {
            var minCount = population.MinPopulationCount;
            var maxCount = population.MaxPopulationCount;

            while (_chromoSomes.Count < minCount)
                _chromoSomes.Add(_chromoSomes[Util.Next(0, _chromoSomes.Count)]);

            _chromoSomes = _chromoSomes.OrderByDescending(c => c.Fitness.Value).ToList();

            if (_chromoSomes.Count > maxCount)
                _chromoSomes = _chromoSomes.Take(maxCount).ToList();

            _bestChromoSome = _chromoSomes.First();
        }
    }
}