using System.Diagnostics;
using NSuggest.Query;
using NUnit.Framework;

namespace TestSuggestions.Gisgraphy
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    internal class GeocodingTest
    {
        private readonly IProvideResponse<Geocoding.Response> _service = new Geocoding();

        [Test, Explicit, Category("Integration")]
        public void ShouldReturnBasicResults()
        {
            Trace.TraceInformation("For: cologne/DE");
            var response = _service.For("cologne", "DE");
            Trace.TraceInformation("Response: {0} results, time: {1} ms", response.NumFound, response.QTime);
            if (response.NumFound > 0)
            {
                //Assert.AreEqual(response.result.Length, response.numFound);
                Assert.IsTrue(response.Results.Length > 0);

                Trace.TraceInformation("Results:");
                foreach (var r in response.Results)
                {
                    Trace.TraceInformation("Id:{0}, Lng:{1}, Lat:{2}, level={3}, name=\"{4}\"",
                        r.Id, r.Longitude, r.Latitude, r.GeocodingLevel, r.Name);
                }
            }
        }
    }
}
