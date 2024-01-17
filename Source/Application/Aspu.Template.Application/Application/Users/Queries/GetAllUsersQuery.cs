using Aspu.Template.Application.Application.Users.Base;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Dto;
using Aspu.Template.Domain.Entities;
using Aspu.Template.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aspu.L2.Application.Application.Users.Queries;

[LoggerOperation("Get all users")]
public record GetAllUsersQuery : IRequest<Result<List<ApplicationUserDto>>> { }

public class GetAllUsersQueryHandler(IMapper mapper, UserManager<ApplicationUser> userManager) : UserQueryBaseHandler(mapper, userManager), IRequestHandler<GetAllUsersQuery, Result<List<ApplicationUserDto>>>
{
    public async Task<Result<List<ApplicationUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
        foreach (var user in users)
        {
            user.Role = await GetUserClaimAsync(user, typeof(AppRole).Name);
        }
        var items = _mapper.Map<List<ApplicationUserDto>>(users);
        return Result<List<ApplicationUserDto>>.Ok(items);
    }
}