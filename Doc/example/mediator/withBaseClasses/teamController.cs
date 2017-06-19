namespace Example.Mediator.WithBaseClasses
{
    using System.Threading.Tasks;
    using Mediator;
    using Miruken.Context;
    using Miruken.Mediator;

    public class TeamController
    {
        private readonly Context _context;

        public TeamController()
        {
            _context  = new Context();
            _context.AddHandlers(new TeamMediator());
        }

        public async Task<Team> CreateTeam(Team team)
        {
            var result = await _context.Send(new CreateTeam { Team = team });
            return result.Team;
        }

        public async Task RemoveTeam(Team team)
        {
            await _context.Send(new RemoveTeam { Team = team });
        }
    }
}
