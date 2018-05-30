namespace Example.Tests.PipelineTests
{
    using System.Threading.Tasks;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using HandlerTests;
    using League.Api.Team;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Callback.Policy;
    using Miruken.Castle;
    using Miruken.Validate;
    using Miruken.Validate.Castle;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanHandleMessages()
        {
            var container = new WindsorContainer();
            container.Install(new FeaturesInstaller(new ValidateFeature())
                .Use(Classes.FromAssemblyContaining<CreateTeam>(),
                     Classes.FromAssemblyContaining<Pipeline.TeamHandler>()));

            HandlerDescriptor.GetDescriptor<Pipeline.TeamHandler>();

            Controller.Context.AddHandlers(
                new WindsorHandler(container),
                new ValidationHandler(),
                new Pipeline.TeamHandler());

            await AssertCanCreateTeam();
        }
    }
}
