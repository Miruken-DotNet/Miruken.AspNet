namespace Miruken.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Context;
    using Http;
    using Http.Format;

    public class HttpRouteContentNegotiator : IContentNegotiator
    {
        public ContentNegotiationResult Negotiate(
            Type type, HttpRequestMessage request, 
            IEnumerable<MediaTypeFormatter> formatters)
        {
            var scope     = request.GetDependencyScope();
            var context   = (Context)scope.GetService(typeof(Context));
            var formatter = context != null
                          ? new HttpRouteMediaTypeFormatter(context, HttpFormatters.Route)
                          : HttpFormatters.Route;
            return new ContentNegotiationResult(formatter, ContentType);
        }

        private static readonly MediaTypeHeaderValue ContentType =
            new MediaTypeHeaderValue("application/json");
    }
}
