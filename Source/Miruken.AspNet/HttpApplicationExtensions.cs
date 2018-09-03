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
            this HttpApplication application, Context context)
        {
            application.Application[RootContextKey] = context;
            return application;
        }

        public static Context UseMiruken(
            this HttpContextBase request, Context parent = null)
        {
            var items = request.Items;
            if (parent == null)
                parent = request.ApplicationInstance.GetRootMirukenContext();
            if (!(items[RequestContextKey] is Context context))
            {
                items[RequestContextKey] = context
                    = parent.BestEffort()?.Proxy<ILogicalContextSelector>()
                          .SelectMvcContext(request)
                      ?? parent?.CreateChild()
                      ?? new Context();
            }
            return context;
        }

        public static Context GetMirukenContext(this HttpContextBase request)
        {
            var items = request.Items;
            if (items[RequestContextKey] is Context context)
                return context;
            var rootContext = request.ApplicationInstance.GetRootMirukenContext();
            return request.UseMiruken(rootContext);
        }

        private static Context GetRootMirukenContext(
            this HttpApplication application)
        {
            var appState = application.Application;
            if (!(appState[RootContextKey] is Context rootContext))
                appState[RootContextKey] = rootContext = new Context();
            return rootContext;
        }
    }
}