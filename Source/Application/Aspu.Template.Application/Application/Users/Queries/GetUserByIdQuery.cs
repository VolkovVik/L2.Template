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

[LoggerOperation("Get user by ID")]
public record GetUserByIdQuery : IRequest<Result<ApplicationUserDto?>>
{
    [LoggerProperty("ID")]
    public string? Id { get; set; }

    public GetUserByIdQuery(string? id)
    {
        Id = id;
    }
}

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(v => v.Id).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Please enter a valid identificator");
    }
}

public class GetUserByIdQueryHandler(IMapper mapper, UserManager<ApplicationUser> userManager) : UserQueryBaseHandler(mapper, userManager), IRequestHandler<GetUserByIdQuery, Result<ApplicationUserDto?>>
{
    public async Task<Result<ApplicationUserDto?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id!);
        if (user == null)
            return Result<ApplicationUserDto?>.Error(default, $"User {request.Id} isn't exists!");

        user.Role = await GetUserClaimAsync(user, typeof(AppRole).Name);
        var item = _mapper.Map<ApplicationUserDto>(user);
        return Result<ApplicationUserDto?>.Ok(item);
    }
}