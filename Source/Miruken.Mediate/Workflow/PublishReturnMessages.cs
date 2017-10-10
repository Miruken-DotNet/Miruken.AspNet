namespace Miruken.Mediate.Workflow
{
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Schedule;

    public class PublishReturnMessages : WorkflowAttribute
    {
        public PublishReturnMessages()
            : base(typeof(PublishReturnMessages<,>))
        {          
        }
    }

    public class PublishReturnMessages<TRequest, TResponse> 
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer)
        {
            var messages = (result as IEnumerable)?.Cast<object>().ToArray();
            if (messages == null || messages.Length == 0) return null;
            return composer.Send(new Concurrent
            {
                Requests = messages.Select(m => new Publish(m)).ToArray()
            });
        }
    }
}
