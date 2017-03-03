using System.Net.Http;

namespace NSuggest.ElasticSearch
{
    public interface IHookHttpRequest
    {
        void Hook(HttpRequestMessage request);
    }
}