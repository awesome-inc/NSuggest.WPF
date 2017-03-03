using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSuggest.ElasticSearch
{
    public interface IHttpClient : IDisposable
    {
        Uri BaseAddress { get; set; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);
    }
}