namespace Miruken.AspNet.Swagger
{
    using System;
    using Swashbuckle.Swagger;

    public static class SwaggerAuthorizationExtensions
    {
        public static SwaggerMediatesFilter RequireBasicToken(
            this SwaggerMediatesFilter filter)
        {
            filter.Operations += op => op.parameters.Add(BasicTokenParameter);
            return filter;
        }

        public static SwaggerMediatesFilter RequireBearerToken(
            this SwaggerMediatesFilter filter, Action<Parameter> configure = null)
        {
            filter.Operations += op => op.parameters.Add(BearerTokenParameter);
            return filter;
        }

        public static SwaggerMediatesFilter RequireAuthorizationToken(
            this SwaggerMediatesFilter filter, string description)
        {
            var token = CreateAuthorizationParameter(description);
            filter.Operations += op => op.parameters.Add(token);
            return filter;
        }

        private static Parameter CreateAuthorizationParameter(string description)
        {
            return new Parameter
            {
                name        = "Authorization",
                @in         = "header",
                description = description,
                required    = false,
                type        = "string"
            };
        }

        private static readonly Parameter BasicTokenParameter
            = CreateAuthorizationParameter("Basic Token");

        private static readonly Parameter BearerTokenParameter
            = CreateAuthorizationParameter("Bearer Token");
    }
}
