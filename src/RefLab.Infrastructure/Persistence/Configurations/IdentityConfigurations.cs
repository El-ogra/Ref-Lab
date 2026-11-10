using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefLab.Domain.Configuration;
using RefLab.Domain.Identity;
using RefLab.Infrastructure.Persistence.Converters;

namespace RefLab.Infrastructure.Persistence.Configurations;

/// <summary>
/// The mapping of the identity tables.
/// </summary>
/// <remarks>
/// The operator table is the single-site successor of the legacy operator
/// table. The legacy table carried a branch column; the laboratory is now
/// single-site, so the column is not reproduced here.
/// </remarks>
internal static class IdentityConfigurations
{
    /// <summary>Maps the operator account.</summary>
    public static void ConfigureOperator(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Operator>(builder =>
        {
            builder.ToTable("Operators");
            builder.HasKey(@operator => @operator.Id);
            builder.Property(@operator => @operator.Id).ValueGeneratedNever();

            builder.Property(@operator => @operator.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(@operator => @operator.RoleCaption).HasMaxLength(ColumnWidths.Name);
            builder.Property(@operator => @operator.PasswordHash).HasMaxLength(ColumnWidths.Note).IsRequired();
            builder.Property(@operator => @operator.SecurityWordHash).HasMaxLength(ColumnWidths.Note);

            var permissions = builder.Property(@operator => @operator.Permissions);

            permissions.HasConversion(new PermissionCollectionConverter())
                .HasColumnName("Permissions")
                .IsRequired();

            permissions.Metadata.SetValueComparer(PersistenceComparers.PermissionCollection);

            builder.HasIndex(@operator => @operator.Name).IsUnique();
        });
    }

    /// <summary>Maps the laboratory settings.</summary>
    public static void ConfigureLaboratorySettings(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<LaboratorySettings>(builder =>
        {
            builder.ToTable("LaboratorySettings");
            builder.HasKey(settings => settings.Id);
            builder.Property(settings => settings.Id).ValueGeneratedNever();

            builder.Property(settings => settings.PaperSize).IsRequired();
            builder.Property(settings => settings.ReportSequenceStart).IsRequired();

            builder.Property(settings => settings.GlobalHeaderText).HasMaxLength(ColumnWidths.Note);
            builder.Property(settings => settings.GlobalHeaderSecondaryText).HasMaxLength(ColumnWidths.Note);
            builder.Property(settings => settings.BackgroundColour).HasMaxLength(ColumnWidths.Short);
            builder.Property(settings => settings.DefaultAccountType).HasMaxLength(ColumnWidths.Short);
            builder.Property(settings => settings.ConnectionReceiptHeader).HasMaxLength(ColumnWidths.Note);
            builder.Property(settings => settings.ConnectionReceiptFooter).HasMaxLength(ColumnWidths.Note);

            builder.Property(settings => settings.LeftMargin).HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);
            builder.Property(settings => settings.RightMargin).HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);
            builder.Property(settings => settings.TopMargin).HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);
            builder.Property(settings => settings.BottomMargin).HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);

            var printers = builder.Property(settings => settings.Printers);

            printers.HasConversion(new PrinterRoleMapConverter())
                .HasColumnName("Printers")
                .IsRequired();

            printers.Metadata.SetValueComparer(PersistenceComparers.PrinterRoleMap);
        });
    }
}
