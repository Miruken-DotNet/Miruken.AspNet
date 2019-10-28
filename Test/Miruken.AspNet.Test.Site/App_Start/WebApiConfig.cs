﻿namespace Miruken.AspNet.Test.Site
{
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            SwaggerConfig.Register(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name:          "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults:      new { id = RouteParameter.Optional }
            );
        }
    }
}