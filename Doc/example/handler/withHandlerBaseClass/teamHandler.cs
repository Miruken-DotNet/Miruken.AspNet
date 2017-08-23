namespace Example.Handler.WithHandlerBaseClass
{
    using League.Api.Team;
    using Miruken.Callback;
    using Miruken.Mediate;

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

        [Mediates]
        public void RemoveTeam(RemoveTeam request)
        {
            //remove team
        }
    }
}