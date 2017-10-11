namespace Miruken.Mediate.Workflow
{
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;

    public class SendAllParallelReturnAttribute : WorkflowAttribute
    {
        public SendAllParallelReturnAttribute()
            : base(typeof(SendAllParallelReturn<,>))
        {          
        }
    }

    public class SendAllParallelReturn<TRequest, TResponse> 
        : WorkflowMiddleware<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer)
        {
            var messages = (result as IEnumerable)?.Cast<object>().ToArray();
            if (messages == null || messages.Length == 0) return null;
            return composer.Send(new Schedule.Parallel
            {
                Requests = messages
            });
        }
    }
}
