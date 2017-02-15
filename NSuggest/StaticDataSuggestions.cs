using System;
using System.Collections.Generic;
using System.Linq;

namespace NSuggest
{
    public sealed class StaticDataSuggestions : IProvideSuggestions
    {
        private readonly IEnumerable<string> _source;

        public StaticDataSuggestions(IEnumerable<string> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _source = source.ToArray();
        }

        public IEnumerable<string> For(string prefix)
        {
            return _source.Where(item => item.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}