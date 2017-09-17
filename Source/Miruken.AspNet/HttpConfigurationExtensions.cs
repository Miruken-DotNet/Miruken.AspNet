namespace Miruken.AspNet
{
    using System.Web.Http;
    using Context;

    public static class HttpConfigurationExtensions
    {
        public static HttpConfiguration UseMiruken(
            this HttpConfiguration configuration, IContext context)
        {
            configuration.DependencyResolver = new ContextualResolver(context);
            return configuration;
        }
    }
}
