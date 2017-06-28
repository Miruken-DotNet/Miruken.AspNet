namespace Example.League.Api.Team
{
    using Miruken.Mediator;

    public class CreateTeam : IRequest<TeamResult>
    {
        public Team Team { get; set; }
    }
}