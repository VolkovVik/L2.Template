using Aspu.L2.Application.Application.Auth.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Application.Interfaces;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aspu.L2.Application.Application.Auth;

[LoggerOperation("Refrash token")]
public record RefreshTokenQuery : IRequest<Result<RefreshTokenQueryResponse?>> { }

public record RefreshTokenQueryResponse : AuthQueryResponse
{
    public string? Token { get; set; }
    public string? UserName { get; set; }
}

public class RefreshTokenQueryHandler(ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenQuery, Result<RefreshTokenQueryResponse?>>
{
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<RefreshTokenQueryResponse?>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var principal = _currentUserService.Principal;
        if (string.IsNullOrWhiteSpace(principal?.Identity?.Name))
            return Result<RefreshTokenQueryResponse?>.Error(default, $"Principal isn't correct!");

        var user = await _userManager.FindByNameAsync(principal.Identity.Name!);
        if (user == null)
            return Result<RefreshTokenQueryResponse?>.Error(default, $"User {principal.Identity.Name} isn't exists!");

        var claims = await _userManager.GetClaimsAsync(user!);
        var (token, expirationTime) = _jwtTokenService.CreateToken(user!, claims);
        var Response = new RefreshTokenQueryResponse { Token = token, ExpirationTime = expirationTime, UserName = principal.Identity.Name };
        return Result<RefreshTokenQueryResponse?>.Ok(Response);
    }
}