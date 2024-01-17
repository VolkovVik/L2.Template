using Aspu.Template.API.Infrastructure.Services;
using Aspu.Template.Application.Interfaces;
using Aspu.Template.Application.Services;
using Aspu.Template.Domain.Configuration;

namespace Aspu.Template.API.Configure;

public static class ConfigureApiServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IVersionService, VersionService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.Configure<JwtTokenConfig>(configuration.GetSection("JwtToken"));
        return services;
    }
}
