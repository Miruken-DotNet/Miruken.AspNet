namespace Miruken.Mediator.Castle
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.MicroKernel.SubSystems.Configuration;
    using Infrastructure;
    using Miruken.Castle;

    public class MiddlewareInstaller : FeatureInstaller
    {
        private Action<ComponentRegistration> _configure;
        private Type[] _middleware;

        public MiddlewareInstaller()
            : base(typeof(IMiddleware<,>).Assembly)
        {         
        }

        public MiddlewareInstaller StandardMiddleware(params Type[] middleware)
        {
            if (middleware
                .Any(type => type.GetOpenTypeConformance(typeof(IMiddleware<,>)) == null))
                throw new ArgumentException("One or more types do not represent middleware");
            _middleware = middleware;
            return this;
        }

        public MiddlewareInstaller ConfigureMiddleware(Action<ComponentRegistration> configure)
        {
            _configure += configure;
            return this;
        }

        protected override void Install(IConfigurationStore store)
        {
            base.Install(store);
            if (_middleware != null)
            {
                Predicate<Type> filter = null;
                if (_middleware.Length != 0)
                    filter = type => _middleware.Contains(type);
                InstallAssembly(Assembly.GetExecutingAssembly(), filter);
                InstallAssembly(typeof(IMiddleware<,>).Assembly, filter);
            }
        }

        protected override void InstallFeature(Assembly assembly)
        {
            InstallAssembly(assembly);
        }

        private void InstallAssembly(Assembly assembly, Predicate<Type> filter = null)
        {
            var middleware = Classes.FromAssembly(assembly)
                .BasedOn(typeof(IMiddleware<,>))
                .WithServiceBase().WithServiceSelf();
            if (filter != null)
                middleware = middleware.If(filter);
            if (_configure != null)
                middleware.Configure(_configure);
            Container.Register(middleware);
        }
    }
}
