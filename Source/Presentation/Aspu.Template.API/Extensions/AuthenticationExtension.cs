using Aspu.Template.Domain.Configuration;
using Aspu.Template.Domain.Enums;
using Aspu.Template.Domain.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Aspu.Template.API.Extensions;

public static class AuthenticationExtension
{
    private const string SectionName = "JwtToken";

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(SectionName).Get<JwtTokenConfig>();
        var issuer = config?.Issuer ?? string.Empty;
        var audience = config?.Audience ?? string.Empty;
        var secret = Encoding.UTF8.GetBytes(config?.Secret ?? string.Empty);
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policyBuilder => policyBuilder.RequireClaim(typeof(AppRole).Name, AppRole.Administrator.ToFriendlyString()));
        })
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidIssuer = issuer,
                ValidateAudience = false,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret)
            };
        });
        return services;
    }
}
