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
    using Validate;

    [HttpRoute]
    public class HttpRouteController : ContextualApiController
    {
        [HttpPost,
         Route("process/{*rest}", Name = "Process"),
         Route("tag/{client}/{*args}", Name = "ProcessTag")]
        public Task<HttpResponseMessage> Process([FromBody]Message message)
        {
            if (!ModelState.IsValid)
                return Task.FromResult(CreateInvalidRequestResponse());

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

        [HttpPost, Route("publish/{*rest}", Name = "Publish")]
        public Task<HttpResponseMessage> Publish([FromBody]Message message)
        {
            if (!ModelState.IsValid)
                return Task.FromResult(CreateInvalidRequestResponse());

            var notification = message?.Payload;
            if (notification == null)
            {
                return Task.FromResult(
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "A notification is required to publish."));
            }

            Context.Store(User ?? Thread.CurrentPrincipal);
            return Context.Publish(notification).Then((_, s) =>
                Request.CreateResponse(new Message()))
                .Catch((ex, s) => CreateErrorResponse(ex));
        }

        private HttpResponseMessage CreateErrorResponse(
            Exception exception, HttpStatusCode? code = null)
        {
            object error = null;
            Context.Infer().BestEffort().All(bundle =>
            {
                bundle.Add(h => error = h.Map<object>(exception, typeof(Exception)));
                if (!code.HasValue)
                    bundle.Add(h => code = h.Map<HttpStatusCode>(exception));
            });
            if (error == null)
                error = new HttpError(exception, true);
            return Request.CreateResponse(
                code ?? HttpStatusCode.InternalServerError,
                new Message(error));
        }

        private HttpResponseMessage CreateInvalidRequestResponse()
        {
            var outcome = new ValidationOutcome();
            foreach (var property in ModelState)
            foreach (var error in property.Value.Errors)
            {
                outcome.AddError(property.Key,
                    error.Exception?.Message ?? error.ErrorMessage);
            }
            return CreateErrorResponse(
                new ValidationException(outcome),
                HttpStatusCode.BadRequest);
        }
    }
}
