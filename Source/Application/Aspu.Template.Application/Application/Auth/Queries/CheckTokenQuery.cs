using Aspu.L2.Application.Application.Auth.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Configuration;
using Aspu.Template.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Aspu.Template.Application.Application.Auth.Queries;

[LoggerOperation("Check token")]
public record CheckTokenQuery : IRequest<Result<CheckTokenQueryResponce>>
{
    public string? Token { get; set; }
}

public class CheckTokenQueryValidator : AbstractValidator<CheckTokenQuery>
{
    public CheckTokenQueryValidator()
    {
        RuleFor(v => v.Token).NotNull().NotEmpty().WithMessage("Please enter a valid token");
    }
}

public record CheckTokenQueryResponce : AuthQueryResponce
{
    [LoggerProperty("Valid")]
    public bool IsValid { get; set; }
}

public class CheckTokenQueryHandler(IOptions<JwtTokenConfig> options, UserManager<ApplicationUser> userManager) : IRequestHandler<CheckTokenQuery, Result<CheckTokenQueryResponce>>
{
    private readonly JwtTokenConfig _jwtTokenConfig = options.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<CheckTokenQueryResponce>> Handle(CheckTokenQuery request, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();
        IPrincipal principal = tokenHandler.ValidateToken(request.Token, validationParameters, out var validatedToken);
        if (principal?.Identity?.IsAuthenticated != true)
            return Result<CheckTokenQueryResponce>.Ok(new CheckTokenQueryResponce());

        var userId = ((JwtSecurityToken)validatedToken).Payload[ClaimTypes.NameIdentifier].ToString();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<CheckTokenQueryResponce>.Ok(new CheckTokenQueryResponce());

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<CheckTokenQueryResponce>.Ok(new CheckTokenQueryResponce());

        var tokenDescriptor = tokenHandler.ReadJwtToken(request.Token);
        var expirationTime = tokenDescriptor.ValidTo;
        var responce = new CheckTokenQueryResponce { IsValid = expirationTime > DateTime.UtcNow, ExpirationTime = expirationTime };
        return Result<CheckTokenQueryResponce>.Ok(responce);
    }

    private TokenValidationParameters GetValidationParameters() => new()
    {
        ValidateLifetime = false,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidIssuer = _jwtTokenConfig.Issuer,
        ValidAudience = _jwtTokenConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret))
    };
}