namespace Miruken.Mediate.Oneway
{
    using System;
    using Api;

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

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other?.GetType() != GetType())
                return false;

            return other is Oneway otherOneway
                   && Equals(Decoratee, otherOneway.Decoratee);
        }

        public override int GetHashCode()
        {
            return Decoratee?.GetHashCode() ?? 0;
        }
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
