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
            var    code  = 0;
            object error = null;
            Context.All(bundle => bundle.Add(h => 
                error = h.BestEffort().Proxy<IMapping>()
                    .Map(exception, HttpRouter.ExceptionSurrogate)
                ).Add(h => code = h.BestEffort().Proxy<IMapping>()
                    .Map<int>(exception, HttpRouter.ExceptionStatusCode)));
            if (error == null)
                error = new HttpError(exception, true);
            var statusCode = code > 0
                ? (HttpStatusCode)code
                : HttpStatusCode.InternalServerError;
            return Request.CreateResponse(statusCode, new Message(error));
        }
    }
}
