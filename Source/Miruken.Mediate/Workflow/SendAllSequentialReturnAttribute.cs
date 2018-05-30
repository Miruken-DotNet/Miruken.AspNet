﻿namespace Miruken.Mediate.Workflow
{
    using Schedule;

    public class SendAllSequentialReturnAttribute : WorkflowAttribute
    {
        public SendAllSequentialReturnAttribute()
            : base(typeof(SendAllSequentialReturn<,>))
        {          
        }
    }

    public class SendAllSequentialReturn<TRequest, TResponse> 
        : WorkflowManyFilter<TRequest, TResponse>
    {
        protected override Scheduled Combine(TRequest request, object[] results)
        {
            return new Sequential
            {
                Requests = results
            };
        }
    }
}
