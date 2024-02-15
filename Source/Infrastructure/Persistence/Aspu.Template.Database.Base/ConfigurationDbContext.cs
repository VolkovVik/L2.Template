using Aspu.Template.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Aspu.L2.DAL;

public class ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : DbContext(options)
{
    public DbSet<AppSetting> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
