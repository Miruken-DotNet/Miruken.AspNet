namespace Miruken.AspNet.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Mvc;
    using Context;
    using global::Castle.Core.Internal;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.MicroKernel.SubSystems.Configuration;
    using Http;
    using Miruken.Castle;

    public class AspNetFeature : FeatureInstaller
    {
        private readonly IContext _context;
        private HttpApplication _application;
        private HttpConfiguration _configuration;
        private Action<ComponentRegistration> _configure;

        public AspNetFeature(IContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public AspNetFeature WithMvc(HttpApplication application)
        {
            _application = application;
            return this;
        }

        public AspNetFeature WithWebApi(HttpConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }

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

        protected override void Install(IConfigurationStore store)
        {
            _application?.UseMiruken(_context);
            _configuration?.UseMiruken(_context);
        }

        public override void InstallFeatures(FromDescriptor from)
        {
            if (_application != null)
            {
                var controllers = from
                    .BasedOn(typeof(IController))
                    .If(controller => controller.Is<IContextual>())
                    .LifestyleCustom<ContextualLifestyleManager>()
                    .WithServiceSelf();
                if (_configure != null)
                    controllers.Configure(_configure);
            }

            if (_configuration != null)
            {
                var controllers = from
                    .BasedOn(typeof(IHttpController))
                    .If(controller => controller.Is<IContextual>())
                    .LifestyleCustom<ContextualLifestyleManager>()
                    .WithServiceSelf();
                if (_configure != null)
                    controllers.Configure(_configure);
            }     
        }
    }
}
