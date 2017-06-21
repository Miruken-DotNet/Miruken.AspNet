namespace Example.Tests.HandlerTests.WithPipelineTests
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanHandleMessages()
        {
            Controller.Context.AddHandlers(new Handler.WithPipeline.TeamHandler());
            await AssertCanCreateTeam();
            await AssertCanRemoveTeam();
        }
    }
}
