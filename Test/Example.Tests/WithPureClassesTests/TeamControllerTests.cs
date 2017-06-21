namespace Example.Tests.WithPureClassesTests
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Context;
    using Team;
    using WithPureClasses;
    using CreateTeam = WithPureClasses.CreateTeam;
    using TeamController = WithPureClasses.TeamController;

    [TestClass]
    public class TeamControllerTests
    {
        [TestMethod]
        public async Task CanCreateTeam()
        {
            var controller = new TeamController { Context = new Context() };
            controller.Context.AddHandlers(new TeamHandler());
            var team = await controller.CreateTeam(new CreateTeam { Team = new Team() });
            Assert.IsNotNull(team);
        }
    }
}
