namespace Miruken.Mediate.Castle
{
    using System;
    using System.Collections.Generic;
    using Cache;
    using global::Castle.MicroKernel.Registration;
    using Miruken.Castle;
    using Oneway;
    using Route;
    using Schedule;

    public class MediateFeature : FeatureInstaller
    {
        private Action<ComponentRegistration> _configureMiddleware;
        private bool _standardMiddleware;

        public MediateFeature ConfigureMiddleware(Action<ComponentRegistration> configure)
        {
            _configureMiddleware += configure;
            return this;
        }

        public MediateFeature WithStandardMiddleware()
        {
            _standardMiddleware = true;
            return this;
        }

        protected override IEnumerable<FromDescriptor> GetFeatures()
        {
            yield return Types.From(
                typeof(CachedHandler), typeof(OnewayHandler),
                typeof(ScheduleHandler), typeof(PassThroughRouter));

            if (_standardMiddleware)
                yield return Types.From(
                    typeof(ValidateMiddleware<,>),
                    typeof(LogMiddleware<,>));
        }

        public override void InstallFeatures(FromDescriptor from)
        {
            var middleware = from.BasedOn(typeof(IMiddleware<,>))
                .WithServiceBase().WithServiceSelf();
            if (_configureMiddleware != null)
                middleware.Configure(_configureMiddleware);
        }
    }
}
