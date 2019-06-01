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

            HandlerDescriptorFactory.Current.RegisterDescriptor<Pipeline.TeamHandler>();

            Controller.Context.AddHandlers(
                new WindsorHandler(container),
                new Pipeline.TeamHandler());

            await AssertCanCreateTeam();
        }
    }
}
