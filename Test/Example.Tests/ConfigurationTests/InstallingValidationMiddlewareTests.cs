namespace Example.Tests.ConfigurationTests
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;

    [TestClass]
    public class InstallingValidationMiddlewareTests
    {
        [TestMethod]
        public void CanConfigureContainer()
        {
            var container = new InstallingValidationMiddleware().Container;
            Assert.IsNotNull(container);
            var featureAssemblies = container.ResolveAll<FeatureAssembly>();

            Assert.AreEqual(3, featureAssemblies.Length);
        }
    }
}
