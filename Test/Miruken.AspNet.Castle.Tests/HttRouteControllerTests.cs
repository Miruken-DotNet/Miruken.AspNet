namespace Miruken.AspNet.Castle.Tests
{
    using System.Web.Http;
    using Callback;
    using Context;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Http;
    using Http.Get;
    using Map;
    using Mediate.Castle;
    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Owin;
    using Validate.Castle;

    [TestClass]
    public class HttRouteControllerTests
    {
        protected WindsorHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _handler = new WindsorHandler(container =>
            {
                container.Install(new FeaturesInstaller(
                    new MediateFeature().WithStandardMiddleware(),
                    new HandleFeature()).Use(
                        Classes.FromAssemblyContaining<HttpRouter>(),
                        Classes.FromAssemblyContaining<MappingHandler>(),
                        Classes.FromThisAssembly()));
            });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _handler.Dispose();
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Formatters.Clear();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);

            var container = new WindsorContainer()
                  .Install(new FeaturesInstaller(
                      new HandleFeature(), new ValidateFeature(),
                      new MediateFeature().WithStandardMiddleware(),
                      new AspNetFeature())
                  .Use(Classes.FromThisAssembly()));
            container.Kernel.AddHandlersFilter(new ContravariantFilter());

            var appContext = new Context();
            appContext.AddHandlers(new WindsorHandler(container));
            config.DependencyResolver = new ContextualResolver(appContext);
        }

        [TestMethod]
        public void Should_Route_Http_Requests()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {

            }
        }
    }
}
