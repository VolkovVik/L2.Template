using Aspu.Template.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspu.Template.Persistence.Base.Configuration;

public class AppSettingConfig : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("AppSettings");
        builder.HasKey(km => km.Key);
        builder.HasIndex(km => km.Key).IsUnique();
        builder.Property(km => km.Key).ValueGeneratedNever();
    }
}
