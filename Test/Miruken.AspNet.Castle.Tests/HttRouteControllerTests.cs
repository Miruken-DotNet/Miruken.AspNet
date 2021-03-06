﻿namespace Miruken.AspNet.Castle.Tests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Api;
    using Api.Route;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using Context;
    using Functional;
    using global::Castle.Facilities.Logging;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Services.Logging.NLogIntegration;
    using global::Castle.Windsor;
    using Http;
    using Http.Post;
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
            _config       = new LoggingConfiguration();
            _memoryTarget = new MemoryTarget
            {
                Layout = "${date} [${threadid}] ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}"
            };
            _config.AddTarget("InMemoryTarget", _memoryTarget);
            _config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _memoryTarget));
            LogManager.Configuration = _config;

            _handler = new WindsorHandler(container => container
                .AddFacility<LoggingFacility>(f => f.LogUsing(new NLogFactory(_config)))
                .Install(new FeaturesInstaller(
                    new HandleFeature().AddFilters(
                        new FilterAttribute(typeof(LogFilter<,>))),
                    new ValidateFeature()).Use(
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
                      new HandleFeature(), new ValidateFeature(), new AspNetFeature())
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

        [TestMethod]
        public async Task Should_Route_Request_Without_Castle()
        {
            HandlerDescriptorFactory.Current.RegisterDescriptor<FilterProvider>();

            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player = new Player
                {
                    Name = "Philippe Coutinho"
                };
                var handler = HttpApiClient.Handler + new FilterProvider();
                var response = await handler.Send(
                    new CreatePlayer {Player = player}.RouteTo(
                    "http://localhost:9000"));
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
                    .Send(new RemovePlayer { Player = player }
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
            foreach (var handler in new[] { typeof(HttpRouter), typeof(PostHandler) })
            {
                HandlerDescriptorFactory.Current.RegisterDescriptor(handler)
                    .AddFilters(new FilterAttribute(typeof(LogFilter<,>)));
            }
            
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
        public async Task Should_Batch_Single_Request()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player = new Player
                {
                    Name = "Paul Pogba"
                };
                var results = await _handler.Batch(batch =>
                {
                    batch.Send(new CreatePlayer { Player = player }
                            .RouteTo("http://localhost:9000"))
                        .Then((response, s) =>
                        {
                            Assert.AreEqual("Paul Pogba", response.Player.Name);
                            Assert.IsTrue(response.Player.Id > 0);
                        });
                });
                Assert.AreEqual(1, results.Length);
                var groups = (object[])results[0];
                Assert.AreEqual(1, groups.Length);
                var group = (Tuple<string, object[]>)groups[0];
                Assert.AreEqual("http://localhost:9000", group.Item1);
                Assert.AreEqual(1, group.Item2.Length);
            }
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
        public async Task Should_Not_Batch_Awaited_Requests()
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
                await _handler.Batch(async batch =>
                {
                    var response = await batch.Send(new CreatePlayer { Player = player1 }
                            .RouteTo("http://localhost:9000"));
                    Assert.AreEqual("Paul Pogba", response.Player.Name);
                    Assert.IsTrue(response.Player.Id > 0);
                    await batch.Send(new CreatePlayer { Player = player2 }
                            .RouteTo("http://localhost:9000"))
                        .Then((resp, s) =>
                        {
                            Assert.AreEqual("Eden Hazard", resp.Player.Name);
                            Assert.IsTrue(response.Player.Id > 0);
                        });
                });
            }
        }

        [TestMethod]
        public async Task Should_Batch_Publications()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var player1 = new Player
                {
                    Name = "Paul Pogba"
                };
                var player2 = new Player
                {
                    Id   = 1,
                    Name = "Eden Hazard"
                };
                var results = await _handler.Batch(batch =>
                {
                    batch.Publish(new PlayerCreated { Player = player1 }
                            .RouteTo("http://localhost:9000"));
                    batch.Publish(new PlayerUpdated { Player = player2 }
                            .RouteTo("http://localhost:9000"));
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
        public async Task Should_Propagate_Failure()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var results = await _handler.Batch(batch =>
                    batch.Send(new CreatePlayer { Player = new Player() }
                        .RouteTo("http://localhost:9000"))
                        .Then((_, s) => Assert.Fail("Should have failed"))
                        .Catch((ValidationException vex, bool s) =>
                        {
                            var outcome = vex.Outcome;
                            Assert.IsNotNull(outcome);
                            CollectionAssert.AreEqual(new[] { "Player" }, outcome.Culprits);
                            Assert.AreEqual("'Player. Name' must not be empty.", outcome["Player.Name"]);
                        })
                        .Catch((ex, s) => Assert.Fail("Unexpected exception")));
                Assert.AreEqual(1, results.Length);
                var groups = (object[])results[0];
                Assert.AreEqual(1, groups.Length);
                var group = (Tuple<string, object[]>)groups[0];
                Assert.AreEqual("http://localhost:9000", group.Item1);
                Assert.AreEqual(1, group.Item2.Length);
            }
        }

        [TestMethod]
        public async Task Should_Propagate_Multiple_Failures()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var results = await _handler.Batch(batch =>
                {
                    batch.Send(new CreatePlayer { Player = new Player() }
                        .RouteTo("http://localhost:9000"))
                        .Catch((ValidationException vex, bool s) =>
                        {
                            var outcome = vex.Outcome;
                            Assert.IsNotNull(outcome);
                            CollectionAssert.AreEqual(new[] { "Player" }, outcome.Culprits);
                            Assert.AreEqual("'Player. Name' must not be empty.", outcome["Player.Name"]);
                        })
                        .Catch((ex, s) => Assert.Fail("Unexpected exception"));
                    batch.Send(new CreatePlayer { Player = new Player
                        {
                            Id   = 3,
                            Name = "Sergio Ramos"
                        }}
                        .RouteTo("http://localhost:9000"))
                        .Catch((ValidationException vex, bool s) =>
                        {
                            var outcome = vex.Outcome;
                            Assert.IsNotNull(outcome);
                            CollectionAssert.AreEqual(new[] { "Player" }, outcome.Culprits);
                            Assert.AreEqual("'Player. Id' should be equal to '0'.", outcome["Player.Id"]);
                        })
                        .Catch((ex, s) => Assert.Fail("Unexpected exception"));
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
        public async Task Should_Propagate_Multiple_Unknown_Failures()
        {
            var count     = 0;
            var completed = false;
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var results = await _handler.Batch(batch => Promise.All(
                    batch.Send(new RemovePlayer { Player = new Player() }
                        .RouteTo("http://localhost:9000"))
                        .Catch((NotSupportedException ex, bool s) =>
                        {
                            ++count;
                            Assert.AreEqual(
                                "Miruken.AspNet.Castle.Tests.RemovePlayer not handled",
                                ex.Message);
                        })
                        .Catch((ex, s) => Assert.Fail("Unexpected exception")),
                    batch.Send(new RemovePlayer
                        {
                            Player = new Player { Id   = 3 }
                        }
                        .RouteTo("http://localhost:9000"))
                        .Catch((NotSupportedException ex, bool s) =>
                        {
                            ++count;
                            Assert.AreEqual(
                                "Miruken.AspNet.Castle.Tests.RemovePlayer not handled",
                                ex.Message);
                        })
                        .Catch((ex, s) => Assert.Fail("Unexpected exception")))
                ).Then((r, s) =>
                {
                    Assert.AreEqual(2, count);
                    completed = true;
                });
                Assert.AreEqual(1, results.Length);
                var groups = (object[])results[0];
                Assert.AreEqual(1, groups.Length);
                var group = (Tuple<string, object[]>)groups[0];
                Assert.AreEqual("http://localhost:9000", group.Item1);
                Assert.AreEqual(2, group.Item2.Length);
                Assert.IsTrue(completed);
            }
        }

        [TestMethod]
        public async Task Should_Propagate_Format_Errors()
        {
            using (WebApp.Start("http://localhost:9000/", Configuration))
            {
                var response = await _handler
                    .Formatters(HttpFormatters.Route)
                    .HttpPost<string, Try<Message, Message>>(
                    @"{
                       'payload': {
                           '$type': 'Miruken.AspNet.Castle.Tests.CreatePlayer, Miruken.AspNet.Castle.Tests',
                           'player': {
                              'id':   'ABC',
                              'name': 'Namee3c27ad6-b812-46c1-9b60-d99c9d3e7ba6',
                                  'person': {
                                  'dob': 'XYZ'
                              }
                           }
                        }
                    }", "http://localhost:9000/Process",
                    HttpFormatters.Route);
                response.Match(error =>
                {
                    var errors = (ValidationErrors[])error.Payload;
                    Assert.AreEqual(1, errors.Length);
                }, success => { Assert.Fail("Should have failed"); });
            }
        }

        private class FilterProvider : Handler
        {
            [Provides]
            public LogFilter<TReq, TResp> CreateLogFilter<TReq, TResp>()
            {
                return new LogFilter<TReq, TResp>();
            }
        }
    }
}
