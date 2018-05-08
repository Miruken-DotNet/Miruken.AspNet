﻿namespace Miruken.Mediate.Oneway
{
    using Api;
    using Callback;
    using Concurrency;

    public class OnewayHandler : Handler
    {
        [Mediates]
        public Promise Oneway(Oneway request, IHandler composer)
        {
            return composer.Send(((IDecorator)request).Decoratee);
        }
    }

    public static class OnewayExtensions
    {
        public static Oneway<TResp> Oneway<TResp>(this IRequest<TResp> request)
        {
            return new Oneway<TResp>(request);
        }
    }
}
