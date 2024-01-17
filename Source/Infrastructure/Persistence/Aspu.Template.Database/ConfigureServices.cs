using Aspu.L2.DAL.Base;
using Aspu.Template.Application.Interfaces.Infrastructure;
using Aspu.Template.Database.MsSql;
using Aspu.Template.Persistence.Base.Implementation;
using Aspu.Template.Persistence.Base.Implementation.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Common;

namespace Aspu.Template.Persistence;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddPersistenceServicesInternal(configuration);
        return services;
    }

    private static IServiceCollection AddPersistenceServicesInternal(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetValue(SqlConstants.Provider, SqlConstants.MsSQL) ?? SqlConstants.MsSQL;
        return provider.ToLower() switch
        {
            SqlConstants.MsSQL => services.AddMsSQLServices(configuration),
            SqlConstants.MySQL => services.AddMsSQLServices(configuration),
            SqlConstants.PostgreSQL => services.AddMsSQLServices(configuration),
            _ => throw new InvalidArgumentException($"Unsupported database provider: {provider}")
        };
    }
}
