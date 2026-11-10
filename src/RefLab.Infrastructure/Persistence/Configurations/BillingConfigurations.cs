using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefLab.Domain.Billing;
using RefLab.Domain.Patients;

namespace RefLab.Infrastructure.Persistence.Configurations;

/// <summary>
/// The mapping of the billing tables.
/// </summary>
internal static class BillingConfigurations
{
    /// <summary>Maps the payment receipt.</summary>
    public static void ConfigurePayment(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Payment>(builder =>
        {
            builder.ToTable("Payments");
            builder.HasKey(payment => payment.Id);
            builder.Property(payment => payment.Id).ValueGeneratedNever();

            builder.Property(payment => payment.LegacyOrderIdentifier).HasMaxLength(ColumnWidths.OrderIdentifier);
            builder.Property(payment => payment.ReferringPartyCode).HasMaxLength(ColumnWidths.Short);
            builder.Property(payment => payment.TestSimpleName).HasMaxLength(ColumnWidths.Name);
            builder.Property(payment => payment.TreatingPhysicianCode).HasMaxLength(ColumnWidths.Short);
            builder.Property(payment => payment.Comment).HasMaxLength(ColumnWidths.Note);

            builder.HasOne<PatientOrder>()
                .WithMany()
                .HasForeignKey(payment => payment.OrderId)
                .HasPrincipalKey(order => order.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(payment => payment.OrderId);
            builder.HasIndex(payment => payment.PaidDate);

            builder.OwnsOne(payment => payment.Amount, money =>
                money.ConfigureMoney("Amount"));
        });
    }

    /// <summary>Maps the price list.</summary>
    public static void ConfigurePriceList(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<PriceList>(builder =>
        {
            builder.ToTable("PriceLists");
            builder.HasKey(price => price.Id);
            builder.Property(price => price.Id).ValueGeneratedNever();

            builder.Property(price => price.TestSimpleName).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(price => price.PriceListName).HasMaxLength(ColumnWidths.Name).IsRequired();

            builder.Property(price => price.Price)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.HasIndex(price => new { price.TestSimpleName, price.PriceListName });
        });
    }

    /// <summary>Maps the referring party.</summary>
    public static void ConfigureReferringParty(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ReferringParty>(builder =>
        {
            builder.ToTable("ReferringParties");
            builder.HasKey(party => party.Id);
            builder.Property(party => party.Id).ValueGeneratedNever();

            builder.Property(party => party.Code).HasMaxLength(ColumnWidths.Short).IsRequired();
            builder.Property(party => party.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(party => party.ContactPersonCode).HasMaxLength(ColumnWidths.Short);
            builder.Property(party => party.ContactPersonName).HasMaxLength(ColumnWidths.Name);
            builder.Property(party => party.City).HasMaxLength(ColumnWidths.Name);
            builder.Property(party => party.Address).HasMaxLength(ColumnWidths.Description);
            builder.Property(party => party.Phone).HasMaxLength(ColumnWidths.Short);
            builder.Property(party => party.Mobile).HasMaxLength(ColumnWidths.Short);
            builder.Property(party => party.Fax).HasMaxLength(ColumnWidths.Short);
            builder.Property(party => party.Email).HasMaxLength(ColumnWidths.Name);
            builder.Property(party => party.Website).HasMaxLength(ColumnWidths.Name);
            builder.Property(party => party.PriceMethod).HasMaxLength(ColumnWidths.Short);
            builder.Property(party => party.ContactPhone).HasMaxLength(ColumnWidths.Short);
            builder.Property(party => party.TreatingPhysicianCode).HasMaxLength(ColumnWidths.Short);

            builder.Property(party => party.SpecialLimit)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.HasIndex(party => party.Code).IsUnique();

            builder.OwnsOne(party => party.CommissionRate, percentage =>
                percentage.ConfigurePercentage("CommissionRate"));
        });
    }

    /// <summary>Maps the treating physician.</summary>
    public static void ConfigureTreatingPhysician(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<TreatingPhysician>(builder =>
        {
            builder.ToTable("TreatingPhysicians");
            builder.HasKey(physician => physician.Id);
            builder.Property(physician => physician.Id).ValueGeneratedNever();

            builder.Property(physician => physician.Code).HasMaxLength(ColumnWidths.Short).IsRequired();
            builder.Property(physician => physician.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(physician => physician.Phone).HasMaxLength(ColumnWidths.Short);

            builder.HasIndex(physician => physician.Code).IsUnique();
        });
    }
}
