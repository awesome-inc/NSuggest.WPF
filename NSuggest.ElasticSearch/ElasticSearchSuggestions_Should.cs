using System.Linq;
using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NUnit.Framework;

namespace TestSuggestions.ElasticSearch
{
    [TestFixtureFor(typeof(ElasticSearchSuggestions))]
    // ReSharper disable once InconsistentNaming
    internal class ElasticSearchSuggestions_Should
    {
        [Test]
        public void Read_JToken_From_Response()
        {
            var ctx = new ContextFor<ElasticSearchSuggestions>();
            ctx.Use(new[] { "author" });
            var sut = ctx.BuildSut();

            var items = sut.GetSuggestions(TestData.Hits, "Tho").ToArray();

            items.Should().Equal("Thomate", "Thomas", "Thomas Smith");
            items.Should().NotContain("Horst");
        }
    }
}