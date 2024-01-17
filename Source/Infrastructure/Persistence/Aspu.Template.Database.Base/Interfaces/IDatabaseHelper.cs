using Microsoft.EntityFrameworkCore;

namespace Aspu.L2.DAL.Base.Interfaces;

public interface IDatabaseHelper
{
    void Migrate(AppDbContext appDbContext);
    void OnModelCreating(ModelBuilder modelBuilder);
    void ConfigureConventions(ModelConfigurationBuilder configurationBuilder);
}
