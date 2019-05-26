namespace Example.Tests.HandlerTests.WithPipelineTests
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Callback.Policy;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanHandleMessages()
        {
            HandlerDescriptorFactory.Current.GetDescriptor<Handler.WithPipeline.TeamHandler>();
            Controller.Context.AddHandlers(new Handler.WithPipeline.TeamHandler());
            await AssertCanCreateTeam();
            await AssertCanRemoveTeam();
        }
    }
}
