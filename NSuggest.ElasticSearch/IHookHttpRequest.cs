using System.Net.Http;

namespace TestSuggestions.ElasticSearch
{
    public interface IHookHttpRequest
    {
        void Hook(HttpRequestMessage request);
    }
}