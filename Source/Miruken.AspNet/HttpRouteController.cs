namespace Miruken.AspNet
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Callback;
    using Http;
    using Map;
    using Mediate;

    [HttpRoute]
    public class HttpRouteController : ContextualApiController
    {
        [HttpPost, Route("Process")]
        public Task<HttpResponseMessage> Process(Message message)
        {
            var request = message.Payload;
            return Context.Send(request).Then((response, s) => 
                Request.CreateResponse(new Message(response)))
                .Catch((ex, s) => CreateErrorResponse(ex));
        }

        [HttpPost, Route("Publish")]
        public Task<HttpResponseMessage> Publish(Message message)
        {
            var notification = message.Payload;
            return Context.Publish(notification).Then((_, s) =>
                Request.CreateResponse(new Message()))
                .Catch((ex, s) => CreateErrorResponse(ex));
        }

        private HttpResponseMessage CreateErrorResponse(Exception exception)
        {
            object error = null;
            var code = HttpStatusCode.InternalServerError;
            Context.Resolve().BestEffort().All(bundle => bundle.Add(h => 
                error = h.Proxy<IMapping>().Map(exception, typeof(Exception))
                ).Add(h => code = h.Resolve().Proxy<IMapping>()
                    .Map<HttpStatusCode>(exception)));
            if (error == null)
                error = new HttpError(exception, true);
            return Request.CreateResponse(code, new Message(error));
        }
    }
}
