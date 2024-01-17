using Serilog;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Aspu.Template.API.Infrastructure;

public static class LogHelper
{
    public const string Body = "body";
    private const string Query = "query";
    private const string Token = "token";
    private const string Login = "login";
    private const string Response = "response";
    private const string Undefined = "undefined";

    private const string UserName = "User";
    public const string RequestBody = "RequestBody";
    private const string QueryString = "QueryString";
    private const string ResponseBody = "ResponseBody";

    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;
        var response = GetHttpRequestResponse(httpContext.Response);
        var query = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;

        var isRequestLogging = IsRequestLogging(request);
        if (!isRequestLogging)
        {
            query = string.Empty;
            response = string.Empty;
            diagnosticContext.Set(RequestBody, string.Empty);
        }
        diagnosticContext.Set(QueryString, ConditionWrite(Query, query));
        diagnosticContext.Set(ResponseBody, ConditionWrite(Response, response));
        diagnosticContext.Set(UserName, httpContext.User?.Identity?.Name ?? Undefined);
    }

    private static bool IsRequestLogging(HttpRequest? request) =>
        request?.Path.Value?.Contains(Token, StringComparison.OrdinalIgnoreCase) != true &&
        request?.Path.Value?.Contains(Login, StringComparison.OrdinalIgnoreCase) != true;

    private static string GetHttpRequestResponse(HttpResponse response)
    {
        try
        {
            return GetHttpRequestResponseInternal(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return string.Empty;
        }
    }

    private static string GetHttpRequestResponseInternal(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(response.Body);
        var value = reader.ReadToEnd();
        response.Body.Seek(0, SeekOrigin.Begin);
        value = TransformString(value);
        return value;
    }

    public static string ConditionWrite(string name, string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : $"{Environment.NewLine}{name}: {value}";

    public static string TransformString(string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith('{')) return value;

            var jsonObj = JsonNode.Parse(value)?.AsObject();
            var jsonString = jsonObj?.ToJsonString(new JsonSerializerOptions { WriteIndented = false }) ?? string.Empty;
            return jsonString;
        }
        catch
        {
            return value;
        }
    }
}