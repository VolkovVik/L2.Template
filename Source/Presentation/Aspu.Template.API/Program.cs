using Aspu.L2.DAL;
using Aspu.L2.DAL.Base.Interfaces;
using Aspu.Template.API.Extensions;
using Aspu.Template.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

LoggingExtension.SetDefaultConfiguration();
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
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.FullName);
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Description = "Bearer $jwt_token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Services CORS
    const string corsPolicy = "CorsAppPoliocy";
    builder.Services.AddCors(p => p.AddPolicy(corsPolicy, builder => builder.SetIsOriginAllowed(x => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCustomServices(configuration);
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbHelper = scope.ServiceProvider.GetRequiredService<IDatabaseHelper>();
        await dbContext.Database.MigrateAsync();
        dbHelper.Migrate(dbContext);
    }

    app.UseSerilogLogger();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseWebSockets();
    app.UseCors(corsPolicy);

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.UseHealthCheck();

    app.MapGet("/version/{min?}/{max?}", (string? min, string? max, HttpContext context) =>
    {
        var routeValues = context.GetRouteData().Values ?? [];
        var service = context.RequestServices.GetRequiredService<IVersionService>();
        context.Response.WriteAsJsonAsync(new { Version = service.ApplicationVersion, Data = routeValues });
    }).RequireCors().RequireAuthorization().WithDisplayName("Version");

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

