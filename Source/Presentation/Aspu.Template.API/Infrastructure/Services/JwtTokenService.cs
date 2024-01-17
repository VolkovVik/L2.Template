using Aspu.Template.Application.Interfaces;
using Aspu.Template.Domain.Configuration;
using Aspu.Template.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aspu.Template.Application.Services;

public class JwtTokenService(IOptions<JwtTokenConfig> options) : IJwtTokenService
{
    private readonly JwtTokenConfig _jwtConfig = options.Value;

    public (string token, DateTime expirationTime) CreateToken(ApplicationUser user, IEnumerable<Claim>? claims = null)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationTime);
        var jwtToken = CreateJwtToken(CreateClaims(user, expirationTime, claims), CreateSigningCredentials(), expirationTime);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return (token, expirationTime);
    }

    private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expirationTime)
    {
        var token = new JwtSecurityToken(_jwtConfig.Issuer, _jwtConfig.Audience, claims, expires: expirationTime, signingCredentials: credentials);
        return token;
    }

    private Claim[] CreateClaims(ApplicationUser user, DateTime expirationTime, IEnumerable<Claim>? claims = null)
    {
        /// <remarks>
        /// https://datatracker.ietf.org/doc/html/rfc7519#section-4.1
        /// </remarks>
        claims ??= Enumerable.Empty<Claim>();
        var items = claims.ToList();
        items.AddRange(new[] {
            new Claim(JwtRegisteredClaimNames.Sub, _jwtConfig.Subject),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, expirationTime.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Name, user.NormalizedUserName?? string.Empty),
            new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email?? string.Empty),
            new Claim("ProductionTime", DateTime.UtcNow.ToString()),
            new Claim("ExpirationTime", expirationTime.ToString()),
            new Claim("ExpirationMinutes", _jwtConfig.ExpirationTime.ToString()),
        });
        return [.. items];
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
        var securityKey = new SymmetricSecurityKey(secret);
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return credential;
    }
}