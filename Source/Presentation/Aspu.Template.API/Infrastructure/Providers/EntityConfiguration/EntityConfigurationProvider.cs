using Aspu.L2.DAL;
using Aspu.L2.DAL.Base;
using Aspu.Template.Database.MsSql;
using Aspu.Template.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Common;
using Serilog;

namespace Aspu.Template.API.Infrastructure.Providers.EntityConfiguration;

public class EntityConfigurationProvider(IConfiguration configuration) : ConfigurationProvider
{
    public override void Load()
    {
        using var dbContext = CreateContext();
        Data = CreateAndSaveDefaultValues(dbContext);
    }

    private ConfigurationDbContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        var provider = configuration.GetValue(SqlConstants.Provider, SqlConstants.MsSQL) ?? SqlConstants.MsSQL;
        return provider.ToLower() switch
        {
            SqlConstants.MsSQL => new ConfigurationDbContext(optionsBuilder.UseSqlServer(MssqlDatabaseService.GetConnectionString(configuration)).Options),
            SqlConstants.MySQL => new ConfigurationDbContext(optionsBuilder.UseSqlServer(MssqlDatabaseService.GetConnectionString(configuration)).Options),
            SqlConstants.PostgreSQL => new ConfigurationDbContext(optionsBuilder.UseSqlServer(MssqlDatabaseService.GetConnectionString(configuration)).Options),
            _ => throw new InvalidArgumentException($"Unsupported database provider: {provider}")
        };
    }

    private static Dictionary<string, string?> CreateAndSaveDefaultValues(ConfigurationDbContext dbContext)
    {
        try
        {
            if (dbContext.AppSettings.Any())
                return dbContext.AppSettings.ToDictionary(c => c.Key, c => c.Value, StringComparer.OrdinalIgnoreCase);

            var settings = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                ["WidgetOptions:EndpointId"] = "b3da3c4c-9c4e-4411-bc4d-609e2dcc5c67",
                ["WidgetOptions:DisplayLabel"] = "Widgets Incorporated, LLC.",
                ["WidgetOptions:WidgetRoute"] = "api/widgets"
            };

            dbContext.AppSettings.AddRange(settings.Select(kvp => new AppSetting { Key = kvp.Key, Value = kvp.Value }).ToArray());
            dbContext.SaveChanges();
            return settings;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database configuration error");
            return [];
        }
    }
}
