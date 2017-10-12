namespace Miruken.Mediate.Tests.Workflow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Callback;
    using Concurrency;
    using Mediate.Schedule;
    using Mediate.Workflow;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PublishReturnTests
    {
        [TestMethod]
        public async Task Should_Publish_Return()
        {
            var store1   = new EventStore();
            var store2   = new EventStore();
            var handler  = store1 + store2
                         + new MiddlewareProvider();
            var eventOne = new EventOne();
            await handler.Publish(eventOne);
            CollectionAssert.Contains(store1.Events, eventOne);
            CollectionAssert.Contains(store2.Events, eventOne);
        }

        [TestMethod]
        public async Task Should_Join_Return()
        {
            var store1  = new EventStoreJoin();
            var store2  = new EventStoreJoin();
            var handler = store1 + store2
                        + new MiddlewareProvider();
            await handler.Publish(new EventOne());
            Assert.AreEqual(7, store1.Events.Count);
            CollectionAssert.AreEquivalent(store1.Events, store2.Events);
        }

        [TestMethod]
        public async Task Should_Publish_All_Return()
        {
            var store1   = new EventStoreAll();
            var store2   = new EventStoreAll();
            var handler  = store1 + store2 
                         + new Scheduler()
                         + new MiddlewareProvider();
            var eventOne = new EventOne();
            await handler.Publish(eventOne);
            CollectionAssert.Contains(store1.Events, eventOne);
            CollectionAssert.Contains(store2.Events, eventOne);
        }

        [TestMethod]
        public async Task Should_Join_All_Return()
        {
            var store1  = new EventStoreAllJoin();
            var store2  = new EventStoreAllJoin();
            var handler = store1 + store2
                        + new Scheduler()
                        + new MiddlewareProvider();
            await handler.Publish(new EventOne());
            Assert.AreEqual(5, store1.Events.Count);
            CollectionAssert.AreEquivalent(store1.Events, store2.Events);
        }

        [TestMethod,
         ExpectedException(typeof(NotSupportedException))]
        public async Task Should_Reject_Join_All_Without_Scheduler()
        {
            var store   = new EventStoreAllJoin();
            var handler = store
                        + new MiddlewareProvider();
            await handler.Publish(new EventOne());
        }

        public class EventOne {}
        public class EventTwo {}
        public class EventThree {}

        public class EventStore : PipelineHandler
        {
            public List<object> Events { get; } = new List<object>();

            [Mediates,
             PublishReturn]
            public EventTwo Consume(EventOne eventOne)
            {
                Events.Add(eventOne);
                return new EventTwo();
            }

            [Mediates,
             PublishReturn]
            public async Task<EventThree> Consume(EventTwo eventTwo)
            {
                await Task.Delay(10);
                Events.Add(eventTwo);
                return new EventThree();
            }

            [Mediates]
            public Promise Consume(EventThree eventThree)
            {
                Events.Add(eventThree);
                return Promise.Empty;
            }
        }

        public class EventStoreJoin : PipelineHandler
        {
            public List<object> Events { get; } = new List<object>();

            [Mediates,
             PublishReturn(Join = true)]
            public EventTwo Consume(EventOne eventOne)
            {
                Events.Add(eventOne);
                return new EventTwo();
            }

            [Mediates,
             PublishReturn(Join = true)]
            public async Task<EventThree> Consume(EventTwo eventTwo)
            {
                await Task.Delay(10);
                Events.Add(eventTwo);
                return new EventThree();
            }

            [Mediates]
            public Promise Consume(EventThree eventThree)
            {
                Events.Add(eventThree);
                return Promise.Empty;
            }
        }

        public class EventStoreAll : PipelineHandler
        {
            public List<object> Events { get; } = new List<object>();

            [Mediates,
             PublishAllReturn]
            public IEnumerable Consume(EventOne eventOne)
            {
                Events.Add(eventOne);
                yield return new EventTwo();
                yield return new EventThree();
            }

            [Mediates]
            public void Consume(EventTwo eventTwo)
            {
                Events.Add(eventTwo);
            }

            [Mediates]
            public Promise Consume(EventThree eventThree)
            {
                Events.Add(eventThree);
                return Promise.Empty;
            }
        }

        public class EventStoreAllJoin : PipelineHandler
        {
            public List<object> Events { get; } = new List<object>();

            [Mediates,
             PublishAllReturn(Join = true)]
            public IEnumerable Consume(EventOne eventOne)
            {
                Events.Add(eventOne);
                yield return new EventTwo();
                yield return new EventThree();
            }

            [Mediates]
            public void Consume(EventTwo eventTwo)
            {
                Events.Add(eventTwo);
            }

            [Mediates]
            public Promise Consume(EventThree eventThree)
            {
                Events.Add(eventThree);
                return Promise.Empty;
            }
        }

        public class EventStoreAllMissingJoin : PipelineHandler
        {
            public List<object> Events { get; } = new List<object>();

            [Mediates,
             PublishAllReturn(Join = true)]
            public IEnumerable Consume(EventOne eventOne)
            {
                Events.Add(eventOne);
                yield return new EventTwo();
                yield return new EventThree();
            }

            [Mediates]
            public Promise Consume(EventThree eventThree)
            {
                Events.Add(eventThree);
                return Promise.Empty;
            }
        }

        public class MiddlewareProvider : Handler
        {
            [Provides]
            public PublishReturn<TEvent, TRes> GetPublishReturn<TEvent, TRes>()
            {
                return new PublishReturn<TEvent, TRes>();
            }

            [Provides]
            public PublishAllReturn<TEvent, TRes> GetPublishAllReturn<TEvent, TRes>()
            {
                return new PublishAllReturn<TEvent, TRes>();
            }
        }
    }
}
