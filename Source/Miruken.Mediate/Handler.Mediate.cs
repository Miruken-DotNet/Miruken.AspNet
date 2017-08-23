namespace Miruken.Mediate
{
    using System;
    using Callback;
    using Concurrency;

    public static class HandlerMediateExtensions
    {
        public static Promise Send(this IHandler handler, object request)
        {
            if (handler == null)
                return Promise.Empty;
            var command = new Command(request)
            {
                WantsAsync = true,
                Policy     = MediatesAttribute.Policy
            };
            return (new Stash() + handler.Resolve()).Handle(command)
                 ? (Promise)command.Result
                 : Promise.Rejected(new NotSupportedException(
                       $"{request.GetType()} not handled"));
        }

        public static Promise<TResp> Send<TResp>(this IHandler handler, object request)
        {
            if (handler == null)
                return Promise<TResp>.Empty;
            var command = new Command(request)
            {
                WantsAsync = true,
                Policy     = MediatesAttribute.Policy
            };
            return (new Stash() + handler.Resolve()).Handle(command)
                 ? (Promise<TResp>)((Promise)command.Result)
                       .Coerce(typeof(Promise<TResp>))
                 : Promise<TResp>.Rejected(new NotSupportedException(
                       $"{request.GetType()} not handled"));
        }

        public static Promise<TResp> Send<TResp>(this IHandler handler, IRequest<TResp> request)
        {
            if (handler == null)
                return Promise<TResp>.Empty;
            var command = new Command(request)
            {
                WantsAsync = true,
                Policy     = MediatesAttribute.Policy
            };
            if (!(new Stash() + handler.Resolve()).Handle(command))
                return Promise<TResp>.Rejected(new NotSupportedException(
                    $"{request.GetType()} not handled"));
            var promise = (Promise)command.Result;
            return (Promise<TResp>)promise.Coerce(typeof(Promise<TResp>));
        }

        public static Promise Publish(this IHandler handler, object notification)
        {
            if (handler == null)
                return Promise.Empty;
            var command = new Command(notification, true)
            {
                WantsAsync = true,
                Policy     = MediatesAttribute.Policy
            };
            return (new Stash() + handler.Resolve()).Handle(command, true)
                 ? (Promise)command.Result
                 : Promise.Empty;
        }
    }
}
