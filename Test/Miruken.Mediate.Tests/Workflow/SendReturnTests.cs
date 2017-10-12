namespace Miruken.Mediate.Tests.Workflow
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Callback;
    using Concurrency;
    using Mediate.Schedule;
    using Mediate.Workflow;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SendReturnTests
    {
        [TestMethod]
        public async Task Should_Send_Return()
        {
            var handler = new Saga() 
                        + new MiddlewareProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(StepTwo));
        }

        [TestMethod]
        public async Task Should_Join_Return()
        {
            var saga    = new SagaJoin();
            var handler = saga + new MiddlewareProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(StepTwo));
            Assert.IsTrue(saga.Complete);
        }

        [TestMethod]
        public async Task Should_Send_All_Return()
        {
            var handler = new SagaAll()
                        + new Scheduler()
                        + new MiddlewareProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(IEnumerable));
        }

        [TestMethod]
        public async Task Should_Join_All_Return()
        {
            var saga    = new SagaAllJoin();
            var handler = saga
                        + new Scheduler()
                        + new MiddlewareProvider();
            var result = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(IEnumerable));
            Assert.IsTrue(saga.Complete);
        }

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Reject_Join_All_Without_Scheduler()
        {
            var saga    = new SagaAllJoin();
            var handler = saga
                        + new MiddlewareProvider();
            await handler.Send(new StepOne());
        }

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Reject_Join_All_Missing_Step()
        {
            var saga    = new SagaAllMissingJoin();
            var handler = saga
                        + new Scheduler()
                        + new MiddlewareProvider();
            await handler.Send(new StepOne());
        }

        public class StepOne {}
        public class StepTwo {}
        public class StepThree {}

        public class Saga : PipelineHandler
        {
            public bool Complete { get; private set; }

            [Mediates,
             SendReturn]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }

            [Mediates,
             SendReturn]
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
             SendReturn(Join = true)]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }

            [Mediates,
             SendReturn(Join = true)]
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

        public class SagaAll : PipelineHandler
        {
            public bool Complete { get; private set; }

            [Mediates,
             SendAllReturn]
            public IEnumerable Do(StepOne stepOne)
            {
                yield return new StepTwo();
                yield return new StepThree();
            }

            [Mediates]
            public void Do(StepTwo stepTwo)
            {
            }

            [Mediates]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaAllJoin : PipelineHandler
        {
            public bool Complete { get; private set; }

            [Mediates,
             SendAllReturn(Join = true)]
            public IEnumerable Do(StepOne stepOne)
            {
                yield return new StepTwo();
                yield return new StepThree();
            }

            [Mediates]
            public void Do(StepTwo stepTwo)
            {
            }

            [Mediates]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaAllMissingJoin : PipelineHandler
        {
            public bool Complete { get; private set; }

            [Mediates,
             SendAllReturn(Join = true)]
            public IEnumerable Do(StepOne stepOne)
            {
                yield return new StepTwo();
                yield return new StepThree();
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
            public SendReturn<TReq, TResp> GetSendReturn<TReq, TResp>()
            {
                return new SendReturn<TReq, TResp>();
            }

            [Provides]
            public SendAllReturn<TReq, TResp> GetSendAllReturn<TReq, TResp>()
            {
                return new SendAllReturn<TReq, TResp>();
            }
        }
    }
}
