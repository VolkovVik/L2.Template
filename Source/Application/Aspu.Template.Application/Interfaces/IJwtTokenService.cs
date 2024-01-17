using Aspu.Template.Domain.Entities;
using System.Security.Claims;

namespace Aspu.Template.Application.Interfaces;

public interface IJwtTokenService
{
    (string token, DateTime expirationTime) CreateToken(ApplicationUser user, IEnumerable<Claim>? claims = null);
}
