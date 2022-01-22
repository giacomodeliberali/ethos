using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Ethos.Application.Contracts;
using Ethos.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ethos.Web.Host
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        private readonly Dictionary<Type, HttpStatusCode> _httpStatusCodes = new ()
        {
            { typeof(BusinessException), HttpStatusCode.BadRequest },
            { typeof(ValidationException), HttpStatusCode.BadRequest },
            { typeof(AuthenticationException), HttpStatusCode.Unauthorized },
        };

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
            catch (Exception ex)
            {
                if (ex is BusinessException)
                {
                    _logger.LogWarning(ex, ex.Message);
                }
                else
                {
                    _logger.LogError(ex, ex.Message);
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorMessage = new ExceptionDto()
            {
                Message = exception.Message,
            };

            var result = JsonSerializer.Serialize(errorMessage, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            context.Response.ContentType = "application/json";

            if (_httpStatusCodes.TryGetValue(exception.GetType(), out var responseStatus))
            {
                context.Response.StatusCode = (int)responseStatus;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            await context.Response.WriteAsync(result);
        }
    }
}
