using System.IO;
using System.Runtime.Serialization.Json;

namespace NSuggest.Query
{
    public sealed class JsonResponseReader<TResponse> 
        : IReadResponse<TResponse> 
        where TResponse : IResponse
    {
// ReSharper disable StaticFieldInGenericType
        static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TResponse));
// ReSharper restore StaticFieldInGenericType

        public TResponse From(Stream stream)
        {
            return (TResponse)Serializer.ReadObject(stream);
        }
    }
}
