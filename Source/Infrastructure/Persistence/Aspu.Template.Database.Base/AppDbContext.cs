using Aspu.L2.DAL.Base.Interfaces;
using Aspu.Template.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Aspu.L2.DAL;

public class AppDbContext(DbContextOptions<AppDbContext> options, IDatabaseHelper helper) : IdentityDbContext<ApplicationUser>(options)
{
    private readonly IDatabaseHelper _helper = helper;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        _helper.OnModelCreating(builder);
        /// new DbInitialize(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        _helper.ConfigureConventions(configurationBuilder);
    }
}
