using Aspu.Template.Domain.Entities;
using Aspu.Template.Domain.Extensions;
using AutoMapper;

namespace Aspu.Template.Domain.Dto;

public class ApplicationUserDto : IMapFrom<ApplicationUser>
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? Time { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(item => item.Role, expression => expression.MapFrom(x => BaseExtensions.GetValue(x.Role, "undefined")));
    }
}