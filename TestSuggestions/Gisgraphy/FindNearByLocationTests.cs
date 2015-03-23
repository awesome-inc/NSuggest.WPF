using System.Diagnostics;
using NSuggest.Query;
using NUnit.Framework;

namespace TestSuggestions.Gisgraphy
{
    /// <summary>
    ///This is a test class for FindNearByLocationTest and is intended
    ///to contain all FindNearByLocationTest Unit Tests
    ///</summary>
    [TestFixture]
    class FindNearByLocationTests
    {
        readonly IProvideResponse<FindNearByLocation.Response> _service = new FindNearByLocation();

        [Test, Explicit, Category("Integration")]
        public void ShouldReturnNearByLocations()
        {
            Trace.TraceInformation("For: 7, 51");
            var response = _service.For(7, 51);
            Trace.TraceInformation("Response: {0} results, time: {1} ms", response.NumFound, response.QTime);
            if (response.NumFound > 0)
            {
                //Assert.AreEqual(response.results.Length, response.numFound);
                Assert.IsTrue(response.Results.Length > 0);

                Trace.TraceInformation("Results:");
                foreach (var r in response.Results)
                {
                    Trace.TraceInformation("Id:{0}, Name:\"{1}\", Distance:{2}, Lng:{3}, Lat:{4}, Alt={5}",
                        r.FeatureId, r.Name, r.Distance, r.Longitude, r.Latitude, r.Altitude);
                }
            }
        }
    }
}
