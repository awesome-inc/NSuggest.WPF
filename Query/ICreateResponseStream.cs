using System;
using System.IO;

namespace NSuggest.Query
{
    /// <summary>
    /// A basic interface that supports creating a response stream for a request url.
    /// </summary>
    public interface ICreateResponseStream
    {
        Stream For(Uri requestUrl);
    }
}
