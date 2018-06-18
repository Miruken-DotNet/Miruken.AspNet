namespace Miruken.AspNet.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Controllers;
    using System.Web.Mvc;
    using Context;
    using global::Castle.Core.Internal;
    using global::Castle.MicroKernel.Registration;
    using Http;
    using Mediate.Schedule;
    using Miruken.Castle;

    public class AspNetFeature : FeatureInstaller
    {
        private Action<ComponentRegistration> _configureMvc;
        private Action<ComponentRegistration> _configureApi;

        public AspNetFeature ConfigureMvc(Action<ComponentRegistration> configure)
        {
            _configureApi += configure;
            return this;
        }

        public AspNetFeature ConfigureApi(Action<ComponentRegistration> configure)
        {
            _configureMvc += configure;
            return this;
        }

        public override IEnumerable<FromDescriptor> GetFeatures()
        {
            yield return Classes.FromAssemblyContaining<HttpRouteController>();
            yield return Classes.FromAssemblyContaining<HttpRouter>();
            yield return Classes.FromAssemblyContaining<Scheduler>();
        }

        public override void InstallFeatures(FromDescriptor from)
        {
            var mvcControllers = from
                .BasedOn(typeof(IController))
                .If(controller => controller.Is<IContextual>())
                .LifestyleCustom<ContextualLifestyleManager>()
                .WithServiceSelf();
            if (_configureMvc != null)
                mvcControllers.Configure(_configureMvc);

            var apiControllers = from
                .BasedOn(typeof(IHttpController))
                .If(controller => controller.Is<IContextual>())
                .LifestyleCustom<ContextualLifestyleManager>()
                .WithServiceSelf();
            if (_configureApi != null)
                apiControllers.Configure(_configureApi);
        }
    }
}
