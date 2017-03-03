using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using NEdifis.Attributes;

namespace NSuggest.ElasticSearch
{
    [ExcludeFromConventions("Copy&Paste, cf: http://stackoverflow.com/a/24067483/2592915")]
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
                   (token.Type == JTokenType.Null);
        }
        public static void ThrowOnErrors(this JToken response)
        {
            var errors = response.SelectToken("errors", false) as JArray;
            if (errors == null) return;
            var exceptions = errors.Select(ToException).Where(e => e != null).ToList();
            if (!exceptions.Any()) return;
            throw new AggregateException(exceptions);
        }
        private static Exception ToException(this JToken error)
        {
            var message = error.SelectToken("message", false)?.ToString();
            return !string.IsNullOrWhiteSpace(message) ? new InvalidOperationException(message) : null;
        }
    }
}