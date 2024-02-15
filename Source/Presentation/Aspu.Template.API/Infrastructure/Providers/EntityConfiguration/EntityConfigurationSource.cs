namespace Aspu.Template.API.Infrastructure.Providers.EntityConfiguration;

public class EntityConfigurationSource(IConfiguration configuration) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new EntityConfigurationProvider(configuration);
}
