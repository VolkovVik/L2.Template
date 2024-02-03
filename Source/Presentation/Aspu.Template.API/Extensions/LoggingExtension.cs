using Aspu.Template.API.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Aspu.Template.API.Extensions;

public static class LoggingExtension
{
    private static readonly PathString _apiPath = new("/api");

    public static void SetDefaultConfiguration()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341")
            .WriteTo.File("logs/server.log",
                retainedFileCountLimit: 3,
                rollingInterval: RollingInterval.Day)
             .CreateBootstrapLogger();
    }

    public static void SetProductionConfiguration()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.Seq("http://localhost:5341", LogEventLevel.Information)
            .WriteTo.Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code)
            .WriteTo.File("logs/server.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 63,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 256 * 1000 * 1000,
                flushToDiskInterval: TimeSpan.FromSeconds(5),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static void UseSerilogLogger(this IApplicationBuilder app)
    {
        app.UseWhen(
            predicate => predicate.Request.Path.StartsWithSegments(_apiPath),
            config => config.UseMiddleware<RequestResponseLoggingMiddleware>());
        app.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = GetLevel!;
            opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
            opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} <user={User}> responded {StatusCode} in {Elapsed:N0} ms{QueryString}{RequestBody}{ResponseBody}";
        });
    }

    private static LogEventLevel GetLevel(HttpContext httpContext, double elapsed, Exception ex) =>
        httpContext.Request.Method == "HEAD" ||
        httpContext.Request.Method == "OPTIONS" ||
        !httpContext.Request.Path.StartsWithSegments(_apiPath)
            ? LogEventLevel.Verbose : LogEventLevel.Debug;
}
