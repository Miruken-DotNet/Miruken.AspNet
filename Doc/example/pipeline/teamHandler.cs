namespace Example.Pipeline
{
    using League.Api.Team;
    using Miruken.Callback;
    using Miruken.Mediate;
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
        public void CreateTeamIntegritey(CreateTeam request, ValidationOutcome outcome)
        {
            if (request.Team == null)
                outcome.AddError("Team", "Something really bad");
        }
    }
}