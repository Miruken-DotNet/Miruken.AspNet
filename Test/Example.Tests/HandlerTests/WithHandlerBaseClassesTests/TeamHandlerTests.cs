namespace Example.Tests.HandlerTests.WithHandlerBaseClassesTests
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanHandleMessages()
        {
            Controller.Context.AddHandlers(new Handler.WithHandlerBaseClass.TeamHandler());
            await AssertCanCreateTeam();
            await AssertCanRemoveTeam();
        }
    }
}
