using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public struct FloatGene : IGene
    {
        public FloatGene(double value)
        {
            _value = value;
        }

        object IGene.Value { get { return _value; } }

        public double Value { get { return _value; } }
        private double _value;

        public IGene Clone()
        {
            return new FloatGene(_value);
        }

        public IGene Mutation(ChromoSome chromoSome, int index)
        {
            var fChromoSome = chromoSome as FloatChromoSome;
            return new FloatGene(fChromoSome.GetValue(index));
        }

        public static bool operator ==(FloatGene first, FloatGene second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(FloatGene first, FloatGene second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (obj is FloatGene)
            {
                var gene = (FloatGene)obj;
                return _value == gene._value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}