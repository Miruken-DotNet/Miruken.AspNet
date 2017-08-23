namespace Example.League.Api.Team
{
    using Miruken.Mediate;

    public class CreateTeam : IRequest<TeamResult>
    {
        public Team Team { get; set; }
    }
}