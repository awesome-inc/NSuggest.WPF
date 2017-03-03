using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NSuggest.ElasticSearch
{
    [TestFixtureFor(typeof(HttpClientWrapper))]
    // ReSharper disable once InconsistentNaming
    internal class HttpClientWrapper_Should
    {
        [Test]
        public void Allow_changing_baseaddress()
        {
            var uri = new Uri("http://server:1234/path/");
            var sut = new HttpClientWrapper { BaseAddress = uri };
            sut.BaseAddress.Should().Be(uri);
            Func<Task> act = async () => await sut.SendAsync(new HttpRequestMessage(HttpMethod.Head, string.Empty));
            act.ShouldThrow<AggregateException>();
            uri = new Uri("http://server2:4321/otherPath/");
            sut.BaseAddress = uri;
            sut.BaseAddress.Should().Be(uri);
            sut.Invoking(x => x.BaseAddress = new Uri("file:///C:/windows-version.txt")).ShouldThrow<ArgumentException>("only http & https allowed");
        }
    }
}