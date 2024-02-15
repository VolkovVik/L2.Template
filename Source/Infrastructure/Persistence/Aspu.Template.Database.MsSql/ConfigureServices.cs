using Aspu.L2.DAL;
using Aspu.L2.DAL.Base.Interfaces;
using Aspu.Template.Application.Interfaces.Infrastructure;
using Aspu.Template.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aspu.Template.Database.MsSql;

public static class ConfigureServices
{
    public static IServiceCollection AddMsSQLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDatabaseHelper, DatabaseHelper>();
        services.AddScoped<IDatabaseService, MssqlDatabaseService>();

        services.AddDatabaseServices(configuration);
        services.AddIdentityServices();
        return services;
    }

    private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(ConfigureServices).Assembly.FullName;
        var connectionString = MssqlDatabaseService.GetConnectionString(configuration);
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(assembly));
        });
        services.AddDbContext<ConfigurationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(assembly));
        });
        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = false;
            options.SignIn.RequireConfirmedAccount = false;

            options.Password.RequiredLength = 4;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }
}
