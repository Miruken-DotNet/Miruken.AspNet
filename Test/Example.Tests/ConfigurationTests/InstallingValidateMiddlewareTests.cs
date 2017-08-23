namespace Example.Tests.ConfigurationTests
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstallingValidateMiddlewareTests
    {
        [TestMethod]
        public void CanConfigureContainer()
        {
            var container = new InstallingValidateMiddleware().Container;
            Assert.IsNotNull(container);
        }
    }
}
