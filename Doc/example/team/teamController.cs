namespace Example.Team
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Miruken.Context;
    using Miruken.Mediator;

    public class TeamController : ApiController
    {
        public IContext Context { get; set; }

        [HttpPost]
        public async Task<Team> CreateTeam(CreateTeam request)
        {
            var result  = await Context.Send(request);
            return result.Team;
        }

        [HttpDelete]
        public async Task RemoveTeam(RemoveTeam request)
        {
            await Context.Send(request);
        }
    }
}
