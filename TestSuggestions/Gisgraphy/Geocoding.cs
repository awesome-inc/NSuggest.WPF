using System.Runtime.Serialization;
using NSuggest.Query;

namespace TestSuggestions.Gisgraphy
{
    /// <summary>
    /// A client implementation for the Gisgraphy Geocoding service.
    /// </summary>
    public sealed class Geocoding : ResponseProvider<Geocoding.Response>
    {
        /// <summary>
        /// The default request template.
        /// </summary>
        public const string DefaultQueryTemplate = "http://services.gisgraphy.com/geocoding/geocoding?address={0}&country={1}&format=json";

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextSearch"/> class.
        /// </summary>
        /// <param name="urlTemplate">The request template.</param>
        public Geocoding(string urlTemplate = DefaultQueryTemplate) 
            : base(urlTemplate, new WebStreamBuilder(), new JsonResponseReader<Response>())
        {
        }

        #region Nested Classes (Data Contract, JSON)

        /// <summary>
        /// The geocoding response.
        /// </summary>
        [DataContract(Namespace="")]
        public sealed class Response : IResponse
        {
            /// <summary>
            /// The number of results found.
            /// </summary>
            [DataMember(Name = "numFound")]
            public long NumFound;

            /// <summary>
            /// The query time in milliseconds.
            /// </summary>
            [DataMember]
            public long QTime;

            /// <summary>
            /// The results.
            /// </summary>
            [DataMember(Name = "result")]
            public Result[] Results;

        }

        /// <summary>
        /// A geocoding result.
        /// </summary>
        [DataContract]
        public sealed class Result
        {
            #region Fields

            // see http://www.gisgraphy.com/documentation/user-guide.htm#geocodingservice

            [DataMember(Name = "city", IsRequired = false)]
            public string City;

            [DataMember(Name = "countryCode")]
            public string CountryCode;

            [DataMember(Name = "geocodingLevel")]
            public string GeocodingLevel;

            /// <summary>
            /// The data record identifier.
            /// </summary>
            [DataMember(Name = "id")]
            public long Id;

            /// <summary>
            /// The location longitude in decimal degrees (WGS84).
            /// </summary>
            [DataMember(Name = "lat")]
            public double Latitude;

            /// <summary>
            /// The location longitude in decimal degrees (WGS84).
            /// </summary>
            [DataMember(Name="lng")]
            public double Longitude;

            [DataMember(Name = "name")]
            public string Name;

            [DataMember(Name = "streetName", IsRequired = false)]
            public string StreetName;

            [DataMember(Name = "streetType", IsRequired = false)]
            public string StreetType;

            [DataMember(Name = "state", IsRequired = false)]
            public string State;

            /*
            [DataMember(Name = "houseNumber", IsRequired = false)]
            public string HouseNumber;

            [DataMember(Name = "houseNumberInfo", IsRequired = false)]
            public string HouseNumberInfo;


            [DataMember(Name = "dependentLocality", IsRequired = false)]
            public string DependentLocality;

            [DataMember(Name = "PostTown", IsRequired = false)]
            public string PostTown;

            [DataMember(Name = "district", IsRequired = false)]
            public string District;

            [DataMember(Name = "quarter", IsRequired = false)]
            public string Quarter;

            [DataMember(Name = "extraInfo", IsRequired = false)]
            public string ExtraInfo;

            [DataMember(Name = "POBox", IsRequired = false)]
            public string POBox;

            [DataMember(Name = "POBoxInfo", IsRequired = false)]
            public string POBoxInfo;

            [DataMember(Name = "POBoxAgency", IsRequired = false)]
            public string POBoxAgency;

            [DataMember(Name = "preDirection", IsRequired = false)]
            public string preDirection;

            [DataMember(Name = "postDirection", IsRequired = false)]
            public string postDirection;

            [DataMember(Name = "streetNameIntersection", IsRequired = false)]
            public string streetNameIntersection;

            [DataMember(Name = "streetTypeIntersection", IsRequired = false)]
            public string streetTypeIntersection;

            [DataMember(Name = "preDirectionIntersection", IsRequired = false)]
            public string preDirectionIntersection;

            [DataMember(Name = "postDirectionIntersection", IsRequired = false)]
            public string postDirectionIntersection;

            [DataMember(Name = "civicNumberSuffix", IsRequired = false)]
            public string civicNumberSuffix;

            [DataMember(Name = "floor", IsRequired = false)]
            public string floor;

            [DataMember(Name = "sector", IsRequired = false)]
            public string sector;

            [DataMember(Name = "quadrant", IsRequired = false)]
            public string quadrant;

            [DataMember(Name = "block", IsRequired = false)]
            public string block;

            [DataMember(Name = "country", IsRequired = false)]
            public string country;
            */

            [DataMember(Name = "zipCode", IsRequired = false)]
            public string ZipCode;

            #endregion
        }

        #endregion
    }
}
