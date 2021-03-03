using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class LimitedList<T> : IEnumerable<T>
    {
        public LimitedList() : this(20)
        {
        }

        public LimitedList(int limit)
        {
            _limit = limit;
            _data = new List<T>();
        }

        private int _limit;
        private List<T> _data;

        public int Count { get { return _data.Count; } }

        public T Last { get { return _data.LastOrDefault(); } }

        public void Add(T item)
        {
            _data.Add(item);
            _EnsureCapacity();
        }

        public void Remove(T item)
        {
            _data.Remove(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public void Resize(int newSize)
        {
            _limit = newSize;
            _EnsureCapacity();
        }

        private void _EnsureCapacity()
        {
            while (_data.Count > _limit)
                _data.RemoveAt(0);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}