using System.Collections.Generic;

namespace NSuggest
{
    public interface IProvideSuggestions
    {
        IEnumerable<string> For(string prefix);
    }
}