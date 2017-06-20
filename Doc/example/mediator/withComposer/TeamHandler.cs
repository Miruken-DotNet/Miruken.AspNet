namespace Example.Mediator.WithComposer
{
    using Mediator;
    using Miruken.Callback;
    using Miruken.Mediator;

    public class TeamHandler
    {
        [Mediates]
        public TeamResult CreateTeam(CreateTeam request, IHandler composer)
        {
            var team = new Team();

            composer.Publish(new TeamCreated { Team = team });

            return new TeamResult
            {
                Team = team
            };
        }
    }
}