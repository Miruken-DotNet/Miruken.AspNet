namespace Miruken.Mediate.Workflow
{
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Schedule;

    public class PublishAllParallelReturnAttribute : WorkflowAttribute
    {
        public PublishAllParallelReturnAttribute()
            : base(typeof(PublishAllParallelReturn<,>))
        {          
        }
    }

    public class PublishAllParallelReturn<TRequest, TResponse> 
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer)
        {
            var messages = (result as IEnumerable)?.Cast<object>().ToArray();
            if (messages == null || messages.Length == 0) return null;
            return composer.Send(new Schedule.Parallel
            {
                Requests = messages.Select(m => new Publish(m)).ToArray()
            });
        }
    }
}
