namespace Example.Tests.MediatorTests.WithPureClassesTests
{
    using System.Threading.Tasks;
    using Mediator;
    using Mediator.WithPureClasses;
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
    }
}
