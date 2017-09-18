namespace Miruken.AspNet.Castle.Tests
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
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
    using Validate;
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
            var appContext = new Context();
            var config     = new HttpConfiguration()
                .UseMiruken(appContext);
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);

            var container = new WindsorContainer()
                  .Install(new FeaturesInstaller(
                      new HandleFeature(), new ValidateFeature(),
                      new MediateFeature().WithStandardMiddleware(),
                      new AspNetFeature())
                  .Use(Classes.FromThisAssembly()));
            container.Kernel.AddHandlersFilter(new ContravariantFilter());
            appContext.AddHandlers(new WindsorHandler(container));
        }

        [TestMethod]
        public async Task Should_Route_Requests()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player = new Player
                {
                    Name = "Philippe Coutinho"
                };
                var response = await _handler
                    .Send(new CreatePlayer { Player = player }
                    .RouteTo("http://localhost:9000/process"));
                Assert.AreEqual("Philippe Coutinho", response.Player.Name);
                Assert.IsTrue(response.Player.Id > 0);
            }
        }

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Fail_Unhandled_Requests()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player = new Player
                {
                    Id   = 1,
                    Name = "Philippe Coutinho"
                };
                await _handler
                    .Send(new UpdatePlayer { Player = player }
                    .RouteTo("http://localhost:9000/process"));
            }
        }

        [TestMethod]
        public async Task Should_Fail_Validation_Rules()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                try
                {
                    await _handler.Send(new CreatePlayer()
                        .RouteTo("http://localhost:9000/process"));
                    Assert.Fail("Should have failed");
                }
                catch (ValidationException vex)
                {
                    var outcome = vex.Outcome;
                    Assert.IsNotNull(outcome);
                    CollectionAssert.AreEqual(new[] { "Player" }, outcome.Culprits);
                    Assert.AreEqual("'Player' must not be empty.", outcome["Player"]);
                }
            }
        }
    }
}
