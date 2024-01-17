using Aspu.Template.Application.Application.Users.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Dto;
using Aspu.Template.Domain.Entities;
using Aspu.Template.Domain.Enums;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aspu.L2.Application.Application.Users.Queries;

[LoggerOperation("Get user by login")]
public record GetUserByLoginQuery : IRequest<Result<ApplicationUserDto?>>
{
    [LoggerProperty("Login")]
    public string? Login { get; set; }

    public GetUserByLoginQuery(string? login)
    {
        Login = login;
    }
}

public class GetUserByLoginQueryValidator : AbstractValidator<GetUserByLoginQuery>
{
    public GetUserByLoginQueryValidator()
    {
        RuleFor(v => v.Login).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Please enter a valid login");
    }
}

public class GetUserByLoginQueryHandler(IMapper mapper, UserManager<ApplicationUser> userManager) : UserQueryBaseHandler(mapper, userManager), IRequestHandler<GetUserByLoginQuery, Result<ApplicationUserDto?>>
{
    public async Task<Result<ApplicationUserDto?>> Handle(GetUserByLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Login!);
        if (user == null)
            return Result<ApplicationUserDto?>.Error(default, $"User {request.Login} isn't exists!");

        user.Role = await GetUserClaimAsync(user, typeof(AppRole).Name);
        var item = _mapper.Map<ApplicationUserDto>(user);
        return Result<ApplicationUserDto?>.Ok(item);
    }
}
