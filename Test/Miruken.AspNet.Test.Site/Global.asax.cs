namespace Miruken.AspNet.Test.Site
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Castle;
    using Castle.Tests;
    using Context;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Miruken.Castle;
    using Validate.Castle;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var appContext = new Context();
            this.UseMiruken(appContext);
            GlobalConfiguration.Configuration.UseMiruken(appContext);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new WindsorContainer()
                .Install(new FeaturesInstaller(
                        new HandleFeature(),
                        new ValidateFeature(),
                        new AspNetFeature())
                    .Use(Classes.FromAssemblyContaining<PlayerHandler>(),
                         Classes.FromThisAssembly()));
            container.Kernel.AddHandlersFilter(new ContravariantFilter());
            appContext.AddHandlers(new WindsorHandler(container));
        }
    }
}
