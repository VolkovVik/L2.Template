using Aspu.Template.API.Infrastructure.Providers.EntityConfiguration;

namespace Aspu.Template.API.Extensions;

public static class ConfigurationExtensions
{
    public static IHostBuilder ConfigureAppSettings(this IHostBuilder host)
    {
        /// var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        host.ConfigureAppConfiguration((hostContext, builder) =>
        {
            var env = hostContext.HostingEnvironment;

            builder.Sources.Clear();
            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            builder.AddEnvironmentVariables();

            builder.AddEntityConfiguration();

            IConfiguration config = builder.Build();
            /// var connectionString = config["SQLServerProvider"];
        });

        return host;
    }

    public static IConfigurationBuilder AddEntityConfiguration(this IConfigurationBuilder builder)
    {
        var configuration = builder.Build();
        builder.Add(new EntityConfigurationSource(configuration));
        return builder;
    }
}