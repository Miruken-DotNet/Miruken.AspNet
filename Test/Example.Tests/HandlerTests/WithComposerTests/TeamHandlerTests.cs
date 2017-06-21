namespace Example.Tests.HandlerTests.WithComposerTests
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanCreateTeam()
        {
            Controller.Context.AddHandlers(new Handler.WithComposer.TeamHandler());
            await AssertCanCreateTeam();
        }
    }
}
