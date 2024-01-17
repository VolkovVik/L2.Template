using Aspu.Template.Application.Infrastructure.Attributes;

namespace Aspu.L2.Application.Application.Auth.Base;

public record AuthQueryResponce
{
    [LoggerProperty("ExpirationTime")]
    public DateTime? ExpirationTime { get; set; }

    [LoggerProperty("ExpirationMinutes")]
    public long? ExpirationMinutes => !ExpirationTime.HasValue ? null : (long)(ExpirationTime.Value - DateTime.UtcNow).TotalMinutes;
}