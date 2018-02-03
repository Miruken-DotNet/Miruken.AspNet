namespace Miruken.Mediate.Oneway
{
    using System;

    public abstract class Oneway : IDecorator
    {
        protected Oneway()
        {
        }

        protected Oneway(object request)
        {
            Decoratee = request 
                     ?? throw new ArgumentNullException(nameof(request));
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
            get => (IRequest<TResp>)Decoratee;
            set => Decoratee = value;
        }
    }
}
