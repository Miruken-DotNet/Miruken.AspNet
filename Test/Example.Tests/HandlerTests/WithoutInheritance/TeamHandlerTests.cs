namespace Example.Tests.HandlerTests.WithoutInheritance
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanCreateTeam()
        {
            Controller.Context.AddHandlers(new Handler.WithoutInheritance.TeamHandler());
            await AssertCanCreateTeam();
        }
    }
}
