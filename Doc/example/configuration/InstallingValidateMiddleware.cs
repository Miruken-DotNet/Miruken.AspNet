namespace Example.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using League.Api.Team;
    using Miruken.Castle;
    using Miruken.Mediate.Castle;
    using Miruken.Validate.Castle;

    public class InstallingValidateMiddleware
    {
        public IWindsorContainer Container { get; set; }

        public InstallingValidateMiddleware()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(
                new CollectionResolver(Container.Kernel, true));

            Container.Install(new FeaturesInstaller(
                new MediateFeature().WithStandardMiddleware(),
                new ValidationFeature()).Use(
                    Classes.FromAssemblyContaining<CreateTeam>()));
        }
    }
}
