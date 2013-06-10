using System;
using System.Collections.Generic;
using Numerics;

namespace NaiveBayes
{
    public static class NaiveBayes
    {
        public static NaiveBayes<TCategory, TAttribute> Create<TCategory, TAttribute>(IEnumerable<Tuple<TCategory, TAttribute[]>> trainingData, long laplaceSmoothingK = 1L)
        {
            return new NaiveBayes<TCategory, TAttribute>(trainingData, laplaceSmoothingK);
        }
    }

    public sealed class NaiveBayes<TCategory, TAttribute>
    {
        private readonly long _laplaceSmoothingK;

        private readonly HashSet<TCategory> _knownCategories = new HashSet<TCategory>();

        private readonly HashSet<TAttribute> _knownAttributes = new HashSet<TAttribute>();

        private readonly Dictionary<Tuple<TCategory, TAttribute>, long> _categoryAttributeCount = new Dictionary<Tuple<TCategory, TAttribute>, long>();
        private readonly Dictionary<TCategory, long> _categoryAttributesCount = new Dictionary<TCategory, long>();
        private readonly Dictionary<TCategory, long> _categorySamplesCount = new Dictionary<TCategory, long>();

        private long _totalSamplesCount;

        public long LaplaceSmoothingK
        {
            get
            {
                return _laplaceSmoothingK;
            }
        }

        public long TotalSamplesCount
        {
            get
            {
                return _totalSamplesCount;
            }
        }

        public NaiveBayes(IEnumerable<Tuple<TCategory, TAttribute[]>> trainingData, long laplaceSmoothingK = 1L)
        {
            _laplaceSmoothingK = laplaceSmoothingK;
            foreach (var tuple in trainingData)
            {
                AddTrainingSample(tuple);
            }
        }

        public long GetCategorySamplesCount(TCategory category)
        {
            long value;
            return _categorySamplesCount.TryGetValue(category, out value) ? value : 0L;
        }

        public long GetCategoryAttributesCount(TCategory category)
        {
            long value;
            return _categoryAttributesCount.TryGetValue(category, out value) ? value : 0L;
        }

        public long GetCategoryAttributeCount(TCategory category, TAttribute attribute)
        {
            long value;
            return _categoryAttributeCount.TryGetValue(Tuple.Create(category, attribute), out value) ? value : 0L;
        }

        public BigRational GetCategoryProbability(TCategory category)
        {
            return ApplyLaplaceSmoothingK(GetCategorySamplesCount(category), _totalSamplesCount, _knownCategories.Count);
        }

        public BigRational GetAttributeProbabilityGivenCategory(TAttribute attribute, TCategory category)
        {
            return ApplyLaplaceSmoothingK(GetCategoryAttributeCount(category, attribute), GetCategoryAttributesCount(category), _knownAttributes.Count);
        }

        public BigRational GetCategoryProbabilityGivenAttributes(TCategory category, TAttribute[] attributes)
        {
            var nominator = GetCategoryNonNormalizedProbabilityGivenAttributes(category, attributes);

            var denominator = BigRational.Zero;
            foreach (var knownCategory in _knownCategories)
            {
                denominator += GetCategoryNonNormalizedProbabilityGivenAttributes(knownCategory, attributes);
            }

            return nominator / denominator;
        }

        private BigRational GetCategoryNonNormalizedProbabilityGivenAttributes(TCategory category, IEnumerable<TAttribute> attributes)
        {
            var result = GetCategoryProbability(category);

            foreach (var attribute in attributes)
            {
                result *= GetAttributeProbabilityGivenCategory(attribute, category);
            }

            return result;
        }

        private BigRational ApplyLaplaceSmoothingK(long count, long totalCount, long groupsCount)
        {
            return ApplyLaplaceSmoothing(_laplaceSmoothingK, count, totalCount, groupsCount);
        }

        private static BigRational ApplyLaplaceSmoothing(long k, long count, long totalCount, long groupsCount)
        {
            return (new BigRational((decimal)count) + new BigRational((decimal)k)) / (new BigRational((decimal)totalCount) + new BigRational((decimal)k) * new BigRational((decimal)groupsCount));
        }

        private void AddTrainingSample(Tuple<TCategory, TAttribute[]> tuple)
        {
            var category = tuple.Item1;
            var attributes = tuple.Item2;

            _totalSamplesCount = _totalSamplesCount + 1L;

            _knownCategories.Add(category);
            IncrementKeyValue(_categorySamplesCount, category, 1L);
            IncrementKeyValue(_categoryAttributesCount, category, attributes.Length);

            foreach (var attribute in attributes)
            {
                _knownAttributes.Add(attribute);
                IncrementKeyValue(_categoryAttributeCount, Tuple.Create(category, attribute), 1L);
            }
        }

        private static void IncrementKeyValue<TKey>(IDictionary<TKey, long> dict, TKey key, long increment)
        {
            long value;
            if (dict.TryGetValue(key, out value))
            {
                value = value + increment;
            }
            else
            {
                value = increment;
            }
            dict[key] = value;
        }
    }

}