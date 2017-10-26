﻿namespace Miruken.Polly
{
    using Mediate;

    public class Policied<TResponse> : RequestDecorator<TResponse>
    {
        public Policied()
        {          
        }

        public Policied(IRequest<TResponse> request)
            : base(request)
        {         
        }
    }
}