using Aspu.Template.Application.Application.Users.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Entities;
using Aspu.Template.Domain.Enums;
using Aspu.Template.Domain.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Aspu.L2.Application.Application.Users.Commands;

[LoggerOperation("Update user login")]
public record UpdateUserCommand : BaseUserCommand, IRequest<Result> { }

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(v => v.Role).IsInEnum().WithMessage("Please enter a valid role value");
        RuleFor(v => v.Login).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid login");
    }
}

public class UpdateUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Login!);
        if (user == null)
            return Result.Error($"User [{request.Login}] isn't exist!");

        var result = await UpdateRoleHandle(request, user);
        if (!result.IsSuccess) return result;

        user.Name = request.Login;
        user.UserName = request.Login;
        var result1 = await _userManager.UpdateAsync(user);
        return result1.Succeeded
            ? Result.Ok($"User [{request.Login}] updated successfully!")
            : Result.Error($"User [{request.Login}] updated failed! Please check user details and try again.");
    }

    public async Task<Result> UpdateRoleHandle(UpdateUserCommand request, ApplicationUser user)
    {
        if (string.Equals(user.Role, request.Role.ToFriendlyString(), StringComparison.OrdinalIgnoreCase))
            return Result.Ok();

        user.Role = request.Role.ToFriendlyString();
        var claims = await _userManager.GetClaimsAsync(user!);
        var oldClaim = claims.FirstOrDefault(x => x.Type == typeof(AppRole).Name);
        if (oldClaim == null)
            return Result.Error($"User [{request.Login}] role isn't found!");

        var result = await _userManager.RemoveClaimAsync(user, oldClaim);
        if (!result.Succeeded)
            return Result.Error($"User [{request.Login}] role delete failed! Please check user details and try again.");

        var newClaim = new Claim(typeof(AppRole).Name, request.Role.ToFriendlyString());
        result = await _userManager.AddClaimAsync(user, newClaim);
        return result.Succeeded
            ? Result.Ok($"User [{request.Login}] role updated successfully!")
            : Result.Error($"User [{request.Login}] role updated failed! Please check user details and try again.");
    }
}