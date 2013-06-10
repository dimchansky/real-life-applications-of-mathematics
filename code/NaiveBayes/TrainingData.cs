using System;
using System.Collections.Generic;

namespace NaiveBayes
{
    public static class TrainingData
    {
        public static TrainingData<TClass, TValue> Create<TClass, TValue>(IEnumerable<Tuple<TClass, TValue>> enumerable)
        {
            return new TrainingData<TClass, TValue>(enumerable);
        }
    }

    public sealed class TrainingData<TCategory, TAttributes> : IEnumerable<Tuple<TCategory, TAttributes>>
    {
        private readonly List<Tuple<TCategory, TAttributes>> _data = new List<Tuple<TCategory, TAttributes>>();

        public TrainingData()
        {
        }

        public TrainingData(IEnumerable<Tuple<TCategory, TAttributes>> enumerable)
        {
            foreach (var pair in enumerable)
            {
                _data.Add(pair);
            }
        }

        public IEnumerator<Tuple<TCategory, TAttributes>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Tuple<TCategory, TAttributes> categroyAttributes)
        {
            _data.Add(categroyAttributes);
        }

        public void Add(TCategory category, TAttributes attributes)
        {
            _data.Add(new Tuple<TCategory, TAttributes>(category, attributes));
        }
    }
}