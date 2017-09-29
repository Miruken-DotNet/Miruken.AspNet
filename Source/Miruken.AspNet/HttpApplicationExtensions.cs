namespace Miruken.AspNet
{
    using System.Web;
    using Context;

    public static class HttpApplicationExtensions
    {
        private const string RootContextKey = "Miruken.Root.Context";

        public static IContext GetRootContext(this HttpApplication application)
        {
            var appState    = application.Application;
            var rootContext = appState[RootContextKey] as IContext;
            if (rootContext == null)
                appState[RootContextKey] = rootContext = new Context();
            return rootContext;
        }

        public static void SetRootContext(this HttpApplication application, IContext context)
        {
            application.Application[RootContextKey] = context;
        }
    }
}
