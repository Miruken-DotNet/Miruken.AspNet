namespace Miruken.AspNet.Swagger
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Swashbuckle.Application;

    public static class SwaggerDocsConfigExtensions
    {
        public static SwaggerDocsConfig UseMiruken(
            this SwaggerDocsConfig config)
        {
            config.SchemaId(SwaggerMediatesFilter.ModelToSchemaId);
            config.DocumentFilter<SwaggerMediatesFilter>();
            config.IncludeApiComments("Miruken.Mediate");
            return config;
        }

        public static void IncludeApiComments(
            this SwaggerDocsConfig config, params string[] patterns)
        {
            var files =
                (AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ??
                 AppDomain.CurrentDomain.BaseDirectory).Split(';')
                .SelectMany(path => Directory.GetFiles(path, "*.xml"));

            foreach (var file in files)
            {
                var filename = new FileInfo(file).Name;
                if (patterns.Any(pattern => Regex.IsMatch(filename, pattern)))
                    config.IncludeXmlComments(file);
            }
        }
    }
}
