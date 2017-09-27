﻿namespace Miruken.Mediate
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using FluentValidation;
    using Validate;
    using ValidationException = Validate.ValidationException;

    public class ValidateMiddleware<TRequest, TResponse>
        : IMiddleware<TRequest, TResponse>
    {
        public int? Order { get; set; } = Stage.Validation;

        public Task<TResponse> Next(TRequest request, MethodBinding method,
            IHandler composer, NextDelegate<Task<TResponse>> next)
        {
            var validator = composer.Proxy<IValidating>();
            return Validate(request, validator)
                .Then((req, s) => next())
                .Then((resp, s) => Validate(resp, validator));
        }

        private static Promise<T> Validate<T>(T message, IValidating validator)
        {
            return message == null ? Promise<T>.Empty
                 : validator.ValidateAsync(message).Then((outcome, s) =>
                   {
                       if (!outcome.IsValid)
                           throw new ValidationException(outcome);
                       return message;
                   });
        }
    }
}
