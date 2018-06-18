namespace Miruken.AspNet
{
    using System.Web;
    using Callback;
    using Context;

    public static class HttpApplicationExtensions
    {
        private const string RootContextKey    = "Miruken.Root.Context";
        private const string RequestContextKey = "Miruken.Request.Context";

        public static HttpApplication UseMiruken(
            this HttpApplication application, IContext context)
        {
            application.Application[RootContextKey] = context;
            return application;
        }

        public static IContext UseMiruken(
            this HttpContextBase request, IContext parent = null)
        {
            var items = request.Items;
            if (parent == null)
                parent = request.ApplicationInstance.GetRootMirukenContext();
            if (!(items[RequestContextKey] is IContext context))
            {
                items[RequestContextKey] = context
                    = parent.BestEffort()?.Proxy<ILogicalContextSelector>()
                          .SelectMvcContext(request)
                      ?? parent?.CreateChild()
                      ?? new Context();
            }
            return context;
        }

        public static IContext GetRootMirukenContext(
            this HttpApplication application)
        {
            var appState = application.Application;
            if (!(appState[RootContextKey] is IContext rootContext))
                appState[RootContextKey] = rootContext = new Context();
            return rootContext;
        }

        public static IContext GetMirukenContext(this HttpContextBase request)
        {
            var items = request.Items;
            return items[RequestContextKey] as IContext;
        }
    }
}