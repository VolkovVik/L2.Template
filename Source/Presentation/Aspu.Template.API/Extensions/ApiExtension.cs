using Aspu.Template.API.Extensions;
using Aspu.Template.API.Infrastructure.Services;
using Aspu.Template.Application;
using Aspu.Template.Application.Interfaces;
using Aspu.Template.Application.Services;
using Aspu.Template.Domain.Configuration;
using Aspu.Template.Infrastructure;

namespace Aspu.Template.API.Configure;

public static class ApiExtension
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthCheckServices();
        services.AddApiServices(configuration);
        services.AddApplicationServices(configuration);
        services.AddInfrastructureServices(configuration);
        services.AddAuthenticationServices(configuration);
        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IVersionService, VersionService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.Configure<JwtTokenConfig>(configuration.GetSection("JwtToken"));
        services.Configure<HostOptions>(hostOptions => { hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore; });
        return services;
    }
}
