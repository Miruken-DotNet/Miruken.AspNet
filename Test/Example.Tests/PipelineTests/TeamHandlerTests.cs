namespace Example.Tests.PipelineTests
{
    using System.Threading.Tasks;
    using Castle.Windsor;
    using HandlerTests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;
    using Miruken.Mediator.Castle;
    using Miruken.Validate.Castle;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanHandleMessages()
        {
            var container = new WindsorContainer();
            container.Install(
                Features.FromAssemblies(),
                new MediatorInstaller(),
                new ValidationInstaller());

            Controller.Context.AddHandlers(new WindsorHandler(container));
            Controller.Context.AddHandlers(new Pipeline.TeamHandler());
            await AssertCanCreateTeam();
        }
    }
}
