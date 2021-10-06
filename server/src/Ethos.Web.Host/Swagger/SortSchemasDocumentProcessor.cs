using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ethos.Web.Host.Swagger
{
    public class SortSchemasDocumentProcessor : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var sortedSchema = context.SchemaRepository.Schemas.ToList().OrderBy(s => s.Key);

            context.SchemaRepository.Schemas.Clear();

            foreach (var schema in sortedSchema)
            {
                context.SchemaRepository.Schemas.Add(schema.Key, schema.Value);
            }
        }
    }
}
