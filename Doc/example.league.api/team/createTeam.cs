namespace Example.League.Api.Team
{
    using Miruken.Api;

    public class CreateTeam : IRequest<TeamResult>
    {
        public Team Team { get; set; }
    }
}