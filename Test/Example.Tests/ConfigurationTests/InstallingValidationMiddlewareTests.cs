namespace Example.Tests.ConfigurationTests
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstallingValidationMiddlewareTests
    {
        [TestMethod]
        public void CanConfigureContainer()
        {
            var container = new InstallingValidationMiddleware().Container;
            Assert.IsNotNull(container);
        }
    }
}
