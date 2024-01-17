using Aspu.Template.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Aspu.Template.Application.Application.Users.Base;

public class UserQueryBaseHandler(IMapper mapper, UserManager<ApplicationUser> userManager)
{
    protected readonly IMapper _mapper = mapper;
    protected readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<string> GetUserClaimAsync(ApplicationUser user, string type)
    {
        if (user == null) return string.Empty;

        var claims = await _userManager.GetClaimsAsync(user!);
        var value = claims.FirstOrDefault(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        return value;
    }
}
