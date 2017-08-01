namespace Miruken.Mediator.Castle.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using global::Castle.MicroKernel;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Mediator.Tests;
    using Validate;
    using Validate.Castle;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class MediatorInstallerTests
    {
        protected IWindsorContainer _container;
        protected Callback.IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _container = new WindsorContainer()
                .Install(WithFeatures.FromAssemblies(typeof(Team).Assembly),
                         new ValidationInstaller(),
                         new MediatorInstaller().WithMiddleware(),
                         new HandlerInstaller());
            _container.Kernel.AddHandlersFilter(new ContravariantFilter());
            _handler = new WindsorHandler(_container)
                     + new DataAnnotationsValidator()
                     + new FluentValidationValidator()
                     + new ValidationHandler();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
        }

        [TestMethod]
        public void Should_Register_Middleware_By_Interface()
        {
            var middleWare = _container.ResolveAll<IMiddleware<GetStockQuote, StockQuote>>();
            Assert.AreEqual(2, middleWare.Length);
            Assert.IsTrue(middleWare.Any(m => m is LoggingMiddleware<GetStockQuote, StockQuote>));
            Assert.IsTrue(middleWare.Any(m => m is ValidationMiddleware<GetStockQuote, StockQuote>));
        }

        [TestMethod]
        public void Should_Register_Middleware_By_Class()
        {
            Assert.IsNotNull(_container.Resolve<LoggingMiddleware<GetStockQuote, StockQuote>>());
            Assert.IsNotNull(_container.Resolve<ValidationMiddleware<GetStockQuote, StockQuote>>());
        }

        [TestMethod,
         ExpectedException(typeof(ComponentNotFoundException))]
        public void Should_Not_Install_Middleware_By_Default()
        {
            var container = new WindsorContainer().Install(new MediatorInstaller());
            container.Resolve<LoggingMiddleware<string, int>>();
        }

        [TestMethod]
        public void Should_Install_Specific_Middleware()
        {
            var container = new WindsorContainer()
                .Install(new MediatorInstaller().WithMiddleware(typeof(LoggingMiddleware<,>)));
            var logging = container.Resolve<LoggingMiddleware<string, int>>();
            Assert.IsNotNull(logging);
        }

        [TestMethod,
         ExpectedException(typeof(ArgumentException))]
        public void Should_Reject_Invalid_Middleware()
        {
             new WindsorContainer()
                .Install(new MediatorInstaller().WithMiddleware(typeof(MediatorInstallerTests)));
        }

        [TestMethod]
        public async Task Should_Send_Request_With_Response()
        {
            var team = await _handler.Resolve()
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
            var team = await _handler.Resolve()
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

            await _handler.Resolve()
                .Send(new RemoveTeam { Team = team });
            Assert.IsFalse(team.Active);
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request()
        {
            try
            {
                await _handler.Resolve().Send(new CreateTeam());
                Assert.Fail("Should have rejected request");
            }
            catch (ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new[] { "Team" }, outcome.Culprits);
                Assert.AreEqual("'Team' should not be empty.", outcome["Team"]);
            }
        }
    }
}
