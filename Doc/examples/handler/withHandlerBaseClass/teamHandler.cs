namespace Example.Handler.WithHandlerBaseClass
{
    using League.Api.Team;
    using Miruken.Callback;

    public class TeamHandler : Handler
    {
        [Handles]
        public TeamResult CreateTeam(CreateTeam request)
        {
            return new TeamResult
            {
                Team = request.Team
            };
        }

        [Handles]
        public void RemoveTeam(RemoveTeam request)
        {
            //remove team
        }
    }
}