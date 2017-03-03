using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NSuggest.ElasticSearch
{
    public class ElasticSearchSuggestions : IProvideSuggestions
    {
        private readonly IRestClient _client;
        private readonly JProperty _fieldsProperty;

        public ElasticSearchSuggestions(IRestClient client, string[] fields)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            _client = client;

            // ReSharper disable once CoVariantArrayConversion
            _fieldsProperty = new JProperty("fields", new JArray(fields));
        }

        public IEnumerable<string> For(string prefix)
        {
            var requestBody = CreateRequest(prefix);
            var response = _client.Post("_search", requestBody).Result;
            return GetSuggestions(response, prefix);
        }

        private JToken CreateRequest(string prefix)
        {
            return new JObject(
                _fieldsProperty,
                new JProperty("query", new JObject(
                        new JProperty("multi_match", new JObject(
                            _fieldsProperty,
                            new JProperty("type", "phrase_prefix"),
                            new JProperty("query", prefix))))
                ));
        }

        internal IEnumerable<string> GetSuggestions(JToken response, string prefix)
        {
            var fields = response.SelectTokens("hits.hits[*].fields");
            var valueArrays = fields.SelectMany(x => x.Values()).OfType<JArray>();
            var values = valueArrays.SelectMany(x => x.ToObject<List<string>>());

            return values.Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).Distinct();
        }
    }
}