using System.IO;
using System.Runtime.Serialization.Json;

namespace NSuggest.Query
{
    public sealed class JsonResponseReader<TResponse> 
        : IReadResponse<TResponse> 
        where TResponse : IResponse
    {
        internal static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TResponse));

        public TResponse From(Stream stream)
        {
            return (TResponse)Serializer.ReadObject(stream);
        }
    }
}
