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

[LoggerOperation("Create user")]
public record CreateUserCommand : BaseUserCommand, IRequest<Result> { }

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.Role).IsInEnum().WithMessage("Please enter a valid role value");
        RuleFor(v => v.Login).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid login");
        RuleFor(v => v.Password).Must(x => !string.IsNullOrWhiteSpace(x)).Length(3, 100).WithMessage("Please enter a valid password");
    }
}

public class CreateUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<CreateUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Login!);
        if (user != null)
            return Result.Error($"User [{request.Login}] already exists!");

        user = new()
        {
            Name = request.Login,
            UserName = request.Login,
            Role = request.Role.ToFriendlyString(),
            SecurityStamp = Guid.NewGuid().ToString()
        };
        var result = await _userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
            return Result.Error($"User [{request.Login}] creation failed! Please check user details and try again.");

        var claims = new[] {
            new Claim(typeof(AppRole).Name, request.Role.ToFriendlyString())
        };
        result = await _userManager.AddClaimsAsync(user, claims);
        if (!result.Succeeded)
            return Result.Error($"User claims [{request.Login}] creation failed! Please check user details and try again.");

        return Result.Ok($"User [{request.Login}] created successfully!");
    }
}