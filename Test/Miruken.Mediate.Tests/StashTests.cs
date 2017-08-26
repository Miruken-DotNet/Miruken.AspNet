namespace Miruken.Mediate.Tests
{
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Validate;
    using Validate.FluentValidation;

    [TestClass]
    public class StashTests
    {
        public enum OrderStatus
        {
            Created,
            Cancelled
        }

        public class Order
        {
            public int         Id     { get; set; }
            public OrderStatus Status { get; set; }
        }

        public class CancelOrder
        {
            public CancelOrder(int orderId)
            {
                OrderId = orderId;
            }
            public int OrderId { get; }
        }

        public class CancelOrderIntegrity : AbstractValidator<CancelOrder>
        {
            public CancelOrderIntegrity()
            {
                RuleFor(co => co.OrderId)
                    .GreaterThan(0)
                    .Must(Exist);
            }

            private static bool Exist(int orderId)
            {
                var order = new Order { Id = orderId };
                Handler.Composer.Proxy<IStash>().Put(order);
                return true;
            }
        }

        public class OrderHandler : PipelineHandler
        {
            [Mediates]
            public Order Cancel(CancelOrder cancel, IHandler composer)
            {
                var order = composer.Proxy<IStash>().Get<Order>();
                order.Status = OrderStatus.Cancelled;
                return order;
            }
        }

        public class MiddlewareProvider : Handler
        {
            [Provides]
            public IMiddleware<TReq, TResp>[] GetMiddleware<TReq, TResp>()
            {
                return new IMiddleware<TReq, TResp>[]
                {
                    new ValidateMiddleware<TReq, TResp>()
                };
            }

            [Provides]
            public IValidator<CancelOrder>[] ChangeOrderValidator() => 
                new[] { new CancelOrderIntegrity() };

        }

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.ResetDescriptors();
        }

        [TestMethod]
        public void Should_Add_To_Stash()
        {
            var order   = new Order();
            var handler = new Stash();
            handler.Proxy<IStash>().Put(order);
            Assert.AreSame(order, handler.Proxy<IStash>().Get<Order>());
        }

        [TestMethod]
        public void Should_Get_Or_Add_To_Stash()
        {
            var order   = new Order();
            var handler = new Stash();
            var result  = handler.Proxy<IStash>().GetOrPut(order);
            Assert.AreSame(order, result);
            Assert.AreSame(order, handler.Proxy<IStash>().Get<Order>());
        }

        [TestMethod]
        public void Should_Drop_From_Stash()
        {
            var order   = new Order();
            var stash   = new Stash();
            var handler = stash + new Stash(true);
            handler.Proxy<IStash>().Put(order);
            handler.Proxy<IStash>().Droid<Order>();
            Assert.IsNull(handler.Proxy<IStash>().Get<Order>());
        }

        [TestMethod]
        public void Should_Cascade_Stash()
        {
            var order    = new Order();
            var handler  = new Stash();
            var handler2 = new Stash() + handler;
            handler.Proxy<IStash>().Put(order);
            Assert.AreSame(order, handler2.Proxy<IStash>().Get<Order>());
        }

        [TestMethod]
        public void Should_Hide_Stash()
        {
            var order    = new Order();
            var handler  = new Stash();
            var handler2 = new Stash() + handler;
            handler.Proxy<IStash>().Put(order);
            handler2.Proxy<IStash>().Put<Order>(null);
            Assert.IsNull(handler2.Proxy<IStash>().Get<Order>());
        }

        [TestMethod]
        public async Task Should_Access_Stash()
        {
            HandlerDescriptor.GetDescriptor<OrderHandler>();

            var handler = new OrderHandler()
                        + new ValidationHandler()
                        + new FluentValidationValidator()
                        + new MiddlewareProvider();

            var order = await handler.Send<Order>(new CancelOrder(1));
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.Id);
            Assert.AreEqual(OrderStatus.Cancelled, order.Status);
        }
    }
}
