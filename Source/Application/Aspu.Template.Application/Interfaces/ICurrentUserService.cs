using System.Security.Claims;

namespace Aspu.Template.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? ClientIp { get; }
    ClaimsPrincipal? Principal { get; }
}
