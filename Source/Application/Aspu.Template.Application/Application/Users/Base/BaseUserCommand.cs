using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Enums;

namespace Aspu.Template.Application.Application.Users.Base;

public record BaseUserCommand
{
    [LoggerProperty("Role")]
    public AppRole Role { get; set; }

    [LoggerProperty("Login")]
    public string? Login { get; set; }
    public string? Password { get; set; }
}