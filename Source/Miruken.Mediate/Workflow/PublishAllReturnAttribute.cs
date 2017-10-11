namespace Miruken.Mediate.Workflow
{
    using System.Linq;
    using Schedule;

    public class PublishAllReturnAttribute : WorkflowAttribute
    {
        public PublishAllReturnAttribute()
            : base(typeof(PublishAllReturn<,>))
        {          
        }
    }

    public class PublishAllReturn<TRequest, TResponse> 
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override object Combine(TRequest request, object[] results)
        {
            return new Concurrent
            {
                Requests = results.Select(r => new Publish(r)).ToArray()
            };
        }
    }
}
