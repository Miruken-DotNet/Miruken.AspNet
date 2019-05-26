namespace Example.Tests.WithPureClassesTests
{
    using System.Threading.Tasks;
    using League.Api.Team;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Callback.Policy;
    using Miruken.Context;
    using WithPureClasses;
    using CreateTeam = WithPureClasses.CreateTeam;
    using TeamController = WithPureClasses.TeamController;

    [TestClass]
    public class TeamControllerTests
    {
        [TestMethod]
        public async Task CanCreateTeam()
        {
            HandlerDescriptorFactory.Current.GetDescriptor<TeamHandler>();
            var controller = new TeamController { Context = new Context() };
            controller.Context.AddHandlers(new TeamHandler());
            var team = await controller.CreateTeam(new CreateTeam { Team = new Team() });
            Assert.IsNotNull(team);
        }
    }
}
