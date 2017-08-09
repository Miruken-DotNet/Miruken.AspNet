namespace Miruken.Mediator.Castle
{
    using System;
    using System.Collections.Generic;
    using Cache;
    using global::Castle.MicroKernel.Registration;
    using Miruken.Castle;
    using Oneway;
    using Route;
    using Schedule;

    public class MediatorInstaller : FeatureInstaller
    {
        private Action<ComponentRegistration> _configureMiddleware;
        private bool _standardMiddleware;

        public MediatorInstaller ConfigureMiddleware(Action<ComponentRegistration> configure)
        {
            _configureMiddleware += configure;
            return this;
        }

        public MediatorInstaller WithStandardMiddleware()
        {
            _standardMiddleware = true;
            return this;
        }

        protected override IEnumerable<FromDescriptor> GetFeatures()
        {
            yield return Types.From(
                typeof(CachedHandler), typeof(OnewayHandler),
                typeof(RouteHandler),  typeof(ScheduleHandler));

            if (_standardMiddleware)
                yield return Types.From(
                    typeof(ValidationMiddleware<,>),
                    typeof(LoggingMiddleware<,>));
        }

        public override void InstallFeatures(FromDescriptor from)
        {
            from.BasedOn<IRouter>().WithServiceBase();
            var middleware = from.BasedOn(typeof(IMiddleware<,>))
                .WithServiceBase().WithServiceSelf();
            if (_configureMiddleware != null)
                middleware.Configure(_configureMiddleware);
        }
    }
}
