namespace Miruken.Mediator
{
    using System;
    using Callback;
    using Infrastructure;

    public class PipelineAttribute : FilterAttribute
    {
        public PipelineAttribute()
            : base(typeof(IMiddleware<,>))
        {         
        }

        public PipelineAttribute(params Type[] middleware)
            : base(middleware)
        {            
        }

        protected override void VerifyFilterType(Type filterType)
        {
            var conformance = filterType.GetOpenTypeConformance(typeof(IMiddleware<,>));
            if (conformance == null)
                throw new ArgumentException($"{filterType.FullName} does not conform to IMiddleware<,>");
            base.VerifyFilterType(filterType);
        }
    }
}