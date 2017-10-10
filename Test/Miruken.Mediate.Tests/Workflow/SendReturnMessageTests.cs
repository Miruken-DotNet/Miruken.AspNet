﻿namespace Miruken.Mediate.Tests.Workflow
{
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using Mediate.Workflow;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SendReturnMessageTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.GetDescriptor<Saga>();
        }

        [TestMethod]
        public async Task Should_Send_Return_As_Message()
        {
            var saga    = new Saga();
            var handler = saga + new MiddlewareProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(StepTwo));
            Assert.IsFalse(saga.Complete);
        }

        [TestMethod]
        public async Task Should_Join_Return_As_Message()
        {
            var saga    = new SagaJoin();
            var handler = saga + new MiddlewareProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(StepTwo));
            Assert.IsTrue(saga.Complete);
        }

        public class StepOne {}
        public class StepTwo {}
        public class StepThree {}

        public class Saga : PipelineHandler
        {
            public bool Complete { get; private set; }

            [Mediates,
             SendReturnMessage]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }

            [Mediates,
             SendReturnMessage]
            public async Task<StepThree> Do(StepTwo stepTwo)
            {
                await Task.Delay(10);
                return new StepThree();
            }

            [Mediates]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaJoin : PipelineHandler
        {
            public bool Complete { get; private set; }

            [Mediates,
             SendReturnMessage(Join = true)]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }

            [Mediates,
             SendReturnMessage(Join = true)]
            public async Task<StepThree> Do(StepTwo stepTwo)
            {
                await Task.Delay(10);
                return new StepThree();
            }

            [Mediates]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class MiddlewareProvider : Handler
        {
            [Provides]
            public SendReturnMessage<TReq, TResp> GetMiddleware<TReq, TResp>()
            {
                return new SendReturnMessage<TReq, TResp>();
            }
        }
    }
}
