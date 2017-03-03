using System.IO;
using System.Runtime.Serialization;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NSuggest.Query
{
    [TestFixtureFor(typeof(JsonResponseReader<>))]
    // ReSharper disable InconsistentNaming
    internal class JsonResponseReader_Should
    {
        [Test]
        public void Serialize_json()
        {
            var sut = new JsonResponseReader<DummyResponse>();
            using (var stream = new MemoryStream())
            {
                var expected = new DummyResponse {Name = "dummy"};
                JsonResponseReader<DummyResponse>.Serializer.WriteObject(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);
                var actual = sut.From(stream);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        [DataContract]
        private class DummyResponse : IResponse
        {
            [DataMember]
            public string Name;
        }
    }
}