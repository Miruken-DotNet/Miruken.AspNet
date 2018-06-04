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
                        + new FilterProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(StepTwo));
        }

        [TestMethod]
        public async Task Should_Join_Return()
        {
            var saga    = new SagaJoin();
            var handler = saga + new FilterProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(StepTwo));
            Assert.IsTrue(saga.Complete);
        }

        [TestMethod]
        public async Task Should_Send_All_Return()
        {
            var handler = new SagaAll()
                        + new Scheduler()
                        + new FilterProvider();
            var result  = await handler.Send(new StepOne());
            Assert.IsInstanceOfType(result, typeof(IEnumerable));
        }

        [TestMethod]
        public async Task Should_Join_All_Return()
        {
            var saga    = new SagaAllJoin();
            var handler = saga
                        + new Scheduler()
                        + new FilterProvider();
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
                        + new FilterProvider();
            await handler.Send(new StepOne());
        }

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Reject_Join_All_Missing_Step()
        {
            var saga    = new SagaAllMissingJoin();
            var handler = saga
                        + new Scheduler()
                        + new FilterProvider();
            await handler.Send(new StepOne());
        }

        public class StepOne {}
        public class StepTwo {}
        public class StepThree {}

        public class Saga : Handler
        {
            public bool Complete { get; private set; }

            [Handles,
             SendReturn]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }

            [Handles,
             SendReturn]
            public async Task<StepThree> Do(StepTwo stepTwo)
            {
                await Task.Delay(10);
                return new StepThree();
            }

            [Handles]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaJoin : Handler
        {
            public bool Complete { get; private set; }

            [Handles,
             SendReturn(Join = true)]
            public StepTwo Do(StepOne stepOne)
            {
                return new StepTwo();
            }

            [Handles,
             SendReturn(Join = true)]
            public async Task<StepThree> Do(StepTwo stepTwo)
            {
                await Task.Delay(10);
                return new StepThree();
            }

            [Handles]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaAll : Handler
        {
            public bool Complete { get; private set; }

            [Handles,
             SendAllReturn]
            public IEnumerable Do(StepOne stepOne)
            {
                yield return new StepTwo();
                yield return new StepThree();
            }

            [Handles]
            public void Do(StepTwo stepTwo)
            {
            }

            [Handles]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaAllJoin : Handler
        {
            public bool Complete { get; private set; }

            [Handles,
             SendAllReturn(Join = true)]
            public IEnumerable Do(StepOne stepOne)
            {
                yield return new StepTwo();
                yield return new StepThree();
            }

            [Handles]
            public void Do(StepTwo stepTwo)
            {
            }

            [Handles]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class SagaAllMissingJoin : Handler
        {
            public bool Complete { get; private set; }

            [Handles,
             SendAllReturn(Join = true)]
            public IEnumerable Do(StepOne stepOne)
            {
                yield return new StepTwo();
                yield return new StepThree();
            }

            [Handles]
            public Promise Do(StepThree stepThree)
            {
                Complete = true;
                return Promise.Empty;
            }
        }

        public class FilterProvider : Handler
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
