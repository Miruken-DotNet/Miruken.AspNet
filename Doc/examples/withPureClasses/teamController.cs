namespace Example.WithPureClasses
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using League.Api.Team;
    using Miruken.Api;
    using Miruken.Context;

    public class TeamController : ApiController
    {
        public Context Context { get; set; }

        [HttpPost]
        public async Task<Team> CreateTeam(CreateTeam request)
        {
            var result = await Context.Send<TeamResult>(request);
            return result.Team;
        }
    }
}
