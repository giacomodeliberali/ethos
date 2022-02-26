using System.Linq;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Ethos.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.Web.Host
{
    public static class ControllersExtensions
    {
        public static void AddEthosControllers(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.Keys
                            .SelectMany(k => context.ModelState[k].Errors)
                            .Select(e => e.ErrorMessage);

                        var exception = new ExceptionDto()
                        {
                            Message = string.Join(". ", errors),
                        };

                        var result = new BadRequestObjectResult(exception);

                        result.ContentTypes.Add(MediaTypeNames.Application.Json);

                        return result;
                    };
                })
                .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IEthosWebAssemblyMarker).Assembly));
        }
    }
}
