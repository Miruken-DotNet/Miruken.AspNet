namespace Miruken.AspNet
{
    using System;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;
    using Http;

    public class HttpRouteAttribute
        : Attribute, IControllerConfiguration
    {
        public void Initialize(
            HttpControllerSettings controllerSettings, 
            HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IContentNegotiator), 
                new JsonContentNegotiator(HttpFormatters.JsonTyped));
        }
    }
}
