﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Selection : ISelection
    {
        protected Selection(int minCount = 2)
        {
            _minCount = minCount;
        }

        private int _minCount;

        public IList<ChromoSome> Select(int count, Generation generation)
        {
            if (count < _minCount) throw new ArgumentOutOfRangeException();
            if (generation == null) throw new ArgumentNullException();
            return SelectInternal(count, generation);
        }

        protected abstract IList<ChromoSome> SelectInternal(int count, Generation generation);
    }
}