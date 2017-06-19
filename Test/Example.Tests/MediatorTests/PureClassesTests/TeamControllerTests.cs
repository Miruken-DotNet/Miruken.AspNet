namespace Example.Tests.MediatorTests.PureClassesTests
{
    using System.Threading.Tasks;
    using Mediator;
    using Mediator.PureClasses;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TeamControllerTests
    {
        [TestMethod]
        public async Task CanCreateTeam()
        {
            var controller = new TeamController();
            var team = await controller.CreateTeam(new Team());
            Assert.IsNotNull(team);
        }

        [TestMethod]
        public async Task CanRemoveTeam()
        {
            var controller = new TeamController();
            await controller.RemoveTeam(new Team());
        }
    }
}
