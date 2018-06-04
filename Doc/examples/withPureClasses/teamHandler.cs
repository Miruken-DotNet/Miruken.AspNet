namespace Example.WithPureClasses
{
    using League.Api.Team;
    using Miruken.Callback;

    public class TeamHandler
    {
        [Handles]
        public TeamResult CreateTeam(CreateTeam request)
        {
            return new TeamResult
            {
                Team = request.Team
            };
        }
    }
}