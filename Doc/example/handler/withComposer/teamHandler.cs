namespace Example.Handler.WithComposer
{
    using Miruken.Callback;
    using Miruken.Mediator;
    using Team;

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