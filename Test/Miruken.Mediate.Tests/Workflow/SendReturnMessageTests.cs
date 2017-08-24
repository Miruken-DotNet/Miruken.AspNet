namespace Miruken.Mediate.Tests.Workflow
{
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Mediate.Workflow;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SendReturnMessageTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<Saga>();
            _handler = new Saga();
        }

        [TestMethod]
        public async Task Should_Send_Return_As_Message()
        {
            var result = await _handler.Send(new StepOne());
        }

        private class StepOne {}
        private class StepTwo {}
        private class StepThree {}

        [Pipeline]
        private class Saga : Handler
        {
            [Mediates,
             SendReturnMessage]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }
        }
    }
}
