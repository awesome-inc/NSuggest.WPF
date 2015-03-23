using System;
using System.Collections.Generic;
using System.Linq;

namespace NSuggest
{
    public class KeyedValuesProxy<TValue> : IProvideSuggestions, IHaveKeyedValues<TValue>
    {
        private readonly TernarySearchTree<TValue> _tree = new TernarySearchTree<TValue>();
        private readonly IBlackList<string> _blackList = new BlackList<string>();
        private readonly ICache<string, TValue> _cache = new Cache<string, TValue>();
        private readonly IHaveKeyedValues<TValue> _keyedValuesProvider;

        public KeyedValuesProxy(IHaveKeyedValues<TValue> keyedValuesProvider)
        {
            if (keyedValuesProvider == null)
                throw new ArgumentNullException("keyedValuesProvider");
            _keyedValuesProvider = keyedValuesProvider;

            MaxFailures = 5;
            MinTermLength = 3;

            _cache.NodeRemoved += (s, e) => _tree.TryRemove(e.Item1);
        }

        public int CacheSize
        {
            get { return _cache.Capacity; }
            set { _cache.Capacity = value; }
        }

        public int BlackListSize
        {
            get { return _blackList.Capacity; }
            set { _blackList.Capacity = value; }
        }

        public int MaxFailures { get; set; }

        public int MinTermLength { get; set; }

        #region IProvideSuggestions Members

        public IEnumerable<string> For(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");

            if (prefix.Length < MinTermLength)
                return null;

            if (IsBlackListed(prefix))
                return null;

            if (_tree.Any(prefix))
                return _tree.KeysMatching(prefix);

            var result = _keyedValuesProvider.ItemsMatching(prefix).ToList();
            if (result.Count > 0)
            {
                PushCache(result);
                return result.Select(value => value.Item1);
            }

            PushBlackList(prefix);
            return null;
        }

        #endregion

        #region Private

        private bool IsBlackListed(string textPattern)
        {
            return _blackList.IsBlackListed(textPattern, MaxFailures);
        }

        private void PushBlackList(string textPattern)
        {
            _blackList.Push(textPattern);
        }

        private void PushCache(IList<Tuple<string,TValue>> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            _tree.AddRange(values);
            foreach (var value in values)
                _cache.Add(value.Item1, value.Item2);
        }

        #endregion

        #region Implementation of IHaveKeyStrings

        public IEnumerable<Tuple<string, TValue>> ItemsMatching(string prefix)
        {
            return _tree.ItemsMatching(prefix);
        }

        public bool Any(string prefix)
        {
            return _tree.Any(prefix);
        }

        public IEnumerable<string> KeysMatching(string prefix)
        {
            return _tree.KeysMatching(prefix);
        }

        #endregion
    }
}