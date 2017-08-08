namespace Example.Tests.ConfigurationTests
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstallingMediatorTests
    {
        [TestMethod]
        public void CanConfigureContainer()
        {
            var container = new InstallingMediator().Container;
            Assert.IsNotNull(container);
        }
    }
}
