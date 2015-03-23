
using System;
using System.IO;
using System.Net;

namespace NSuggest.Query
{
    public sealed class WebStreamBuilder : ICreateResponseStream
    {
        public Stream For(Uri requestUrl)
        {
            var webRequest = WebRequest.Create(requestUrl);

            // This minimizes WebExceptions, see
            // http://social.msdn.microsoft.com/Forums/en-US/netfxnetcom/thread/faf9737d-5cb3-442b-bf9d-26341a204475/
            var httpRequest = webRequest as HttpWebRequest;
            if (httpRequest != null)
                httpRequest.KeepAlive = false;

            var response = webRequest.GetResponse();
            return response.GetResponseStream();
        }
    }
}
