namespace Miruken.Mediator.Middleware
{
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using Validate;

    public class ValidationMiddleware<Cb, Res> : IMiddleware<Cb, Res>
    {
        public int? Order { get; set; }

        public Promise<Res> Next(Cb request, MethodBinding method,
            IHandler composer, NextDelegate<Promise<Res>> next)
        {
            return Validate(request, composer)
                .Then((req, s) => next())
                .Then((resp, s) => Validate(resp, composer));
        }

        private static Promise<T> Validate<T>(T message, IHandler composer)
        {
            return message == null ? Promise<T>.Empty 
                 : composer.P<IValidator>().ValidateAsync(message)
                    .Then((outcome, s) =>
                    {
                        if (!outcome.IsValid)
                            throw new ValidationException(outcome);
                        return message;
                    });
        }
    }
}
