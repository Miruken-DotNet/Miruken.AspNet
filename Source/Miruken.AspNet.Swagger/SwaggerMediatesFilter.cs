namespace Miruken.AspNet.Swagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Description;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Callback.Policy;
    using Http;
    using Mediate;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Swashbuckle.Swagger;

    public class SwaggerMediatesFilter : IDocumentFilter
    {
        private readonly Fixture _examples;

        private static readonly MethodInfo CreateExampleMethod =
            typeof(SwaggerMediatesFilter).GetMethod(nameof(CreateExample),
         BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly JsonSerializerSettings SerializerSettings
            = new JsonSerializerSettings
            {
                ContractResolver               = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling               = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

        private static readonly string[] JsonFormats = { "application/json" };

        public SwaggerMediatesFilter()
        {
            _examples = CreateExamplesGenerator();
        }

        public static string ModelToSchemaId(Type type)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Message<>))
            {
                var message = type.GetGenericArguments()[0];
                return $"{typeof(Message).FullName}<{message.FullName}>";
            }
            return type.FullName;
        }

        public void Apply(SwaggerDocument document,
            SchemaRegistry registry, IApiExplorer apiExplorer)
        {
            var bindings = MediatesAttribute.Policy.GetMethods();
            AddPaths(document, registry, "Process", bindings);

            document.paths = document.paths.OrderBy(e => e.Key)
                .ToDictionary(e => e.Key, e => e.Value);

            var messageName = typeof(Message).FullName ?? "Message";
            document.definitions[messageName].example = new Message();
        }

        private void AddPaths(SwaggerDocument document, SchemaRegistry registry,
            string resource, IEnumerable<PolicyMethodBinding> bindings)
        {
            foreach (var path in BuildPaths(resource, registry, bindings))
            {
                if (!document.paths.ContainsKey(path.Item1))
                    document.paths.Add(path.Item1, path.Item2);
            }
        }

        private IEnumerable<Tuple<string, PathItem>> BuildPaths(
            string resource, SchemaRegistry registry, IEnumerable<PolicyMethodBinding> bindings)
        {
            return bindings.Select(x =>
            {
                var requestType = x.Key as Type;
                if (requestType == null || requestType.IsAbstract ||
                    requestType.ContainsGenericParameters)
                    return null;
                var responseType   = x.Dispatcher.LogicalReturnType;
                var handler        = x.Dispatcher.Owner.HandlerType;
                var assembly       = handler.Assembly.GetName();
                var tag            = $"{assembly.Name} - {assembly.Version}";
                var requestSchema  = GetMessageSchema(registry, requestType);
                var responseSchema = GetMessageSchema(registry, responseType);
                var requestPath    = HttpOptionsExtensions.GetRequestPath(requestType);

                var requestSummary = GetReferencedSchema(registry,
                    registry.GetOrRegister(requestType))?.description;

                var handlerAssembly = handler.Assembly.GetName();
                var handlerNotes    = $"Handled by {handler.FullName} in {handlerAssembly.Name} - {handlerAssembly.Version}";

                return Tuple.Create($"/{resource}/{requestPath}", new PathItem
                {
                    post = new Operation
                    {
                        summary     = requestSummary,
                        operationId = requestType.FullName,
                        description = handlerNotes,
                        tags        = new[] { tag },
                        consumes    = JsonFormats,
                        produces    = JsonFormats,
                        parameters  = new[]
                        {
                            new Parameter
                            {
                                @in         = "body",
                                name        = "message",
                                description = "request to process",
                                schema      = requestSchema,
                                required    = true
                            }
                        },
                        responses = new Dictionary<string, Response>
                        {
                            {
                                "200", new Response {
                                    description = "OK",
                                    schema      = responseSchema
                                }
                            }
                        }
                    }
                });
            }).Where(p => p != null);
        }

        private static Schema GetReferencedSchema(SchemaRegistry registry, Schema reference)
        {
            var parts = reference.@ref.Split('/');
            var name = parts.Last();
            return registry.Definitions[name];
        }

        private Schema GetMessageSchema(SchemaRegistry registry, Type message)
        {
            if (message == null || message == typeof(void) || message == typeof(object))
                return registry.GetOrRegister(typeof(Message));
            var schema = registry.GetOrRegister(typeof(Message<>).MakeGenericType(message));
            var definition = GetReferencedSchema(registry, schema);
            definition.example = CreateExampleMessage(message);
            return schema;
        }

        private object CreateExampleMessage(Type message)
        {
            try
            {
                var creator = CreateExampleMethod.MakeGenericMethod(message);
                var example = creator.Invoke(null, new object[] { _examples });
                var jsonString = JsonConvert.SerializeObject(example, SerializerSettings);
                return JsonConvert.DeserializeObject(jsonString);
            }
            catch
            {
                return null;
            }
        }

        private static Message<T> CreateExample<T>(ISpecimenBuilder builder)
        {
            return new Message<T> { Payload = builder.Create<T>() };
        }

        private static Fixture CreateExamplesGenerator()
        {
            var generator     = new Fixture { RepeatCount = 1 };
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(generator);
            return generator;
        }
    }

    public class Message<T>
    {
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public T Payload { get; set; }
    }
}
