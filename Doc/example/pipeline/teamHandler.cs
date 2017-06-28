namespace Example.Pipeline
{
    using League.Api.Team;
    using Miruken.Callback;
    using Miruken.Mediator;

    [Pipeline]
    public class TeamHandler : Handler
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