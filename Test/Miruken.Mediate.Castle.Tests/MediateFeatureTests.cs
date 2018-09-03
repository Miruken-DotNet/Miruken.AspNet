namespace Miruken.Mediate.Castle.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Mediate.Tests;
    using Validate;
    using Validate.Castle;
    using ValidationException = Validate.ValidationException;

    [TestClass]
    public class MediateFeatureTests
    {
        protected IWindsorContainer _container;
        protected Callback.IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.ResetDescriptors();

            _container = new WindsorContainer()
                .Install(new FeaturesInstaller(
                    new HandleFeature()
                        .AddFilters(typeof(ValidateFilter <,>)),
                    new ValidateFeature()).Use(
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
        public void Should_Register_Filters_By_Interface()
        {
            var filters = _container.ResolveAll<IFilter<GetStockQuote, StockQuote>>();
            Assert.AreEqual(2, filters.Length);
            Assert.IsTrue(filters.Any(m => m is LogFilter<GetStockQuote, StockQuote>));
            Assert.IsTrue(filters.Any(m => m is ValidateFilter<GetStockQuote, StockQuote>));
        }

        [TestMethod]
        public void Should_Register_Filters_By_Class()
        {
            Assert.IsNotNull(_container.Resolve<LogFilter<GetStockQuote, StockQuote>>());
            Assert.IsNotNull(_container.Resolve<ValidateFilter<GetStockQuote, StockQuote>>());
        }

        [TestMethod]
        public async Task Should_Send_Request_With_Response()
        {
            var team = await _handler.Infer()
                .Send(new CreateTeam
                {
                    Team = new Team
                    {
                        Name = "Liverpool Owen"
                    }
                });
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);
        }

        [TestMethod]
        public async Task Should_Send_Request_With_Response_Dynamic()
        {
            var team = await _handler.Infer()
                .Send<Team>((object)new CreateTeam
            {
                Team = new Team
                {
                    Name = "Liverpool Owen"
                }
            });
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);
        }

        [TestMethod]
        public async Task Should_Send_Request_Without_Response()
        {
            var team = new Team
            {
                Id     = 1,
                Name   = "Liverpool Owen",
                Active = true
            };

            await _handler.Infer()
                .Send(new RemoveTeam { Team = team });
            Assert.IsFalse(team.Active);
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request()
        {
            try
            {
                await _handler.Infer().Send(new CreateTeam());
                Assert.Fail("Should have rejected request");
            }
            catch (ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new[] {"Team"}, outcome.Culprits);
                Assert.AreEqual("'Team' should not be empty.", outcome["Team"]);
            }
        }
    }
}
