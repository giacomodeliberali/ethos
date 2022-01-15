using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Ethos.Application.Contracts;
using Ethos.Domain;
using Ethos.Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ethos.Web.Host
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(
            RequestDelegate next,
            ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException businessException)
            {
                await HandleExceptionAsync(context, env, businessException);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CaughtException");
                await HandleExceptionAsync(context, env, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, IWebHostEnvironment env, Exception exception)
        {
            string result;

            if (env.IsDevelopment())
            {
                var errorMessage = new ExceptionDto()
                {
                    Message = exception.Message,
                    InnerException = exception.InnerException != null ? new ExceptionDto()
                    {
                        Message = exception.InnerException.Message,
                    }
                        : null,
                };

                result = JsonSerializer.Serialize(errorMessage, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
            }
            else
            {
                var errorMessage = new ExceptionDto
                {
                    Message = exception.Message,
                };

                result = JsonSerializer.Serialize(errorMessage, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}
