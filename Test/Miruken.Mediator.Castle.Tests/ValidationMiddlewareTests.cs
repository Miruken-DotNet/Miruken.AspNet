namespace Miruken.Mediator.Castle.Tests
{
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Mediator.Tests;
    using Validate;
    using Validate.Castle;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class ValidationMiddlewareTests
    {
        protected IWindsorContainer _container;
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<TeamHandler>();

            _container = new WindsorContainer()
                .Install(new FeaturesInstaller(
                    new MediatorInstaller().WithStandardMiddleware(),
                    new ValidationInstaller()).Use(
                        Classes.FromAssemblyContaining<Team>()));
            _container.Kernel.AddHandlersFilter(new ContravariantFilter());
            _handler = new TeamHandler()
                     + new WindsorHandler(_container)
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
        public async Task Should_Validate_Request()
        {
            var team = await _handler.Send(new CreateTeam
            {
                Team = new Team
                {
                    Name = "Liverpool Owen"
                }
            });
            Assert.IsTrue(team.ValidationOutcome.IsValid);
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);
        }

        [TestMethod]
        public async Task Should_Match_All_Validators()
        {
            try
            {
                await _handler.Send(new RemoveTeam
                {
                    Team = new Team()
                });
                Assert.Fail("Should not succeed");
            }
            catch (ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                var team = outcome.GetOutcome("Team");
                Assert.IsNotNull(team);
                CollectionAssert.AreEqual(new[] { "Id" }, team.Culprits);
                Assert.AreEqual("'Team. Id' must be greater than '0'.", team["Id"]);
            }
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request()
        {
            try
            {
                await _handler.Send(new CreateTeam());
            }
            catch (ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new[] { "Team" }, outcome.Culprits);
                Assert.AreEqual("'Team' should not be empty.", outcome["Team"]);
            }
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request_Content()
        {
            try
            {
                await _handler.Send(new CreateTeam
                {
                    Team = new Team()
                });
                Assert.Fail("Should not succeed");
            }
            catch (ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                var team = outcome.GetOutcome("Team");
                Assert.IsNotNull(team);
                CollectionAssert.AreEqual(new[] { "Name" }, team.Culprits);
                Assert.AreEqual("'Name' should not be empty.", team["Name"]);
            }
        }

        [Pipeline]
        public class TeamHandler : Handler
        {
            public int _teamId;

            [Mediates]
            public Promise<Team> Create(CreateTeam create, IHandler composer)
            {
                var team = create.Team;
                team.Id = ++_teamId;
                team.Active = true;

                composer.Publish(new TeamCreated { Team = team });
                return Promise.Resolved(team);
            }

            [Mediates]
            public void Remove(RemoveTeam remove, IHandler composer)
            {
            }
        }
    }
}
