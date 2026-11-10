using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefLab.Domain.Auditing;
using RefLab.Domain.Farming;
using RefLab.Domain.Microbiology;
using RefLab.Domain.Patients;
using RefLab.Domain.SendOut;
using RefLab.Infrastructure.Persistence.Converters;

namespace RefLab.Infrastructure.Persistence.Configurations;

/// <summary>
/// The mapping of the microbiology, send-out, auditing and farming tables.
/// </summary>
/// <remarks>
/// These tables sit outside the core order pipeline, but they are part of the
/// same laboratory installation and are mapped here so that the initial
/// migration creates the whole schema in one step.
/// </remarks>
internal static class OperationalConfigurations
{
    /// <summary>Maps the antibiotic catalogue.</summary>
    public static void ConfigureAntibiotic(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Antibiotic>(builder =>
        {
            builder.ToTable("Antibiotics");
            builder.HasKey(antibiotic => antibiotic.Id);
            builder.Property(antibiotic => antibiotic.Id).ValueGeneratedNever();

            builder.Property(antibiotic => antibiotic.Symbol).HasMaxLength(ColumnWidths.Short).IsRequired();
            builder.Property(antibiotic => antibiotic.ScientificName).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(antibiotic => antibiotic.CultureType).HasMaxLength(ColumnWidths.Name);
            builder.Property(antibiotic => antibiotic.Sensitivity).HasMaxLength(ColumnWidths.Short);
            builder.Property(antibiotic => antibiotic.SensitivityUnit).HasMaxLength(ColumnWidths.Short);

            var commonNames = builder.Property(antibiotic => antibiotic.CommonNames);

            commonNames.HasConversion(new StringListConverter())
                .HasColumnName("CommonNames")
                .IsRequired();

            commonNames.Metadata.SetValueComparer(PersistenceComparers.StringList);

            builder.HasIndex(antibiotic => antibiotic.Symbol).IsUnique();
        });
    }

    /// <summary>Maps the culture result.</summary>
    public static void ConfigureCultureResult(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<CultureResult>(builder =>
        {
            builder.ToTable("CultureResults");
            builder.HasKey(result => result.Id);
            builder.Property(result => result.Id).ValueGeneratedNever();

            builder.Property(result => result.PatientId)
                .HasConversion(new PatientIdConverter())
                .IsRequired();

            builder.Property(result => result.CultureId).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(result => result.Organism).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(result => result.SecondOrganism).HasMaxLength(ColumnWidths.Name);
            builder.Property(result => result.Condition).HasMaxLength(ColumnWidths.Description);
            builder.Property(result => result.Colony).HasMaxLength(ColumnWidths.Description);
            builder.Property(result => result.Reactions).HasMaxLength(ColumnWidths.Note);

            var susceptibilities = builder.Property(result => result.Susceptibilities);

            susceptibilities.HasConversion(new SusceptibilityListConverter())
                .HasColumnName("Susceptibilities")
                .IsRequired();

            susceptibilities.Metadata.SetValueComparer(PersistenceComparers.SusceptibilityList);

            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(result => result.PatientId)
                .HasPrincipalKey(patient => patient.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(result => result.PatientId);
        });
    }

    /// <summary>Maps the send-out request.</summary>
    public static void ConfigureSendOutRequest(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<SendOutRequest>(builder =>
        {
            builder.ToTable("SendOutRequests");
            builder.HasKey(request => request.Id);
            builder.Property(request => request.Id).ValueGeneratedNever();

            builder.Property(request => request.DestinationLabName).HasMaxLength(ColumnWidths.Name).IsRequired();

            builder.Property(request => request.OutPrice)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.Property(request => request.OutPriceBeforeDiscount)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.HasOne<OrderTest>()
                .WithMany()
                .HasForeignKey(request => request.OrderTestId)
                .HasPrincipalKey(test => test.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(request => request.OrderTestId);
        });
    }

    /// <summary>Maps the sample disposition.</summary>
    public static void ConfigureSampleDisposition(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<SampleDisposition>(builder =>
        {
            builder.ToTable("SampleDispositions");
            builder.HasKey(disposition => disposition.Id);
            builder.Property(disposition => disposition.Id).ValueGeneratedNever();

            builder.Property(disposition => disposition.DestinationLabName).HasMaxLength(ColumnWidths.Name);
            builder.Property(disposition => disposition.RecordedBy).HasMaxLength(ColumnWidths.Name).IsRequired();

            builder.HasOne<OrderTest>()
                .WithMany()
                .HasForeignKey(disposition => disposition.OrderTestId)
                .HasPrincipalKey(test => test.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(disposition => disposition.OrderTestId);
        });
    }

    /// <summary>Maps the print audit trail.</summary>
    public static void ConfigurePrintAudit(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<PrintAudit>(builder =>
        {
            builder.ToTable("PrintAudits");
            builder.HasKey(audit => audit.Id);
            builder.Property(audit => audit.Id).ValueGeneratedNever();

            builder.Property(audit => audit.Operator).HasMaxLength(ColumnWidths.Name).IsRequired();

            builder.HasOne<OrderTest>()
                .WithMany()
                .HasForeignKey(audit => audit.OrderTestId)
                .HasPrincipalKey(test => test.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(audit => audit.OrderTestId);
            builder.HasIndex(audit => audit.PrintedAt);
        });
    }

    /// <summary>Maps the attendance entry.</summary>
    public static void ConfigureAttendanceEntry(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<AttendanceEntry>(builder =>
        {
            builder.ToTable("AttendanceEntries");
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).ValueGeneratedNever();

            builder.Property(entry => entry.OperatorName).HasMaxLength(ColumnWidths.Name).IsRequired();

            builder.HasIndex(entry => entry.OperatorId);
            builder.HasIndex(entry => entry.OccurredAt);
        });
    }

    /// <summary>Maps the farm.</summary>
    public static void ConfigureFarm(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Farm>(builder =>
        {
            builder.ToTable("Farms");
            builder.HasKey(farm => farm.Id);
            builder.Property(farm => farm.Id).ValueGeneratedNever();

            builder.Property(farm => farm.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(farm => farm.Address).HasMaxLength(ColumnWidths.Description);
            builder.Property(farm => farm.Phone).HasMaxLength(ColumnWidths.Short);
            builder.Property(farm => farm.SecondaryPhone).HasMaxLength(ColumnWidths.Short);
            builder.Property(farm => farm.Note).HasMaxLength(ColumnWidths.Note);

            builder.HasIndex(farm => farm.Name).IsUnique();
        });
    }

    /// <summary>Maps the farm transaction.</summary>
    public static void ConfigureFarmTransaction(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<FarmTransaction>(builder =>
        {
            builder.ToTable("FarmTransactions");
            builder.HasKey(transaction => transaction.Id);
            builder.Property(transaction => transaction.Id).ValueGeneratedNever();

            builder.Property(transaction => transaction.FarmName).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(transaction => transaction.Note).HasMaxLength(ColumnWidths.Note);

            builder.Property(transaction => transaction.IssuedAmount)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.Property(transaction => transaction.ReceivedAmount)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.HasOne<Farm>()
                .WithMany()
                .HasForeignKey(transaction => transaction.FarmId)
                .HasPrincipalKey(farm => farm.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(transaction => transaction.FarmId);
        });
    }
}
