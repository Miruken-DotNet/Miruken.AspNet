namespace Miruken.AspNet.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Controllers;
    using global::Castle.MicroKernel.Registration;
    using Http;
    using Miruken.Castle;

    public class AspNetFeature : FeatureInstaller
    {
        private Action<ComponentRegistration> _configure;

        public AspNetFeature ConfigureControllers(Action<ComponentRegistration> configure)
        {
            _configure += configure;
            return this;
        }

        protected override IEnumerable<FromDescriptor> GetFeatures()
        {
            yield return Classes.FromAssemblyContaining<HttpRouter>();
            yield return Classes.FromAssemblyContaining<HttpRouteController>();
        }

        public override void InstallFeatures(FromDescriptor from)
        {
            var controllers = from
                .BasedOn(typeof(IHttpController))
                .LifestyleCustom<ContextualLifestyleManager>()
                .WithServiceSelf();
            if (_configure != null)
                controllers.Configure(_configure);
        }
    }
}
