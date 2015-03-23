using System;
using System.Collections.Generic;
using System.Linq;
using NSuggest;
using TestSuggestions.Gisgraphy;

namespace TestSuggestions
{
    public class GisgraphySuggestions : IProvideSuggestions, IHaveKeyedValues<FullTextSearch.Document>
    {
        private readonly FullTextSearch _fullTextSearch = new FullTextSearch();

        public IEnumerable<string> For(string prefix)
        {
            return KeysMatching(prefix);
        }

        #region Implementation of IHaveKeyStrings

        public IEnumerable<Tuple<string, FullTextSearch.Document>> ItemsMatching(string prefix)
        {
            var response = _fullTextSearch.For(prefix);
            return response.Result.Docs.Select(d => new Tuple<string, FullTextSearch.Document>(d.Name, d));
        }

        public IEnumerable<string> KeysMatching(string prefix)
        {
            return ItemsMatching(prefix).Select(d => d.Item1);
        }

        #endregion
    }
}