
using System.Xml.Serialization;
using NSuggest.Query;

namespace TestSuggestions.Gisgraphy
{
    /// <summary>
    /// A client implementation for the Gisgraphy Geolocalisation / FindNearByLocation service.
    /// </summary>
    public sealed class FindNearByLocation : ResponseProvider<FindNearByLocation.Response>
    {
        // http://localhost:8080/geoloc/findnearbylocation?lat=4.5&lng=5.7

        /// <summary>
        /// The default request template.
        /// </summary>
        public const string DefaultQueryTemplate = "http://services.gisgraphy.com/geoloc/findnearbylocation?lat={1}&lng={0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextSearch"/> class.
        /// </summary>
        /// <param name="urlTemplate">The query template.</param>
        public FindNearByLocation(string urlTemplate = DefaultQueryTemplate) 
            : base(urlTemplate, new WebStreamBuilder(), new XmlResponseReader<Response>())
        {
        }

        #region Nested Classes (Data Contract, JSON)

        /// <summary>
        /// The response for a reverse geocoding search request.
        /// </summary>
        //[DataContract(Name="results", Namespace="http://gisgraphy.com")]
        //[CollectionDataContract(ItemName="result")]
        [XmlType(AnonymousType=true, Namespace="http://gisgraphy.com")]
        [XmlRoot("results", Namespace="http://gisgraphy.com", IsNullable=false)]
        public sealed class Response : /*Collection<Results>,*/ IResponse
        {
            /// <summary>
            /// The number of results found.
            /// </summary>
            [XmlElement("numFound")]
            public int NumFound;

            /// <summary>
            /// The query time in milliseconds.
            /// </summary>
            public int QTime;

            [XmlElement("result")]
            public Result[] Results;
        }

        [XmlType(AnonymousType = true, Namespace = "http://gisgraphy.com")]
        public sealed class Result
        {
            /// <summary>
            /// The distance in metres.
            /// </summary>
            [XmlElement("distance")]
            public double Distance;

            /// <summary>
            /// The location longitude in decimal degrees (WGS84).
            /// </summary>
            [XmlElement("lng")]
            public double Longitude;

            /// <summary>
            /// The location longitude in decimal degrees (WGS84).
            /// </summary>
            [XmlElement("lat")]
            public double Latitude;

            [XmlElement("gtopo30")]
            public double Altitude;

            /// <summary>
            /// The location name.
            /// </summary>
            [XmlElement("name")]
            public string Name;

            [XmlElement("admlCode")]
            public string AdmlCode;

            [XmlElement("admlName")]
            public string AdmlName;

            [XmlElement("asciiName")]
            public string AsciiName;

            [XmlElement("countryCoude")]
            public string CountryCoude;

            [XmlElement("featureClass")]
            public string FeatureClass;

            [XmlElement("featureCode")]
            public string FeatureCode;

            [XmlElement("featureId")]
            public long FeatureId;

            [XmlElement("population")]
            public long Population; // default:0

            [XmlElement("timeZone")]
            public string TimeZone;

            [XmlElement("placeType")]
            public string PlaceType;

            [XmlElement("oneWay")]
            public bool OneWay;

            /*
            [DataMember(Name = "google_map_url")]
            public string GoogleMapUrl;

            [DataMember(Name = "yahoo_map_url")]
            public string YahooMapUrl;
            */

            //[DataMember(Name = "country_flag_url")]
            [XmlElement("country_flag_url")]
            public string CountryFlagUrl;
        }

        #endregion
    }
}
