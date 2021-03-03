using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public enum ToDoAction
    {
        Continue,
        Stop,
    }

    public class GAMachine
    {
        public const double DefaultCrossoverProbability = 0.6;
        public const double DefaultMutationProbability = 0.02;

        public GAMachine(Population population, ISelection selection, ICrossover crossover, IMutation mutation, Fitness fitness, StopCondition stopCondition, double crossoverProbability = DefaultCrossoverProbability, double mutationProbability = DefaultMutationProbability)
        {
            _population = population;
            _selection = selection;
            _crossover = crossover;
            _mutation = mutation;
            _adjustment = new BestReservationAdjustment();
            _fitness = fitness;
            _stopCondition = stopCondition;
            _crossoverProbability = crossoverProbability;
            _mutationProbability = mutationProbability;
            _population.ReInit();
        }

        public double CrossoverProbability { get { return _crossoverProbability; } set { _crossoverProbability = value; } }
        private double _crossoverProbability;

        public double MutationProbability { get { return _mutationProbability; } set { _mutationProbability = value; } }
        private double _mutationProbability;

        public Population Population { get { return _population; } }
        private Population _population;

        public Fitness Fitness { get { return _fitness; } }
        private Fitness _fitness;

        public StopCondition StopCondition { get { return _stopCondition; } }
        private StopCondition _stopCondition;

        public ISelection Selection { get { return _selection; } }
        private ISelection _selection;

        public ICrossover Crossover { get { return _crossover; } }
        private ICrossover _crossover;

        public IMutation Mutation { get { return _mutation; } }
        private IMutation _mutation;

        public IAdjustment Adjustment
        {
            get { return _adjustment; }
            set
            {
                _adjustment = value;
                if (_adjustment == null)
                    throw new ArgumentNullException();
            }
        }
        private IAdjustment _adjustment;

        public void Reset()
        {
            _population.ReInit();
        }

        public ToDoAction EvolveIterate()
        {
            if (_population.GenerationCount > 1)
            {
                var parents = _Select();
                var children = _Cross(parents);
                _Mutate(children);
                children = _Adjustment(children, parents);
                _population.GenerateGeneration(children);
            }
            return _TryEndEvolveIterate() ? ToDoAction.Stop : ToDoAction.Continue;
        }

        private bool _TryEndEvolveIterate()
        {
            _Fitness();
            _population.EndUpdateCurrentGeneration();
            return _stopCondition.Invoke(_population);
        }

        private void _Fitness()
        {
            Parallel.ForEach(_population.CurrentGeneration.ChromoSomes.Where(cs => !cs.Fitness.HasValue), cs => cs.Fitness = _fitness(cs));
        }

        private IList<ChromoSome> _Select()
        {
            return _selection.DoSelection(_population.MinPopulationCount, _population.CurrentGeneration);
        }

        private IList<ChromoSome> _Cross(IList<ChromoSome> parents)
        {
            var count = _crossover.ParentCount;
            var subParents = new List<ChromoSome>();
            var children = new List<ChromoSome>(_population.MinPopulationCount);
            var index = 0;
            foreach (var parent in parents)
            {
                subParents.Add(parent);
                if (++index == count)
                {
                    if (Util.NextDouble() < _crossoverProbability)
                        children.AddRange(_crossover.Cross(subParents));
                    subParents.Clear();
                    index = 0;
                }
            }
            return children;
        }

        private void _Mutate(IList<ChromoSome> chromoSomes)
        {
            foreach (var chromoSome in chromoSomes)
                _mutation.DoMutation(chromoSome, _mutationProbability);
        }

        private IList<ChromoSome> _Adjustment(IList<ChromoSome> children, IList<ChromoSome> parents)
        {
            return _adjustment.Adjust(_population, children, parents);
        }
    }
}