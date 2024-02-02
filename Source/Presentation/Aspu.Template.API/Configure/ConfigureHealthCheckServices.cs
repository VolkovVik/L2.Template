﻿using Aspu.L2.DAL;
using Aspu.Template.API.Infrastructure.HealthCheck;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspu.Template.API.Configure;

public static class ConfigureHealthCheckServices
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(tags: ["database"])
            .AddCheck<MyHealthCheck>(nameof(MyHealthCheck), tags: ["custom"]);
        ///.AddDiskStorageHealthCheck(x => x.AddDrive("C:\\", 10_000), "Проверить основной диск - предупреждение", HealthStatus.Degraded)
        ///.AddDiskStorageHealthCheck(x => x.AddDrive("C:\\", 2_000), "Проверить основной диск - ошибка", HealthStatus.Unhealthy);
        services.AddHealthChecksUI().AddInMemoryStorage();
        return services;
    }

    public static void ConfigureHealthCheck(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecksUI(opt =>
            {
                /// opt.UIPath = "/health";
                /// opt.ApiPath = "/healthAPI";
                /// opt.UseRelativeApiPath = false;
                /// opt.UseRelativeResourcesPath = false;
                /// opt.AsideMenuOpened = false;
            });

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecks("/health/custom", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("custom"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecks("/health/secure", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }).RequireAuthorization();
        });
    }
}