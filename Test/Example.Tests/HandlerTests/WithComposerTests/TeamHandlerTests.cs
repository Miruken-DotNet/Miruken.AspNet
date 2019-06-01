namespace Example.Tests.HandlerTests.WithComposerTests
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
            HandlerDescriptorFactory.Current.RegisterDescriptor<Handler.WithComposer.TeamHandler>();
            Controller.Context.AddHandlers(new Handler.WithComposer.TeamHandler());
            await AssertCanCreateTeam();
        }
    }
}
