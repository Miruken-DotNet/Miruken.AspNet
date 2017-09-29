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
            var activator = (IHttpControllerActivator)
                configuration.Services .GetService(typeof(IHttpControllerActivator));
            configuration.DependencyResolver = new ContextualResolver(context, activator);
            return configuration;
        }
    }
}
