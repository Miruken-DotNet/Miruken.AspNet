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
            Many = true;
        }

        public PipelineAttribute(params Type[] middleware)
            : base(middleware)
        {            
        }

        protected override void VerifyFilterType(Type middlewareType)
        {
            var conformance = middlewareType.GetOpenTypeConformance(typeof(IMiddleware<,>));
            if (conformance == null)
                throw new ArgumentException($"{middlewareType.FullName} does not conform to IMiddleware<,>");
            base.VerifyFilterType(middlewareType);
        }
    }
}