using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Ethos.Web.Host.Serilog;

public static class LogHelper
{
    public static string RequestPayload { get; set; } = string.Empty;

    public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        diagnosticContext.Set("RequestBody", RemovePiiFromJsonString(RequestPayload));

        var responseBodyPayload = await ReadResponseBody(httpContext.Response);
        
        diagnosticContext.Set("ResponseBody", RemovePiiFromJsonString(responseBodyPayload));
        
        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is not null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return responseBody;
    }

    public static string RemovePiiFromJsonString(string text)
    {
        var replaceFields = "password,accessToken"
            .Split(',')
            .Select(s => $"(?<=\"{s}\":\")[^\"]+(?=\")");
        
        var regex = new Regex(string.Join('|', replaceFields), RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return regex.Replace(text, "*****");
    }
}