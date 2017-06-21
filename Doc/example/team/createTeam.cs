namespace Example.Team
{
    using Miruken.Mediator;

    public class CreateTeam : IRequest<TeamResult>
    {
        public Team Team { get; set; }
    }
}