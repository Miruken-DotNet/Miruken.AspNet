namespace Miruken.Mediator.Oneway
{
    using System;
    using Callback;

    public abstract class Oneway : IRequest, IDecorator
    {
        protected Oneway()
        {
        }

        protected Oneway(object request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            Decoratee = request;
        }

        public object Decoratee { get; protected set; }
    }

    public class Oneway<TResp> : Oneway
    {
        public Oneway()
        {
        }

        public Oneway(IRequest<TResp> request)
            : base(request)
        {
        }

        public IRequest<TResp> Request
        {
            get { return (IRequest<TResp>)Decoratee; }
            set { Decoratee = value; }
        }
    }
}
