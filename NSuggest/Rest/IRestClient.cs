using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NSuggest.Rest
{
    public interface IRestClient
    {
        Task<JToken> Request(HttpMethod method = null, string uri = null, JToken body = null);
        Uri BaseAddress { get; set; }
    }
}