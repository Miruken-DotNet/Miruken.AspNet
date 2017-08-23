namespace Miruken.Mediate.Castle.Tests
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using global::Castle.Facilities.Logging;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Services.Logging.NLogIntegration;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Mediate.Tests;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using Validate;
    using Validate.Castle;

    [TestClass]
    public class LogMiddlewareTests
    {
        protected IWindsorContainer _container;
        protected MemoryTarget _memoryTarget;
        private IHandler _handler;
    
        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.ResetDescriptors();

            var config = new LoggingConfiguration();
            _memoryTarget = new MemoryTarget
            {
                Layout = "${date} [${threadid}] ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}"
            };
            config.AddTarget("InMemoryTarget", _memoryTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _memoryTarget));
            LogManager.Configuration = config;
            _container = new WindsorContainer()
                .AddFacility<LoggingFacility>(f => f.LogUsing(new NLogFactory(config)))
                .Install(new FeaturesInstaller(
                    new HandlerFeature(), new ValidationFeature(),
                    new MediateFeature().WithStandardMiddleware()).Use(
                        Types.From(typeof(TeamIntegrity),
                                   typeof(TeamActionIntegrity),
                                   typeof(RemoveTeamIntegrity),
                                   typeof(HandlerMediateTests.TeamHandler))));
            _container.Kernel.AddHandlersFilter(new ContravariantFilter());

            _handler = new WindsorHandler(_container);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
        }

        [TestMethod]
        public async Task Should_Send_Request_With_Response()
        {
            var team = await _handler.Send(new CreateTeam
            {
                Team = new Team
                {
                    Name = "Liverpool Owen"
                }
            });
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);

            var events = _memoryTarget.Logs;
            Assert.AreEqual(4, events.Count);
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Handling CreateTeam").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Handling TeamCreated").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Completed TeamCreated").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Completed CreateTeam.*with Team").Success));
        }

        [TestMethod]
        public async Task Should_Send_Request_Without_Response()
        {
            var team = new Team
            {
                Id = 1,
                Name = "Liverpool Owen",
                Active = true
            };

            await _handler.Send(new RemoveTeam { Team = team });
            Assert.IsFalse(team.Active);

            var events = _memoryTarget.Logs;
            Assert.AreEqual(4, events.Count);
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Handling RemoveTeam").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Handling TeamRemoved").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Completed TeamRemoved").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Completed RemoveTeam").Success));
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request()
        {
            try
            {
                await _handler.Send(new CreateTeam());
                Assert.Fail("Should have rejected request");
            }
            catch (ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new[] { "Team" }, outcome.Culprits);
                Assert.AreEqual("'Team' should not be empty.", outcome["Team"]);
            }

            var events = _memoryTarget.Logs;
            Assert.AreEqual(2, events.Count);
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"DEBUG.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Handling CreateTeam").Success));
            Assert.IsTrue(events.Any(x => Regex.Match(x,
                @"WARN.*Miruken\.Mediate\.Tests\.HandlerMediateTests\+TeamHandler.*Failed CreateTeam").Success));
        }
    }
}
