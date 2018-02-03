﻿namespace Example.Configuration
{
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using Miruken.Castle;
    using Miruken.Mediate.Castle;

    public class InstallingMediator
    {
        public IWindsorContainer Container { get; set; }

        public InstallingMediator()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(
                new CollectionResolver(Container.Kernel, true));

            Container.Install(new FeaturesInstaller(
                new MediateFeature().WithStandardMiddleware()));
        }
    }
}