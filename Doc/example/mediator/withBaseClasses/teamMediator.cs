namespace Example.Mediator.WithBaseClasses
{
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

        [Mediates]
        public void RemoveTeam(RemoveTeam request)
        {
            //remove team
        }
    }
}