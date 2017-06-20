namespace Example.Tests.MediatorTests.WithComposerTests
{
    using System.Threading.Tasks;
    using Mediator;
    using Mediator.WithBaseClasses;
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
