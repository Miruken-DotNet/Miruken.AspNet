﻿namespace Miruken.Mediate.Workflow
{
    using System.Threading.Tasks;
    using Callback;

    public class SendReturnAttribute : WorkflowAttribute
    {
        public SendReturnAttribute()
            : base(typeof(SendReturn<,>))
        {          
        }
    }

    public class SendReturn<TRequest, TResponse> 
        : WorkflowFilter<TRequest, TResponse>
    {
        protected override Task Orchestrate(TRequest request, 
            TResponse result, IHandler composer, IWorkflowConfig config)
        {
            return composer.Send(result);
        }
    }
}
