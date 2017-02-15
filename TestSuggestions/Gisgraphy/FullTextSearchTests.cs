using System.Diagnostics;
using NSuggest.Query;
using NUnit.Framework;

namespace TestSuggestions.Gisgraphy
{
    [TestFixture]
    internal class FullTextSearchTests
    {
        private readonly IProvideResponse<FullTextSearch.Response> _service = new FullTextSearch();

        [Test, Explicit, Category("Integration")]
        public void ShouldReturnBasicResults()
        {
            Trace.TraceInformation("For: cologne");
            var response = _service.For("cologne");

            Trace.TraceInformation("Response:");
            Trace.TraceInformation("\tHeader: status={0}, time={1} ms", 
                response.Header.Status, response.Header.QTime);
            Trace.TraceInformation("\tResult: {0} results, start={1}, maxScore={2}", 
                response.Result.NumFound, response.Result.Start, response.Result.MaxScore);

            if (response.Result.NumFound > 0)
            {
                Assert.IsTrue(response.Result.Docs.Length > 0);

                Trace.TraceInformation("\tDocs:");
                foreach (var r in response.Result.Docs)
                {
                    Trace.TraceInformation("Id:{0}, Lng:{1}, Lat:{2}, name=\"{3}\", placetype={4}",
                        r.Id, r.Longitude, r.Latitude, r.Name, r.PlaceType);
                }
            }
        }
    }
}
