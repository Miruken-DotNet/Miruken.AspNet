namespace Miruken.Mediator.Castle
{
    using System;
    using global::Castle.MicroKernel.Registration;
    using Miruken.Castle;

    public class MediatorInstaller : PluginInstaller
    {
        private Action<ComponentRegistration> _configure;

        public MediatorInstaller ConfigureMiddleware(Action<ComponentRegistration> configure)
        {
            _configure += configure;
            return this;
        }

        protected override void InstallPlugin(Plugin plugin)
        {
            var middleware = Classes.FromAssembly(plugin.Assembly)
                .BasedOn(typeof(IMiddleware<,>))
                .WithServiceBase().WithServiceSelf();
            if (_configure != null)
                middleware.Configure(_configure);
            Container.Register(middleware);
        }
    }
}
