using System.Runtime.Serialization;
using NSuggest.Query;

namespace TestSuggestions.Gisgraphy
{
    /// <summary>
    /// A client implementation for the Gisgraphy FullTextSearch service.
    /// </summary>
    public sealed class FullTextSearch : ResponseProvider<FullTextSearch.Response>
    {
        /// <summary>
        /// The default request template.
        /// </summary>
        public const string DefaultQueryTemplate = "http://services.gisgraphy.com/fulltext/fulltextsearch?q={0}&format=json&lang=en&spellchecking=false&style=medium";

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextSearch"/> class.
        /// </summary>
        /// <param name="urlTemplate">The query template.</param>
        public FullTextSearch(string urlTemplate = DefaultQueryTemplate) 
            : base(urlTemplate, new WebStreamBuilder(), new JsonResponseReader<Response>())
        {
        }

        #region Nested Classes (Data Contract, JSON)

        /// <summary>
        /// The response for a full text search request.
        /// </summary>
        [DataContract(Namespace="")]
        public sealed class Response : IResponse
        {
            /// <summary>
            /// The response header.
            /// </summary>
            [DataMember(Name = "responseHeader")]
            public Header Header;

            /// <summary>
            /// The result.
            /// </summary>
            [DataMember(Name = "response")]
            public Result Result;

            /*
            /// <summary>
            /// The spell check result.
            /// </summary>
            [DataMember(Name = "spellcheck")]
            public SpellCheck spellCheck;
            */
        }

        /// <summary>
        /// The response header.
        /// </summary>
        [DataContract]
        public sealed class Header
        {
            #region Fields

            /// <summary>
            /// The number of results found.
            /// </summary>
            [DataMember(Name = "status")]
            public int Status;

            /// <summary>
            /// The query time in milliseconds.
            /// </summary>
            [DataMember]
            public int QTime;

            #endregion
        }

        /// <summary>
        /// The result.
        /// </summary>
        [DataContract]
        public sealed class Result
        {
            #region Fields

            /// <summary>
            /// The number of results found.
            /// </summary>
            [DataMember(Name = "numFound")]
            public int NumFound;

            /// <summary>
            /// The start pagination.
            /// </summary>
            [DataMember(Name = "start")]
            public int Start;

            /// <summary>
            /// The maximum score.
            /// </summary>
            [DataMember(Name = "maxScore")]
            public double MaxScore;

            [DataMember(Name = "docs")]
            public Document[] Docs;

            #endregion
        }

        /// <summary>
        /// A result document.
        /// </summary>
        [DataContract]
        public sealed class Document
        {
            #region Fields

            // see http://www.gisgraphy.com/documentation/user-guide.htm#fulltextwebservice

            /// <summary>
            /// The data record identifier.
            /// </summary>
            [DataMember(Name="feature_id")]
            public long Id;

            [DataMember(Name="name")]
            public string Name;

            [DataMember(Name = "name_ascii")]
            public string NameAscii;

            /// <summary>
            /// The location longitude in decimal degrees (WGS84).
            /// </summary>
            [DataMember(Name = "lng")]
            public double Longitude;

            /// <summary>
            /// The location longitude in decimal degrees (WGS84).
            /// </summary>
            [DataMember(Name = "lat")]
            public double Latitude;

            [DataMember(Name="placetype")]
            public string PlaceType;

            [DataMember(Name="country_name")]
            public string CountryName;

            [DataMember(Name="country_code")]
            public string CountryCode;

            [DataMember(Name = "country_flag_url")]
            public string CountryFlagUrl;

            [DataMember(Name = "timezone")]
            public string TimeZone;

            [DataMember(Name = "feature_class")]
            public string FeatureClass;

            [DataMember(Name = "feature_code")]
            public string FeatureCode;

            [DataMember(Name = "fully_qualified_name")]
            public string FullyQualifiedName;

            [DataMember(Name = "gtopo30")]
            public double Altitude;

            [DataMember(Name = "score")]
            public double Score;

            [DataMember(Name = "population")]
            public long Population; // default:0

            [DataMember(Name = "zipcode")]
            public string[] ZipCode;

            /*
            [DataMember(Name = "google_map_url")]
            public string GoogleMapUrl;

            [DataMember(Name = "yahoo_map_url")]
            public string YahooMapUrl;
            */

            #endregion
        }

        /*
        /// <summary>
        /// A spellcheck result.
        /// </summary>
        [DataContract]
        public sealed class SpellCheck
        {
            [DataMember]
            public Suggestion[] suggestions;
        }

        /// <summary>
        /// The suggestions from a spellcheck result.
        /// </summary>
        [DataContract]
        public sealed class Suggestion
        {
            [DataMember]
            public string[] suggestion;
        }
         */

        #endregion
    }
}
