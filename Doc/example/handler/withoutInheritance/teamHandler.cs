namespace Example.Handler.WithoutInheritance
{
    using League.Api.Team;
    using Miruken.Mediate;

    public class TeamHandler
    {
        [Mediates]
        public TeamResult CreateTeam(CreateTeam request)
        {
            return new TeamResult
            {
                Team = request.Team
            };
        }
    }
}