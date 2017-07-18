namespace Example.Tests.PipelineTests
{
    using System.Threading.Tasks;
    using Castle.Windsor;
    using HandlerTests;
    using League.Api.Team;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Miruken.Mediator.Castle;
    using Miruken.Validate;
    using Miruken.Validate.Castle;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanHandleMessages()
        {
            var container = new WindsorContainer();
            container.Install(
                WithFeatures.FromAssemblies(
                    typeof(CreateTeam).Assembly,
                    typeof(Pipeline.TeamHandler).Assembly),
                new MediatorInstaller().WithMiddleware(),
                new ValidationInstaller());

            Controller.Context.AddHandlers(
                new WindsorHandler(container),
                new ValidationHandler(),
                new Pipeline.TeamHandler());

            await AssertCanCreateTeam();
        }
    }
}
