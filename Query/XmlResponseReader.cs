
using System.IO;
using System.Xml.Serialization;

namespace NSuggest.Query
{
    public sealed class XmlResponseReader<TResponse>
        : IReadResponse<TResponse>
        where TResponse : IResponse
    {
// ReSharper disable StaticFieldInGenericType
        static readonly XmlSerializer Serializer = new XmlSerializer(typeof(TResponse));
// ReSharper restore StaticFieldInGenericType

        public TResponse From(Stream stream)
        {
            return (TResponse)Serializer.Deserialize(stream);
        }
    }
}
