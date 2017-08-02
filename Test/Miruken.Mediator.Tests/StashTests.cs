namespace Miruken.Mediator.Tests
{
    using System.Threading.Tasks;
    using Callback;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Validate;
    using Validate.FluentValidation;
    using static Protocol;

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

        public class ChangeOrderIntegrity : AbstractValidator<CancelOrder>
        {
            public ChangeOrderIntegrity()
            {
                RuleFor(co => co.OrderId)
                    .GreaterThan(0)
                    .Must(Exist);
            }

            private static bool Exist(int orderId)
            {
                var order = new Order {Id = orderId};
                P<IStash>(Handler.Composer).Put(order);
                return true;
            }
        }

        [Pipeline]
        public class OrderHandler : Handler
        {
            [Mediates]
            public Order Cancel(CancelOrder cancel, IHandler composer)
            {
                var order = P<IStash>(composer).Get<Order>();
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
                    new ValidationMiddleware<TReq, TResp>()
                };
            }

            [Provides]
            public IValidator<CancelOrder>[] ChangeOrderValidator() 
                => new[] {new ChangeOrderIntegrity()};

        }

        [TestMethod]
        public void Should_Add_To_Stash()
        {
            var order   = new Order();
            var handler = new Stash();
            P<IStash>(handler).Put(order);
            Assert.AreSame(order, P<IStash>(handler).Get<Order>());
        }

        [TestMethod]
        public void Should_Get_Or_Add_To_Stash()
        {
            var order   = new Order();
            var handler = new Stash();
            var result  = P<IStash>(handler).GetOrPut(order);
            Assert.AreSame(order, result);
            Assert.AreSame(order, P<IStash>(handler).Get<Order>());
        }

        [TestMethod]
        public void Should_Drop_From_Stash()
        {
            var order   = new Order();
            var stash   = new Stash();
            var handler = stash + new Stash(true);
            P<IStash>(handler).Put(order);
            P<IStash>(handler).Drop<Order>();
            Assert.IsNull(P<IStash>(handler).Get<Order>());
        }

        [TestMethod]
        public void Should_Cascade_Stash()
        {
            var order    = new Order();
            var handler  = new Stash();
            var handler2 = new Stash() + handler;
            P<IStash>(handler).Put(order);
            Assert.AreSame(order, P<IStash>(handler2).Get<Order>());
        }

        [TestMethod]
        public void Should_Hide_Stash()
        {
            var order    = new Order();
            var handler  = new Stash();
            var handler2 = new Stash() + handler;
            P<IStash>(handler).Put(order);
            P<IStash>(handler2).Put<Order>(null);
            Assert.IsNull(P<IStash>(handler2).Get<Order>());
        }

        [TestMethod]
        public async Task Should_Access_Stash()
        {
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
