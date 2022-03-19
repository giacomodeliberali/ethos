using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ethos.Web.Host.Serilog;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestBodyPayload = await ReadRequestBody(context.Request);
        
        LogHelper.RequestPayload = requestBodyPayload;
        
        var originalResponseBodyStream = context.Response.Body;

        await using var responseBody = new MemoryStream();
        
        context.Response.Body = responseBody;

        // Continue down the Middleware pipeline, eventually returning to this class
        await _next(context);

        // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
        await responseBody.CopyToAsync(originalResponseBodyStream);
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();

        var body = request.Body;
        var buffer = new byte[Convert.ToInt32(request.ContentLength, CultureInfo.InvariantCulture)];
        await request.Body.ReadAsync(buffer, request.HttpContext.RequestAborted);
        var requestBody = Encoding.UTF8.GetString(buffer);
        body.Seek(0, SeekOrigin.Begin);
        request.Body = body;

        return requestBody;
    }
}