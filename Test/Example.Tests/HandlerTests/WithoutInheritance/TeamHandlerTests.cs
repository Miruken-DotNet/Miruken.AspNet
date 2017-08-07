namespace Example.Tests.HandlerTests.WithoutInheritance
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Callback.Policy;

    [TestClass]
    public class TeamHandlerTests : HandlerTestScenario
    {
        [TestMethod]
        public async Task CanCreateTeam()
        {
            HandlerDescriptor.GetDescriptor<Handler.WithoutInheritance.TeamHandler>();
            Controller.Context.AddHandlers(new Handler.WithoutInheritance.TeamHandler());
            await AssertCanCreateTeam();
        }
    }
}
