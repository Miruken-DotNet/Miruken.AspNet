namespace Example.teams
{
    using Miruken.Mediator;

    public class CreateTeam : IRequest<Team>
    {
        public Team Team { get; set; }
    }
}