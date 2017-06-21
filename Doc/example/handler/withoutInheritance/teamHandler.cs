namespace Example.Handler.WithoutInheritance
{
    using Miruken.Mediator;
    using Team;

    public class TeamHandler
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