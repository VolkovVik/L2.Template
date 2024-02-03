using Serilog.Context;
using System.Text;

namespace Aspu.Template.API.Infrastructure.Middlewares;

public class RequestResponseLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        string body = await ReadRequestBody(context.Request);
        body = LogHelper.TransformString(body);

        var originalResponseBodyStream = context.Response.Body;
        using var newResponseBodyStream = new MemoryStream();
        using (LogContext.PushProperty(LogHelper.RequestBody, LogHelper.ConditionWrite(LogHelper.Body, body)))
        {
            context.Response.Body = newResponseBodyStream;

            await _next(context);

            await newResponseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
    }

    private static async Task<string> ReadRequestBody(HttpRequest request, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        request.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(request.Body, encoding);
        var value = await reader.ReadToEndAsync().ConfigureAwait(false);
        request.Body.Seek(0, SeekOrigin.Begin);
        return value;
    }
}