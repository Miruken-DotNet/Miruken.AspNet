namespace Example.Tests.ConfigurationTests
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstallingValidateFilterTests
    {
        [TestMethod]
        public void CanConfigureContainer()
        {
            var container = new InstallingValidateFilter().Container;
            Assert.IsNotNull(container);
        }
    }
}
