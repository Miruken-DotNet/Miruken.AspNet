namespace Miruken.AspNet
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Callback;
    using Http;
    using Map;
    using Mediate;

    [HttpRoute]
    public class HttpRouteController : ContextualApiController
    {
        [HttpPost, Route("Process/{*rest}")]
        public Task<HttpResponseMessage> Process(Message message, string rest)
        {
            var request = message?.Payload;
            if (request == null)
            {
                return Task.FromResult(
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "A request is required to process."));
            }
            Context.Store(User ?? Thread.CurrentPrincipal);
            return Context.Send(request).Then((response, s) => 
                Request.CreateResponse(new Message(response)))
                .Catch((ex, s) => CreateErrorResponse(ex));
        }

        [HttpPost, Route("Publish/{*rest}")]
        public Task<HttpResponseMessage> Publish(Message message, string rest)
        {
            var notification = message?.Payload;
            if (notification == null)
            {
                return Task.FromResult(
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "A notification is required to publish"));
            }
            Context.Store(User ?? Thread.CurrentPrincipal);
            return Context.Publish(notification).Then((_, s) =>
                Request.CreateResponse(new Message()))
                .Catch((ex, s) => CreateErrorResponse(ex));
        }

        private HttpResponseMessage CreateErrorResponse(Exception exception)
        {
            object error = null;
            var code = HttpStatusCode.InternalServerError;
            Context.Resolve().BestEffort().All(bundle => bundle
                .Add(h => error = h.Proxy<IMapping>()
                    .Map<object>(exception, typeof(Exception)))
                .Add(h => code = h.Proxy<IMapping>()
                    .Map<HttpStatusCode>(exception)));
            if (error == null)
                error = new HttpError(exception, true);
            return Request.CreateResponse(code, new Message(error));
        }
    }
}
