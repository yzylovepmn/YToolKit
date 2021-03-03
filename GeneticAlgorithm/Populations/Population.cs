using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Population
    {
        public Population(int minPopulationCount, int maxPopulationCount, ChromoSome originChromoSome)
        {
            if (minPopulationCount < 2 || maxPopulationCount < _minPopulationCount)
                throw new ArgumentOutOfRangeException();
            _minPopulationCount = minPopulationCount;
            _maxPopulationCount = maxPopulationCount;
            _originChromoSome = originChromoSome;
            _generations = new List<Generation>();
        }

        public ChromoSome OriginChromoSome { get { return _originChromoSome; } }
        private ChromoSome _originChromoSome;

        public int MinPopulationCount { get { return _minPopulationCount; } }
        private int _minPopulationCount;

        public int MaxPopulationCount { get { return _maxPopulationCount; } }
        private int _maxPopulationCount;

        public IEnumerable<Generation> Generations { get { return _generations; } }
        private List<Generation> _generations;

        public Generation CurrentGeneration { get { return _currentGeneration; } }
        private Generation _currentGeneration;

        public ChromoSome BestChromoSome { get { return _bestChromoSome; } }
        private ChromoSome _bestChromoSome;

        public event EventHandler BestChromosomeChanged;

        public int GenerationCount { get { return _generations.Count; } }

        public void ReInit()
        {
            _bestChromoSome = null;
            _currentGeneration = null;
            _generations.Clear();

            var chromoSomes = new List<ChromoSome>();

            for (int i = 0; i < _minPopulationCount; i++)
            {
                var chromoSome = _originChromoSome.Random();
                if (chromoSome == null)
                    throw new ArgumentNullException();
                chromoSome.Verify();
                chromoSomes.Add(chromoSome);
            }

            GenerateGeneration(chromoSomes);
        }

        public void GenerateGeneration(IList<ChromoSome> chromoSomes)
        {
            if (chromoSomes == null) throw new ArgumentNullException();
            chromoSomes.Verify();
            _currentGeneration = new Generation(GenerationCount + 1, chromoSomes);
            _generations.Add(_currentGeneration);
        }

        public void EndUpdateCurrentGeneration()
        {
            _currentGeneration.EndUpdate(this);

            if (_bestChromoSome != _currentGeneration.BestChromoSome)
            {
                _bestChromoSome = _currentGeneration.BestChromoSome;

                BestChromosomeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}