﻿namespace Example.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using League.Api.Team;
    using Miruken.Castle;
    using Miruken.Mediator.Castle;

    public class InstallingMediator
    {
        public IWindsorContainer Container { get; set; }

        public InstallingMediator()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(
                new CollectionResolver(Container.Kernel, true));

            Container.Install(
                new MediatorInstaller(),
                WithFeatures.From(Classes.FromAssemblyContaining<CreateTeam>()));
        }
    }
}
