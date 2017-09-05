﻿namespace Miruken.Mediate.Castle
{
    using System;
    using System.Collections.Generic;
    using Cache;
    using global::Castle.MicroKernel.Registration;
    using Miruken.Castle;
    using Oneway;
    using Route;
    using Schedule;

    public class MediateFeature : FeatureInstaller
    {
        private bool _standardMiddleware;

        public MediateFeature WithStandardMiddleware()
        {
            _standardMiddleware = true;
            return this;
        }

        protected override IEnumerable<FromDescriptor> GetFeatures()
        {
            yield return Types.From(
                typeof(CachedHandler), typeof(OnewayHandler),
                typeof(ScheduleHandler), typeof(PassThroughRouter));

            if (_standardMiddleware)
                yield return Types.From(
                    typeof(ValidateMiddleware<,>),
                    typeof(LogMiddleware<,>));
        }

        public override void InstallFeatures(FromDescriptor from)
        {
        }
    }
}