namespace Miruken.AspNet.Castle.Tests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Callback;
    using Context;
    using global::Castle.Facilities.Logging;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Services.Logging.NLogIntegration;
    using global::Castle.Windsor;
    using Http;
    using Mediate;
    using Mediate.Castle;
    using Mediate.Route;
    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using Owin;
    using Validate;
    using Validate.Castle;

    [TestClass]
    public class HttRouteControllerTests
    {
        private MemoryTarget _memoryTarget;
        private LoggingConfiguration _config;
        private WindsorHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _config = new LoggingConfiguration();
            _memoryTarget   = new MemoryTarget
            {
                Layout = "${date} [${threadid}] ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}"
            };
            _config.AddTarget("InMemoryTarget", _memoryTarget);
            _config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _memoryTarget));
            LogManager.Configuration = _config;

            _handler = new WindsorHandler(container => container
                .AddFacility<LoggingFacility>(f => f.LogUsing(new NLogFactory(_config)))
                .Install(new FeaturesInstaller(
                    new HandleFeature(), new ValidateFeature(),
                    new MediateFeature().WithStandardMiddleware()).Use(
                        Classes.FromAssemblyContaining<HttpRouter>(),
                        Classes.FromThisAssembly()))
            );
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
                  .AddFacility<LoggingFacility>(f => f.LogUsing(new NLogFactory(_config)))
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
                    .RouteTo("http://localhost:9000"));
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
                    .RouteTo("http://localhost:9000"));
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
                        .RouteTo("http://localhost:9000/"));
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

        [TestMethod]
        public async Task Should_Log_Client_And_Server_Requests()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player = new Player
                {
                    Name = "Philippe Coutinho"
                };
                await _handler.Send(new CreatePlayer { Player = player }
                    .RouteTo("http://localhost:9000"));
            }

            var events = _memoryTarget.Logs;
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Http\.HttpRouter.*Handling RoutedRequest<PlayerResponse>").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Http\.Post\.PostHandler.*Handling PostRequest<Message, Try<Message, Message>>").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.AspNet\.Castle\.Tests\.PlayerHandler.*Handling CreatePlayer ").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.AspNet\.Castle\.Tests\.PlayerHandler.*Completed CreatePlayer ").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Http\.Post\.PostHandler.*Completed PostRequest<Message, Try<Message, Message>>").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Http\.HttpRouter.*Completed RoutedRequest<PlayerResponse>").Success));
        }

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Reject_Invalid_Route()
        {
            await _handler.Send(new CreatePlayer()
                .RouteTo("abc://localhost:9000"));
        }

        [TestMethod]
        public async Task Should_Batch_Requests()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player1 = new Player
                {
                    Name = "Paul Pogba"
                };
                var player2 = new Player
                {
                    Name = "Eden Hazard"
                };
                var results = await _handler.Batch(batch =>
                {
                    batch.Send(new CreatePlayer { Player = player1 }
                        .RouteTo("http://localhost:9000"))
                        .Then((response, s) =>
                        {
                            Assert.AreEqual("Paul Pogba", response.Player.Name);
                            Assert.IsTrue(response.Player.Id > 0);
                        });
                    batch.Send(new CreatePlayer { Player = player2 }
                            .RouteTo("http://localhost:9000"))
                        .Then((response, s) =>
                        {
                            Assert.AreEqual("Eden Hazard", response.Player.Name);
                            Assert.IsTrue(response.Player.Id > 0);
                        });
                });
                Assert.AreEqual(1, results.Length);
                var groups = (object[])results[0];
                Assert.AreEqual(1, groups.Length);
                var group = (Tuple<string, object[]>)groups[0];
                Assert.AreEqual("http://localhost:9000", group.Item1);
                Assert.AreEqual(2, group.Item2.Length);
            }
        }

        [TestMethod]
        public async Task Should_Propogate_Failure()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                try
                {
                    await _handler.Batch(batch =>
                        batch.Send(new CreatePlayer { Player = new Player() }
                            .RouteTo("http://localhost:9000")));
                }
                catch (ValidationException vex)
                {
                    var outcome = vex.Outcome;
                    Assert.IsNotNull(outcome);
                    CollectionAssert.AreEqual(new[] { "Player" }, outcome.Culprits);
                    Assert.AreEqual("'Player. Name' should not be empty.", outcome["Player.Name"]);
                }
            }
        }

        [TestMethod]
        public async Task Should_Propogate_Multiple_Failures()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                try
                {
                    await _handler.Batch(batch =>
                    {
                        batch.Send(new CreatePlayer { Player = new Player() }
                            .RouteTo("http://localhost:9000"));
                        batch.Send(new CreatePlayer { Player = new Player
                                   {
                                       Id   = 3,
                                       Name = "Sergio Ramos"
                                   }}
                            .RouteTo("http://localhost:9000"));
                    });
                }
                catch (AggregateException ex)
                {
                    Assert.AreEqual(2, ex.InnerExceptions.Count);
                }
            }
        }
    }
}
