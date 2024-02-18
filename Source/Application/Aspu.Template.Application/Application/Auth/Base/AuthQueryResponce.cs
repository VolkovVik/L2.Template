using Aspu.Template.Application.Infrastructure.Attributes;

namespace Aspu.L2.Application.Application.Auth.Base;

public record AuthQueryResponse
{
    [LoggerProperty("ExpirationTime")]
    public DateTime? ExpirationTime { get; set; }

    [LoggerProperty("ExpirationMinutes")]
    public long? ExpirationMinutes => !ExpirationTime.HasValue ? null : (long)(ExpirationTime.Value - DateTime.UtcNow).TotalMinutes;

    [LoggerProperty("ExpirationSeconds")]
    public long? ExpirationSeconds => !ExpirationTime.HasValue ? null : (long)(ExpirationTime.Value - DateTime.UtcNow).TotalSeconds;
}