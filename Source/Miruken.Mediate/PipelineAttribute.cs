namespace Miruken.Mediate
{
    using System;
    using Callback;
    using Callback.Policy;
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

        protected override bool AllowFilterType(Type filterType, MethodBinding binding)
        {
            var policyBinding = binding as PolicyMethodBinding;
            return policyBinding?.Definition is MediatesAttribute;
        }

        protected override void ValidateFilterType(Type middlewareType)
        {
            var conformance = middlewareType.GetOpenTypeConformance(typeof(IMiddleware<,>));
            if (conformance == null)
                throw new ArgumentException($"{middlewareType.FullName} does not conform to IMiddleware<,>");
            base.ValidateFilterType(middlewareType);
        }
    }
}