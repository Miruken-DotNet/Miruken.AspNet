namespace Miruken.AspNet.Test.Site
{ 
    using Swagger;
    using Swashbuckle.Application;
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Web.Http;

    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            config.EnableSwagger(swagger =>
                {
                    swagger.RootUrl(GetRootUrl);
                    swagger.SingleApiVersion("v1", nameof(WebApiApplication));
                    swagger.UseMiruken(d =>d.RequireBearerToken());
                })
                .EnableSwaggerUi();
        }

        private static string GetRootUrl(HttpRequestMessage request)
        {
            return new UriBuilder(
                    request.RequestUri.Scheme,
                    request.RequestUri.Host,
                    int.Parse(request.RequestUri.Port.ToString(
                        CultureInfo.InvariantCulture)),
                    request.GetRequestContext().VirtualPathRoot).
                Uri.AbsoluteUri.TrimEnd('/');
        }
    }
}
