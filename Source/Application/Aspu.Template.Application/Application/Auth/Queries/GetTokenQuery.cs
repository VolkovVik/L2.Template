using Aspu.L2.Application.Application.Auth.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Application.Interfaces;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aspu.Template.Application.Application.Auth.Queries;

[LoggerOperation("Get token")]
public record GetTokenQuery : IRequest<Result<GetTokenQueryResponce?>>
{
    [LoggerProperty("Login")]
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

public class GetTokenQueryValidator : AbstractValidator<GetTokenQuery>
{
    public GetTokenQueryValidator()
    {
        RuleFor(v => v.UserName).NotNull().NotEmpty().MinimumLength(4).MaximumLength(100).WithMessage("Please enter a valid login");
        RuleFor(v => v.Password).NotNull().NotEmpty().MinimumLength(4).MaximumLength(100).WithMessage("Please enter a valid password");
    }
}

public record GetTokenQueryResponce : AuthQueryResponce
{
    public string? Token { get; set; }
    public string? UserName { get; set; }
    public string? Lang { get; set; }
}

public class GetTokenQueryHandler(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService) : IRequestHandler<GetTokenQuery, Result<GetTokenQueryResponce?>>
{
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<GetTokenQueryResponce?>> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName!);
        if (user == null)
            return Result<GetTokenQueryResponce?>.Error(default, $"User {request.UserName} isn't exists!");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user!, request.Password!);
        if (!isPasswordValid)
            return Result<GetTokenQueryResponce?>.Error(default, $"User {request.UserName} with {request.Password} isn't valid!");

        var claims = await _userManager.GetClaimsAsync(user!);
        var (token, expirationTime) = _jwtTokenService.CreateToken(user!, claims);
        var responce = new GetTokenQueryResponce { Token = token, ExpirationTime = expirationTime, UserName = request.UserName };
        return Result<GetTokenQueryResponce?>.Ok(responce);
    }
}