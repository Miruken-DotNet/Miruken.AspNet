namespace Example.Tests.HandlerTests.WithHandlerBaseClassesTests
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
            HandlerDescriptor.GetDescriptor<Handler.WithHandlerBaseClass.TeamHandler>();
            Controller.Context.AddHandlers(new Handler.WithHandlerBaseClass.TeamHandler());
            await AssertCanCreateTeam();
            await AssertCanRemoveTeam();
        }
    }
}
