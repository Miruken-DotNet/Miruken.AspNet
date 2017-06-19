namespace Example.Mediator.PureClasses
{
    using Miruken.Mediator;

    public class TeamMediator
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