using Aspu.Template.API.Configure;
using Aspu.Template.Application;
using Aspu.Template.Infrastructure;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

ConfigureLoggingServices.SetDefaultConfiguration();
try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;
    builder.Host.UseSerilog((context, services, configuration) => configuration
       .ReadFrom.Configuration(context.Configuration)
       .ReadFrom.Services(services)
       .Enrich.FromLogContext());

    // Add services to the container.

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // Global settings: use the defaults, but serialize enums as strings
        // (because it really should be the default)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();
    builder.Services.Configure<HostOptions>(hostOptions => hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

    builder.Services.AddApiServices(configuration);
    builder.Services.AddApplicationServices(configuration);
    builder.Services.AddInfrastructureServices(configuration);
    builder.Services.AddAuthenticationServices(configuration);

    var app = builder.Build();
    app.ConfigureLogger();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseWebSockets();
    app.UseCors(options => options.SetIsOriginAllowed(x => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Stopping web application");
    Log.CloseAndFlush();
}

