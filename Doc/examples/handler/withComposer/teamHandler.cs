namespace Example.Handler.WithComposer
{
    using League.Api.Team;
    using Miruken.Callback;
    using Miruken.Mediate;

    public class TeamHandler
    {
        [Mediates]
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