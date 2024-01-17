using Aspu.L2.DAL;
using Aspu.L2.DAL.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspu.Template.Database.MsSql;

public class DatabaseHelper : IDatabaseHelper
{
    /// <remarks>
    /// Collations and Case Sensitivity
    /// https://learn.microsoft.com/en-us/ef/core/miscellaneous/collations-and-case-sensitivity
    /// Collation and Unicode support in MsSQL
    /// https://learn.microsoft.com/en-us/sql/relational-databases/collations/collation-and-unicode-support?view=sql-server-ver16
    /// CollationName
    /// private const string DefaultCollationName = "SQL_Latin1_General_CP1_CI_AS";
    /// private const string EnglishCaseSensitivityCollationName = "Latin1_General_CS_AS";
    /// private const string CyrillicCaseSensitivityCollationName = "Cyrillic_General_CS_AS";
    /// </remarks>

    public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }

    public void Migrate(AppDbContext appDbContext) { }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        ///modelBuilder.UseCollation(CyrillicCaseSensitivityCollationName);
        ///modelBuilder.Entity<Km>().Property(c => c.Code).UseCollation(EnglishCaseSensitivityCollationName);
    }
}
