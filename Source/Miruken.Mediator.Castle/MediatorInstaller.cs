namespace Miruken.Mediator.Castle
{
    using System;
    using global::Castle.MicroKernel.Registration;
    using Miruken.Castle;
    using Route;

    public class MediatorInstaller : FeatureInstaller
    {
        private Action<ComponentRegistration> _configureMiddleware;

        public MediatorInstaller ConfigureMiddleware(Action<ComponentRegistration> configure)
        {
            _configureMiddleware += configure;
            return this;
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
