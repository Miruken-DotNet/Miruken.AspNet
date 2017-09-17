﻿namespace Miruken.AspNet.TestWeb
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Castle;
    using Context;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Mediate.Castle;
    using Miruken.Castle;
    using Validate.Castle;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container  = new WindsorContainer()
                .Install(new FeaturesInstaller(
                    new HandleFeature(), new ValidateFeature(),
                    new MediateFeature().WithStandardMiddleware(),
                    new AspNetFeature())
                .Use(Classes.FromThisAssembly()));
            container.Kernel.AddHandlersFilter(new ContravariantFilter());

            var appContext = new Context();
            appContext.AddHandlers(new WindsorHandler(container));

            GlobalConfiguration.Configuration.DependencyResolver =
                new ContextualResolver(appContext);
        }
    }
}