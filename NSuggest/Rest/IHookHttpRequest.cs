using System.Net.Http;

namespace NSuggest.Rest
{
    public interface IHookHttpRequest
    {
        void Hook(HttpRequestMessage request);
    }
}