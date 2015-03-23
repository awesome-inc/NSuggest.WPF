namespace SSE.Services.Query
{
    using System.Net;

    public sealed class WebRequestBuilder : IRequestBuilder
    {
        public System.IO.Stream GetStream(string query)
        {
            var request = (HttpWebRequest)WebRequest.Create(query);
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }
    }
}
