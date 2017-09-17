namespace Miruken.AspNet.Castle.Tests
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Callback;
    using Context;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Http;
    using Map;
    using Mediate;
    using Mediate.Castle;
    using Mediate.Route;
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
                    new HandleFeature(), new ValidateFeature()).Use(
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
        public async Task Should_Route_Http_Requests()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player = new Player
                {
                    Name = "Philippe Coutinho"
                };
                var response = await _handler
                    .Send(new RegisterPlayer()
                        .RouteTo("http://localhost:9000/process"));
            }
        }
    }
}
