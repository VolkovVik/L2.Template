using Microsoft.AspNetCore.Identity;

namespace Aspu.Template.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
    public string? Role { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? Time { get; set; } = DateTime.UtcNow;
}