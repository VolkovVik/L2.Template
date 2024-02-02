using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspu.Template.API.Infrastructure.HealthCheck;

public class MyHealthCheck : IHealthCheck
{
    private readonly Random _random = new();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var responseTime = _random.Next(1, 300);
        if (responseTime < 100)
            return Task.FromResult(HealthCheckResult.Healthy("Healthy result from MyHealthCheck"));

        if (responseTime < 200)
            return Task.FromResult(HealthCheckResult.Degraded("Degraded result from MyHealthCheck"));

        return Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from MyHealthCheck"));
    }
}
