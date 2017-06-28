namespace Example.Tests.ConfigurationTests
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Castle;

    [TestClass]
    public class InstallingMediatorTests
    {
        [TestMethod]
        public void CanConfigureContainer()
        {
            var container = new InstallingMediator().Container;
            Assert.IsNotNull(container);
            var featureAssemblies = container.ResolveAll<FeatureAssembly>();

            Assert.AreEqual(1, featureAssemblies.Length);
        }
    }
}
