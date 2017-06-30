namespace Example.Pipeline
{
    using League.Api.Team;
    using Miruken.Callback;
    using Miruken.Mediator;
    using Miruken.Validate;

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

        [Validates]
        public void CreateTeamIntegritey(CreateTeam message, ValidationOutcome outcome)
        {
            outcome.AddError("Team", "Something really bad");
        }
    }
}