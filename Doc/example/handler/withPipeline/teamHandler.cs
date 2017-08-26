namespace Example.Handler.WithPipeline
{
    using League.Api.Team;
    using Miruken.Mediate;

    public class TeamHandler : PipelineHandler
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