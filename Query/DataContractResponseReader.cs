
using System.IO;
using System.Runtime.Serialization;

namespace NSuggest.Query
{
    public sealed class DataContractResponseReader<TResponse>
        : IReadResponse<TResponse>
        where TResponse : IResponse
    {
// ReSharper disable StaticFieldInGenericType
        private static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(TResponse));
// ReSharper restore StaticFieldInGenericType

        public TResponse From(Stream stream)
        {
            return (TResponse)Serializer.ReadObject(stream);
        }
    }
}
