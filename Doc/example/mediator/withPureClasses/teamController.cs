namespace Example.Mediator.WithPureClasses
{
    using System.Threading.Tasks;
    using Miruken.Context;
    using Miruken.Mediator;

    public class TeamController
    {
        private readonly Context _context;

        public TeamController()
        {
            _context  = new Context();
            _context.AddHandlers(new TeamHandler());
        }

        public async Task<Team> CreateTeam(Team team)
        {
            var result = await _context.Send<TeamResult>(new CreateTeam { Team = team });
            return result.Team;
        }
    }
}
