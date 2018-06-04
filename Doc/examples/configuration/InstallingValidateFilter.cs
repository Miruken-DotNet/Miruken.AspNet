namespace Example.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using League.Api.Team;
    using Miruken.Castle;
    using Miruken.Validate.Castle;

    public class InstallingValidateFilter
    {
        public IWindsorContainer Container { get; set; }

        public InstallingValidateFilter()
        {
            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(
                new CollectionResolver(Container.Kernel, true));

            Container.Install(new FeaturesInstaller(
                new ValidateFeature()).Use(
                    Classes.FromAssemblyContaining<CreateTeam>()));
        }
    }
}
