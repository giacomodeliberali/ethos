using System;
using System.Linq;
using Ethos.Application.Contracts;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ethos.Web.Host.Swagger
{
    public class TypeScriptDocumentProcessor : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var typesWithAttribute = AppDomain.CurrentDomain.GetAssemblies()
                .AsParallel()
                .SelectMany(x => x.GetTypes())
                .Where(type => type
                    .GetCustomAttributes(typeof(TypeScriptAttribute), true)
                    .Any());

            foreach (var type in typesWithAttribute)
            {
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            }

            var sortedSchema = context.SchemaRepository.Schemas.OrderBy(s => s.Key).ToList();

            context.SchemaRepository.Schemas.Clear();

            foreach (var schema in sortedSchema)
            {
                context.SchemaRepository.Schemas.Add(schema.Key, schema.Value);
            }
        }
    }
}
