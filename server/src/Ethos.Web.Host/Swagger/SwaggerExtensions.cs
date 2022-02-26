using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ethos.Web.Host.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddEthosSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.UseAllOfToExtendReferenceSchemas(); // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1915
                c.CustomSchemaIds(type =>
                {
                    if (type.IsNested)
                    {
                        return $"{type.DeclaringType!.Name}_{type.Name}";
                    }

                    return type.Name;
                });

                c.SwaggerGeneratorOptions.DocumentFilters.Add(new TypeScriptDocumentProcessor());
                c.SwaggerGeneratorOptions.DocumentFilters.Add(new SortSchemasDocumentProcessor());
                c.CustomOperationIds(apiDesc =>
                {
                    var hasMethodInfo = apiDesc.TryGetMethodInfo(out var methodInfo);

                    if (!hasMethodInfo)
                    {
                        return null;
                    }

                    return methodInfo.Name.Replace("Async", string.Empty);
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ethos",
                    Version = "v1",
                });
                var path = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory) !);
                foreach (var filePath in Directory.GetFiles(path, "*.xml"))
                {
                    c.IncludeXmlComments(filePath);
                }
            });
        }
    }
}
