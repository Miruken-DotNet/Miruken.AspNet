namespace Miruken.Mediator.Middleware
{
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using Validate;

    public class ValidationMiddleware<Req, Res> : IMiddleware<Req, Res>
    {
        public int? Order { get; set; } = Stage.Validation;

        public async Task<Res> Next(Req request, MethodBinding method,
            IHandler composer, NextDelegate<Task<Res>> next)
        {
            await Validate(request, composer);
            var response = await next();
            return await Validate(response, composer);
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
