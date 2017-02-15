using System;

namespace NSuggest.Query
{
    public abstract class ResponseProvider<TResponse> : IProvideResponse<TResponse>
        where TResponse : IResponse
    {
        public string UrlTemplate { get; set; }
        public ICreateResponseStream StreamBuilder { get; set; }
        public IReadResponse<TResponse> ResponseReader { get; set; }

        protected ResponseProvider(string urlTemplate,
            ICreateResponseStream createResponseStream, IReadResponse<TResponse> readResponse)
        {
            if (String.IsNullOrEmpty(urlTemplate))
                throw new ArgumentNullException(nameof(urlTemplate));
            if (createResponseStream == null)
                throw new ArgumentNullException(nameof(createResponseStream));
            if (readResponse == null)
                throw new ArgumentNullException(nameof(readResponse));

            UrlTemplate = urlTemplate;
            StreamBuilder = createResponseStream;
            ResponseReader = readResponse;
        }

        public TResponse For(Uri requestUrl)
        {
            using (var stream = StreamBuilder.For(requestUrl))
            {
                return ResponseReader.From(stream);
            }
        }

        public TResponse For(params object[] args)
        {
            var requestUrl = new Uri(string.Format(UrlTemplate, args), UriKind.RelativeOrAbsolute);
            return For(requestUrl);
        }
    }
}
