namespace Example.Configuration
{
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using League.Api.Team;
    using Miruken.Castle;
    using Miruken.Mediator.Castle;
    using Miruken.Validate.Castle;

    public class InstallingValidationMiddleware
    {
        public IWindsorContainer Container { get; set; }

        public InstallingValidationMiddleware()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(
                new CollectionResolver(Container.Kernel, true));

            Container.Install(
                WithFeatures.FromAssembly(typeof(CreateTeam).Assembly),
                new MediatorInstaller(),
                new ValidationInstaller());
        }
    }
}
