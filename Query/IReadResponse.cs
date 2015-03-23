using System.IO;

namespace NSuggest.Query
{
    /// <summary>
    /// A basic interface thats supports reading a strong typed object from a generic stream.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IReadResponse<out TResponse> where TResponse : IResponse
    {
        TResponse From(Stream stream);
    }
}
