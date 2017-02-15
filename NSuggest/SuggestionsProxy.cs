﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NSuggest
{
    public sealed class SuggestionsProxy : IProvideSuggestions
    {
        private readonly IBlackList<string> _blackList = new BlackList<string>();
        private readonly ICache<string, UInt32> _cache = new Cache<string, uint>();
        private readonly ISuffixTree _suffixTree = new TernarySearchTree();
        private readonly IProvideSuggestions _suggestionProvider;

        public SuggestionsProxy(IProvideSuggestions suggestionProvider)
        {
            if (suggestionProvider == null)
                throw new ArgumentNullException(nameof(suggestionProvider));
            _suggestionProvider = suggestionProvider;

            MaxFailures = 5;
            MinTermLength = 3;

            _cache.NodeRemoved += CacheNodeRemoved;
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
                throw new ArgumentNullException(nameof(prefix));

            if (prefix.Length < MinTermLength)
                return null;

            if (IsBlackListed(prefix))
                return null;

            if (_suffixTree.Any(prefix))
                return _suffixTree.KeysMatching(prefix);

            var result = _suggestionProvider.For(prefix).ToList();
            if (result.Count > 0)
            {
                PushCache(result);
                return result;
            }

            PushBlackList(prefix);
            return null;
        }

        #endregion

        private void CacheNodeRemoved(object sender, Tuple<string, uint> e)
        {
            _suffixTree.TryRemove(e.Item1);
        }

        private bool IsBlackListed(string textPattern)
        {
            return _blackList.IsBlackListed(textPattern, MaxFailures);
        }

        private void PushBlackList(string textPattern)
        {
            _blackList.Push(textPattern);
        }

        private void PushCache(IList<string> keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            _suffixTree.AddRange(keys);
            foreach (var key in keys)
                _cache.Add(key, 1);
        }
    }
}