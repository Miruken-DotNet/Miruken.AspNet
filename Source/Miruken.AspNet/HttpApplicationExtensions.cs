namespace Miruken.AspNet
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Callback;
    using Context;

    public static class HttpApplicationExtensions
    {
        private const string RootContextKey    = "Miruken.Root.Context";
        private const string RequestContextKey = "Miruken.Request.Context";

        public static HttpApplication UseMiruken(
            this HttpApplication application, IContext context)
        {
            ControllerBuilder.Current.SetControllerFactory(new ContextualControllerFactory());
            SetRootMirukenContext(application, context);
            return application;
        }

        public static IContext GetRootMirukenContext(
            this HttpApplication application)
        {
            var appState    = application.Application;
            var rootContext = appState[RootContextKey] as IContext;
            if (rootContext == null)
                appState[RootContextKey] = rootContext = new Context();
            return rootContext;
        }

        public static void SetRootMirukenContext(
            this HttpApplication application, IContext context)
        {
            application.Application[RootContextKey] = context;
        }

        public static IContext GetMirukenContext(this RequestContext request)
        {
            var items   = request.HttpContext.Items;
            var context = items[RequestContextKey] as IContext;
            if (context == null)
            {
                var app = request.HttpContext.ApplicationInstance;
                var rootContext = GetRootMirukenContext(app);
                items[RequestContextKey] = context
                    = rootContext.BestEffort()?.Proxy<IHttpContextSelector>()
                        .SelectMvcContext(app)
                    ?? rootContext?.CreateChild() 
                    ?? new Context();
            }
            return context;
        }

        public static void SetMirukenContext(
            this RequestContext request, IContext context)
        {
            request.HttpContext.Items[RequestContextKey] = context;
        }
    }
}