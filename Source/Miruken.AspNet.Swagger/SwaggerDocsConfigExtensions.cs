namespace Miruken.AspNet.Swagger
{
    using System;
    using System.IO;
    using System.Linq;
    using Swashbuckle.Application;

    public static class SwaggerDocsConfigExtensions
    {
        public static SwaggerDocsConfig UseMiruken(this SwaggerDocsConfig config)
        {
            config.SchemaId(SwaggerMediatesFilter.ModelToSchemaId);
            config.DocumentFilter<SwaggerMediatesFilter>();
            IncludeApiComments(config);
            return config;
        }

        private static void IncludeApiComments(SwaggerDocsConfig config)
        {
            var files =
                (AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ??
                 AppDomain.CurrentDomain.BaseDirectory).Split(';')
                .SelectMany(path => Directory.GetFiles(path, "*.xml"));

            foreach (var file in files)
            {
                var filename = new FileInfo(file).Name;
                if (filename.IndexOf("Api", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    filename.IndexOf("Miruken.Mediate", StringComparison.OrdinalIgnoreCase) >= 0)
                    config.IncludeXmlComments(file);
            }
        }
    }
}
