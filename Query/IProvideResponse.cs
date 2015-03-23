using System;

namespace NSuggest.Query
{
    /// <summary>
    /// A basic searchTerm service interface.
    /// </summary>
    /// <typeparam name="TResponse">The respones type.</typeparam>
    public interface IProvideResponse<TResponse> where TResponse : IResponse
    {
        /// <summary>
        /// Gets or sets the template used to create the complete searchTerm request url.
        /// </summary>
        /// <value>The searchTerm template.</value>
        string UrlTemplate { get; set; }

        /// <summary>
        /// Gets or sets the request builder.
        /// </summary>
        /// <value>The request builder.</value>
        ICreateResponseStream StreamBuilder { get; set; }

        /// <summary>
        /// Gets or sets the response reader.
        /// </summary>
        /// <value>The response reader.</value>
        IReadResponse<TResponse> ResponseReader { get; set; }

        /// <summary>
        /// Returns the response for the specified request.
        /// </summary>
        /// <param name="request">The request url.</param>
        /// <returns></returns>
        TResponse For(Uri request);

        /// <summary>
        /// Performs a request, formatting the searchTerm from the specified arguments.
        /// </summary>
        /// <param name="args">The searchTerm arguments.</param>
        /// <returns></returns>
        TResponse For(params object[] args);
    }
}
