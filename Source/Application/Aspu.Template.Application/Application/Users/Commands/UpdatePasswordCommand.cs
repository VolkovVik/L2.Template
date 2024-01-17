using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aspu.L2.Application.Application.Users.Commands;

[LoggerOperation("Update user password")]
public record UpdatePasswordCommand : IRequest<Result>
{
    [LoggerProperty("Login")]
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? NewPassword { get; set; }
}

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(v => v.Login).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid login");
        RuleFor(v => v.Password).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid old password");
        RuleFor(v => v.NewPassword).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid new password");
    }
}

public class UpdatePasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<UpdatePasswordCommand, Result>
{
    public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.Login!);
        if (user == null)
            return Result.Error($"User [{request.Login}] isn't exist!");

        var result = await userManager.ChangePasswordAsync(user, request.Password!, request.NewPassword!);
        return result.Succeeded
            ? Result.Ok($"User [{request.Login}] password updated successfully!")
            : Result.Error($"User [{request.Login}] password updated failed! Please check user details and try again.");
    }
}