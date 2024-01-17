using Aspu.Template.Application.Application.Users.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aspu.L2.Application.Application.Users.Commands;

[LoggerOperation("Delete user")]
public record DeleteUserCommand : BaseUserCommand, IRequest<Result> { }

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(v => v.Login).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid login");
    }
}

public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Login!);
        if (user == null)
            return Result.Error($"User [{request.Login}] isn't exist!");

        var claims = await _userManager.GetClaimsAsync(user!);
        var result = await _userManager.RemoveClaimsAsync(user, claims);
        if (!result.Succeeded)
            return Result.Error($"User [{request.Login}] role delete failed! Please check user details and try again.");

        result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? Result.Ok($"User [{request.Login}] deleted successfully!")
            : Result.Error($"User [{request.Login}] deleted failed! Please check user details and try again.");
    }
}