using Aspu.L2.DAL.Base;
using Aspu.Template.Application.Interfaces.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Serilog;

namespace Aspu.Template.Database.MsSql;

public class MssqlDatabaseService(IConfiguration configuration) : IDatabaseService
{
    private readonly IConfiguration _configuration = configuration;

    public static string? GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(SqlConstants.ConnectionName);
        connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : configuration.GetConnectionString(SqlConstants.MsSQL);
        return connectionString;
    }

    public void CreateBackup(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;

        try
        {
            Log.Debug($"Start creating DB backup: {fileName}");

            var connectionString = GetConnectionString(_configuration);
            using var connection = new SqlConnection(connectionString);
            var server = new Server(new ServerConnection(connection));
            var backup = new Backup()
            {
                Database = connection.Database,
                Action = BackupActionType.Database
            };
            backup.Devices.AddDevice(fileName, DeviceType.File);
            backup.Initialize = true;
            backup.SqlBackup(server);
            Log.Debug($"Stop creating DB backup: {fileName}");
        }
        catch (FailedOperationException ex)
        {
            Exception e = ex;
            while (e.InnerException != null)
            {
                e = e.InnerException;
                if (e is SqlException)
                    break;
            }

            Log.Error(e, "Create full backup error");
            throw;
        }
        catch (Exception e)
        {
            Log.Error(e, "Create full backup error");
            throw;
        }
    }
}