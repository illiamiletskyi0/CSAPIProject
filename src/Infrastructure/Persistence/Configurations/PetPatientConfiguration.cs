using Domain.PetPatients;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetPatientConfiguration : IEntityTypeConfiguration<PetPatient>
{
    public void Configure(EntityTypeBuilder<PetPatient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MicrochipId)
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.HasIndex(x => x.MicrochipId)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.Property(x => x.Species)
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.AgeYears)
            .IsRequired();

        builder.Property(x => x.IsVaccinated)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}