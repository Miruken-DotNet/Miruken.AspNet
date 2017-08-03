namespace Miruken.Mediator.Castle
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.MicroKernel.SubSystems.Configuration;
    using Infrastructure;
    using Miruken.Castle;
    using Route;

    public class MediatorInstaller : FeatureInstaller
    {
        private Action<ComponentRegistration> _configureMiddleware;
        private Type[] _middleware;

        public MediatorInstaller()
            : base(typeof(IMiddleware<,>).Assembly)
        {         
        }

        public MediatorInstaller StandardMiddleware(params Type[] middleware)
        {
            if (middleware
                .Any(type => type.GetOpenTypeConformance(typeof(IMiddleware<,>)) == null))
                throw new ArgumentException("One or more types do not represent middleware");
            _middleware = middleware;
            return this;
        }

        public MediatorInstaller ConfigureMiddleware(Action<ComponentRegistration> configure)
        {
            _configureMiddleware += configure;
            return this;
        }

        protected override void Install(IConfigurationStore store)
        {
            base.Install(store);
            if (_middleware != null)
            {
                Predicate<Type> filter = null;
                if (_middleware.Length != 0)
                    filter = type => 
                        type.GetOpenTypeConformance(typeof(IMiddleware<,>)) == null ||
                        _middleware.Contains(type);
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
            var compoments = Classes.FromAssembly(assembly)
                .BasedOn(typeof(IMiddleware<,>))
                .OrBasedOn(typeof(IRouter))
                .WithServiceBase().WithServiceSelf();
            if (filter != null)
                compoments = compoments.If(filter);
            if (_configureMiddleware != null)
                compoments.Configure(component =>
                {
                    if (component.Implementation.GetOpenTypeConformance(typeof(IMiddleware<,>)) != null)
                        compoments.Configure(_configureMiddleware);
                });
            Container.Register(compoments);
        }
    }
}
