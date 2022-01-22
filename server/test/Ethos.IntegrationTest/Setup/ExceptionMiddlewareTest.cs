using System;
using System.Collections.Generic;
using System.Net;
using Ethos.Domain.Exceptions;
using Ethos.Web.Host;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Setup
{
    public class ExceptionMiddlewareTest
    {
        [Theory]
        [MemberData(nameof(GetExceptions))]
        public void ShouldReturnAppropriateStatusCode(Exception exception, HttpStatusCode expectedStatusCode)
        {
            RequestDelegate _next = context => throw exception;
            var sut = new ExceptionHandler(_next, Substitute.For<ILogger<ExceptionHandler>>());

            var context = Substitute.For<HttpContext>();
            sut.Invoke(context, Substitute.For<IWebHostEnvironment>());

            context.Response.ContentType.ShouldBe("application/json");
            context.Response.StatusCode.ShouldBe((int)expectedStatusCode);
        }

        public static IEnumerable<object[]> GetExceptions()
        {
            yield return new object [] { new Exception(), HttpStatusCode.InternalServerError };
            yield return new object [] { new ValidationException(string.Empty), HttpStatusCode.BadRequest };
            yield return new object [] { new BusinessException(string.Empty), HttpStatusCode.BadRequest };
            yield return new object [] { new AuthenticationException(string.Empty), HttpStatusCode.Unauthorized };
        }
    }
}
