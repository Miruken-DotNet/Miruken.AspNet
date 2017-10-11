namespace Miruken.Mediate.Workflow
{
    using System.Linq;
    using Schedule;

    public class PublishAllSequentialReturnAttribute : WorkflowAttribute
    {
        public PublishAllSequentialReturnAttribute()
            : base(typeof(PublishAllSequentialReturn<,>))
        {          
        }
    }

    public class PublishAllSequentialReturn<TRequest, TResponse> 
        : WorkflowManyMiddleware<TRequest, TResponse>
    {
        protected override object Combine(TRequest request, object[] results)
        {
            return new Sequential
            {
                Requests = results.Select(r => new Publish(r)).ToArray()
            };
        }
    }
}
