namespace Miruken.AspNet
{
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using Context;

    public static class HttpConfigurationExtensions
    {
        public static HttpConfiguration UseMiruken(
            this HttpConfiguration configuration, IContext context)
        {
            var services  = configuration.Services;
            var activator = (IHttpControllerActivator)
                services.GetService(typeof(IHttpControllerActivator));
            var resolver  = new ContextualResolver(context, activator);
            services.Replace(typeof(IHttpControllerActivator), resolver);
            configuration.DependencyResolver = resolver;
            return configuration;
        }
    }
}
