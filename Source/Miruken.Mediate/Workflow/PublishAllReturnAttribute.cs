namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class PublishAllReturnAttribute : WorkflowAttribute
    {
        public PublishAllReturnAttribute()
            : base(typeof(PublishAllReturn<,>))
        {          
        }
    }

    public class PublishAllReturn<TRequest, TResponse> 
        : WorkflowManyFilter<TRequest, TResponse>
    {
        protected override Scheduled Combine(TRequest request, object[] results)
        {
            return new Concurrent
            {
                Requests = Published(results)
            };
        }
    }
}
