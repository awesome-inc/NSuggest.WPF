using System;
using System.Collections.Generic;

namespace NSuggest
{
    public interface IHaveKeyedValues<TValue> : IHaveKeyStrings
    {
        /// <summary>
        /// Gets all items with keys matching the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns />
        IEnumerable<Tuple<string, TValue>> ItemsMatching(string prefix);
    }
}