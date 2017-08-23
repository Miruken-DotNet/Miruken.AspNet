namespace Miruken.AspNet
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Http;

    public class HttpRouteController : ApiController
    {
        [HttpPost]
        public async Task<Message> Process(Message message)
        {
            return null;
        }

        [HttpPost]
        public async Task<Message> Publish(Message message)
        {
            return null;
        }
    }
}
