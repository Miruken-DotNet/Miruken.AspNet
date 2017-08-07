namespace Example.Tests.HandlerTests
{
    using System.Threading.Tasks;
    using League.Api.Team;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Context;
    using Team;

    public abstract class HandlerTestScenario
    {
        protected TeamController Controller;

        [TestInitialize]
        public void TestInitialize()
        {
            Controller  = new TeamController();
            var context = new Context();
            Controller.Context = context;
        }

        protected async Task AssertCanCreateTeam()
        {
            var team = await Controller.CreateTeam(new CreateTeam
            {
                Team = new Team {Id = 1, Name = "my team"}
            });
            Assert.IsNotNull(team);
            Assert.IsTrue(!string.IsNullOrEmpty(team.Name));
            Assert.IsTrue(team.Id > 0);
        }

        protected async Task AssertCanRemoveTeam()
        {
            var removeTeam = new RemoveTeam {Team = new Team()};
            Assert.IsNotNull(removeTeam.Team);
            await Controller.RemoveTeam(removeTeam);
        }
    }
}