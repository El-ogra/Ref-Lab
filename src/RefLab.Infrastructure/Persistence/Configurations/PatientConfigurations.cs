using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefLab.Domain.Patients;
using RefLab.Infrastructure.Persistence.Converters;

namespace RefLab.Infrastructure.Persistence.Configurations;

/// <summary>
/// The mapping of the patient tables.
/// </summary>
/// <remarks>
/// <para>
/// The patient is the laboratory's master record. The order, the history
/// sheet, the picture and the culture result all reference it, so the
/// identifier is mapped once as a GUID and every foreign key uses the same
/// conversion.
/// </para>
/// <para>
/// The order test carries a frozen copy of the reference range — the
/// <c>normalrang</c> column of the legacy <c>patienttest</c> table — so that
/// the printed report shows the policy that was in force when the analyte was
/// measured. That copy is modelled as an owned value object rather than as a
/// second reference to the range table.
/// </para>
/// </remarks>
internal static class PatientConfigurations
{
    /// <summary>Maps the patient master record.</summary>
    public static void ConfigurePatient(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Patient>(builder =>
        {
            builder.ToTable("Patients");
            builder.HasKey(patient => patient.Id);

            builder.Property(patient => patient.Id)
                .HasConversion(new PatientIdConverter())
                .ValueGeneratedNever();

            builder.Property(patient => patient.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(patient => patient.Address).HasMaxLength(ColumnWidths.Description);
            builder.Property(patient => patient.Phone).HasMaxLength(ColumnWidths.Short);
            builder.Property(patient => patient.Mobile).HasMaxLength(ColumnWidths.Short);
            builder.Property(patient => patient.Fax).HasMaxLength(ColumnWidths.Short);
            builder.Property(patient => patient.Email).HasMaxLength(ColumnWidths.Name);
            builder.Property(patient => patient.ReferringPhysicianCode).HasMaxLength(ColumnWidths.Short);
            builder.Property(patient => patient.ReceptionNote).HasMaxLength(ColumnWidths.Note);
            builder.Property(patient => patient.AdditionalInfo).HasMaxLength(ColumnWidths.Note);
            builder.Property(patient => patient.AdditionalInfo2).HasMaxLength(ColumnWidths.Note);

            builder.Property(patient => patient.Age)
                .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);

            builder.HasIndex(patient => patient.Name);
        });
    }

    /// <summary>Maps the patient order — the laboratory visit header.</summary>
    public static void ConfigurePatientOrder(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<PatientOrder>(builder =>
        {
            builder.ToTable("PatientOrders");
            builder.HasKey(order => order.Id);

            builder.Property(order => order.Id).ValueGeneratedNever();

            builder.Property(order => order.PatientId)
                .HasConversion(new PatientIdConverter())
                .IsRequired();

            builder.Property(order => order.TimeIn).HasMaxLength(ColumnWidths.Short);
            builder.Property(order => order.PriceMethod).HasMaxLength(ColumnWidths.Short);
            builder.Property(order => order.CurrencyCode).HasMaxLength(ColumnWidths.Short).IsRequired();
            builder.Property(order => order.TreatingPhysicianCode).HasMaxLength(ColumnWidths.Short);
            builder.Property(order => order.Barcode).HasMaxLength(ColumnWidths.Short);
            builder.Property(order => order.ReceptionNote).HasMaxLength(ColumnWidths.Note);
            builder.Property(order => order.AdditionalInfo).HasMaxLength(ColumnWidths.Note);
            builder.Property(order => order.AdditionalInfo2).HasMaxLength(ColumnWidths.Note);
            builder.Property(order => order.CreatedBy).HasMaxLength(ColumnWidths.Name);

            builder.Property(order => order.Discount100)
                .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);

            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(order => order.PatientId)
                .HasPrincipalKey(patient => patient.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(order => order.Tests)
                .WithOne()
                .HasForeignKey(test => test.OrderId);

            builder.Navigation(order => order.Tests)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(order => order.PatientId);
            builder.HasIndex(order => order.RequestDate);

            builder.OwnsOne(order => order.LegacyIdentifier, identifier =>
                identifier.ConfigureOrderIdentifier("LegacyIdentifier"));

            builder.OwnsOne(order => order.TotalPrice, money =>
                money.ConfigureMoney("TotalPrice"));

            builder.OwnsOne(order => order.TotalPriceAfterDiscount, money =>
                money.ConfigureMoney("TotalPriceAfterDiscount"));

            builder.OwnsOne(order => order.CommissionBaseAmount, money =>
                money.ConfigureMoney("CommissionBaseAmount"));

            builder.OwnsOne(order => order.Discount, percentage =>
                percentage.ConfigurePercentage("Discount"));
        });
    }

    /// <summary>Maps the requested test of an order.</summary>
    public static void ConfigureOrderTest(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<OrderTest>(builder =>
        {
            builder.ToTable("OrderTests");
            builder.HasKey(test => test.Id);
            builder.Property(test => test.Id).ValueGeneratedNever();

            builder.Property(test => test.TestSimpleName).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(test => test.TestAbbreviation).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.TestName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.HistoryName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.TestUnit).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.ResultCasePrint).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.SampleTakenOutsideLab).IsRequired();
            builder.Property(test => test.OtherLabName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.Timeout).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.Comment).HasMaxLength(ColumnWidths.Note);
            builder.Property(test => test.LogGroup).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.CreatedBy).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.EditedBy).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.PreviousUser).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.PrintedBy).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.HandedOverBy).HasMaxLength(ColumnWidths.Name);

            builder.Property(test => test.OutPrice)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.Property(test => test.OutPriceBeforeDiscount)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            var additionalResults = builder.Property(test => test.AdditionalResults);

            additionalResults.HasConversion(new NullableStringListConverter())
                .HasColumnName("AdditionalResults")
                .IsRequired();

            additionalResults.Metadata.SetValueComparer(PersistenceComparers.NullableStringList);

            builder.HasOne<PatientOrder>()
                .WithMany()
                .HasForeignKey(test => test.OrderId)
                .HasPrincipalKey(order => order.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(test => test.OrderId);
            builder.HasIndex(test => test.TestCode);

            builder.OwnsOne(test => test.Result, result =>
                result.ConfigureResultValue("Result"));

            builder.OwnsOne(test => test.Price, money =>
                money.ConfigureMoney("Price"));

            builder.OwnsOne(test => test.ReferenceSnapshot, snapshot =>
            {
                snapshot.Property(value => value.TestSimpleName)
                    .HasMaxLength(ColumnWidths.Name)
                    .HasColumnName("ReferenceSnapshot_TestSimpleName")
                    .IsRequired();

                snapshot.Property(value => value.SexScope)
                    .HasColumnName("ReferenceSnapshot_SexScope");

                snapshot.Property(value => value.CapturedAt)
                    .HasColumnName("ReferenceSnapshot_CapturedAt");

                snapshot.OwnsOne(value => value.AgeBand, ageBand =>
                    ageBand.ConfigureAgeBand("ReferenceSnapshot_AgeBand"));

                snapshot.OwnsOne(value => value.Interval, interval =>
                    interval.ConfigureReferenceInterval("ReferenceSnapshot_Interval"));
            });
        });
    }

    /// <summary>Maps the patient history sheet.</summary>
    public static void ConfigurePatientHistory(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<PatientHistory>(builder =>
        {
            builder.ToTable("PatientHistories");
            builder.HasKey(history => new { history.PatientId, history.Selector });

            builder.Property(history => history.PatientId)
                .HasConversion(new PatientIdConverter())
                .IsRequired();

            builder.Property(history => history.Selector).ValueGeneratedNever();

            var slots = builder.Property(history => history.Slots);

            slots.HasConversion(new StringArrayConverter())
                .HasColumnName("Slots")
                .IsRequired();

            slots.Metadata.SetValueComparer(PersistenceComparers.StringArray);

            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(history => history.PatientId)
                .HasPrincipalKey(patient => patient.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>Maps the patient picture.</summary>
    public static void ConfigurePatientPicture(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<PatientPicture>(builder =>
        {
            builder.ToTable("PatientPictures");
            builder.HasKey(picture => picture.Id);
            builder.Property(picture => picture.Id).ValueGeneratedNever();

            builder.Property(picture => picture.PatientId)
                .HasConversion(new PatientIdConverter())
                .IsRequired();

            builder.Property(picture => picture.ImagePath).HasMaxLength(ColumnWidths.Note).IsRequired();
            builder.Property(picture => picture.Caption).HasMaxLength(ColumnWidths.Description);

            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(picture => picture.PatientId)
                .HasPrincipalKey(patient => patient.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(picture => picture.PatientId);
        });
    }
}
