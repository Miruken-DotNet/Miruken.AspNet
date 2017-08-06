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
        private Predicate<Type> _filterMiddleware;

        public MediatorInstaller()
            : base(typeof(IMiddleware<,>).Assembly)
        {         
        }

        public MediatorInstaller StandardMiddleware(params Type[] middleware)
        {
            if (middleware.Length == 0)
            {
                _filterMiddleware = _ => true;
                return this;
            }
            if (middleware.Any(type => 
                type.GetOpenTypeConformance(typeof(IMiddleware<,>)) == null))
                throw new ArgumentException("One or more types do not represent middleware");
            middleware = (Type[])middleware.Clone();
            _filterMiddleware = type => middleware.Contains(type);
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
            Container.Install(
                WithStandardFeature(Assembly.GetExecutingAssembly()),
                WithStandardFeature(typeof(IMiddleware<,>).Assembly)
            );
        }

        protected override void InstallFeature(FeatureAssembly feature)
        {
            var compoments = Classes.FromAssembly(feature.Assembly);
            compoments.BasedOn<IRouter>().WithServiceBase();
            var middleware = compoments.BasedOn(typeof(IMiddleware<,>))
                .WithServiceBase().WithServiceSelf();
            if (feature.Filter != null)
                middleware = middleware.If(feature.Filter);
            if (_configureMiddleware != null)
                middleware.Configure(_configureMiddleware);
            Container.Register(compoments);
        }

        private FeatureAssembly WithStandardFeature(Assembly assembly)
        {
            return WithFeatures.FromAssembly(assembly, feature =>
            {
                feature.Where(type =>
                    type.GetOpenTypeConformance(typeof(IMiddleware<,>)) == null || 
                    _filterMiddleware?.Invoke(type) == true);
                feature.SkipInstallers();
            });
        }
    }
}
