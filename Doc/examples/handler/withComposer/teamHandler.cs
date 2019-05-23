namespace Example.Handler.WithComposer
{
    using League.Api.Team;
    using Miruken.Api;
    using Miruken.Callback;

    public class TeamHandler
    {
        [Handles]
        public TeamResult CreateTeam(CreateTeam request, IHandler composer)
        {
            var team = request.Team;

            composer.Publish(new TeamCreated());

            return new TeamResult
            {
                Team = team
            };
        }
    }
}