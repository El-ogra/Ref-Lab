using Microsoft.EntityFrameworkCore;
using RefLab.Application.Abstractions;
using RefLab.Domain.Auditing;
using RefLab.Domain.Billing;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Configuration;
using RefLab.Domain.Farming;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;
using RefLab.Domain.Patients;
using RefLab.Domain.SendOut;
using RefLab.Infrastructure.Persistence.Configurations;

namespace RefLab.Infrastructure.Persistence;

/// <summary>
/// The Entity Framework Core context of Ref Lab.
/// </summary>
/// <remarks>
/// <para>
/// The context implements the two Application abstractions that describe the
/// persistence store: <see cref="IAppDbContext"/> for reading and
/// <see cref="IUnitOfWork"/> for writing. Both are implemented explicitly, so
/// the Application layer sees only <see cref="IQueryable{T}"/> and never a
/// <c>DbSet&lt;T&gt;</c>.
/// </para>
/// <para>
/// The laboratory is a single-site installation. There is no branch column,
/// no branch table and no per-branch schema: every table in the schema belongs
/// to the one laboratory.
/// </para>
/// </remarks>
public sealed class RefLabDbContext : DbContext, IAppDbContext, IUnitOfWork
{
    /// <summary>
    /// Initialises a new context.
    /// </summary>
    /// <param name="options">The options that select and configure the provider.</param>
    public RefLabDbContext(DbContextOptions<RefLabDbContext> options)
        : base(options)
    {
    }

    /// <summary>The operator accounts.</summary>
    public DbSet<Operator> Operators => Set<Operator>();

    /// <summary>The laboratory settings.</summary>
    public DbSet<LaboratorySettings> LaboratorySettings => Set<LaboratorySettings>();

    /// <summary>The test catalogue.</summary>
    public DbSet<TestDefinition> TestDefinitions => Set<TestDefinition>();

    /// <summary>The reference ranges.</summary>
    public DbSet<ReferenceRange> ReferenceRanges => Set<ReferenceRange>();

    /// <summary>The clinical constants.</summary>
    public DbSet<ClinicalConstant> ClinicalConstants => Set<ClinicalConstant>();

    /// <summary>The clinical sub-groups.</summary>
    public DbSet<ClinicalSubGroup> ClinicalSubGroups => Set<ClinicalSubGroup>();

    /// <summary>The measurement units.</summary>
    public DbSet<TestUnit> TestUnits => Set<TestUnit>();

    /// <summary>The unit conversion factors.</summary>
    public DbSet<ConversionFactor> ConversionFactors => Set<ConversionFactor>();

    /// <summary>The abbreviations.</summary>
    public DbSet<Abbreviation> Abbreviations => Set<Abbreviation>();

    /// <summary>The prothrombin control calibration table.</summary>
    public DbSet<ProthrombinControlCalibration> ProthrombinControlCalibrations =>
        Set<ProthrombinControlCalibration>();

    /// <summary>The patients.</summary>
    public DbSet<Patient> Patients => Set<Patient>();

    /// <summary>The patient orders.</summary>
    public DbSet<PatientOrder> PatientOrders => Set<PatientOrder>();

    /// <summary>The requested tests of the orders.</summary>
    public DbSet<OrderTest> OrderTests => Set<OrderTest>();

    /// <summary>The patient history sheets.</summary>
    public DbSet<PatientHistory> PatientHistories => Set<PatientHistory>();

    /// <summary>The patient pictures.</summary>
    public DbSet<PatientPicture> PatientPictures => Set<PatientPicture>();

    /// <summary>The payments.</summary>
    public DbSet<Payment> Payments => Set<Payment>();

    /// <summary>The price lists.</summary>
    public DbSet<PriceList> PriceLists => Set<PriceList>();

    /// <summary>The referring parties.</summary>
    public DbSet<ReferringParty> ReferringParties => Set<ReferringParty>();

    /// <summary>The treating physicians.</summary>
    public DbSet<TreatingPhysician> TreatingPhysicians => Set<TreatingPhysician>();

    /// <summary>The antibiotic catalogue.</summary>
    public DbSet<Antibiotic> Antibiotics => Set<Antibiotic>();

    /// <summary>The culture results.</summary>
    public DbSet<CultureResult> CultureResults => Set<CultureResult>();

    /// <summary>The send-out requests.</summary>
    public DbSet<SendOutRequest> SendOutRequests => Set<SendOutRequest>();

    /// <summary>The sample dispositions.</summary>
    public DbSet<SampleDisposition> SampleDispositions => Set<SampleDisposition>();

    /// <summary>The print audit trail.</summary>
    public DbSet<PrintAudit> PrintAudits => Set<PrintAudit>();

    /// <summary>The attendance entries.</summary>
    public DbSet<AttendanceEntry> AttendanceEntries => Set<AttendanceEntry>();

    /// <summary>The farms.</summary>
    public DbSet<Farm> Farms => Set<Farm>();

    /// <summary>The farm transactions.</summary>
    public DbSet<FarmTransaction> FarmTransactions => Set<FarmTransaction>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureOperator();
        modelBuilder.ConfigureLaboratorySettings();

        modelBuilder.ConfigureTestDefinition();
        modelBuilder.ConfigureReferenceRange();
        modelBuilder.ConfigureClinicalConstant();
        modelBuilder.ConfigureClinicalSubGroup();
        modelBuilder.ConfigureTestUnit();
        modelBuilder.ConfigureConversionFactor();
        modelBuilder.ConfigureAbbreviation();
        modelBuilder.ConfigureProthrombinControlCalibration();

        modelBuilder.ConfigurePatient();
        modelBuilder.ConfigurePatientOrder();
        modelBuilder.ConfigureOrderTest();
        modelBuilder.ConfigurePatientHistory();
        modelBuilder.ConfigurePatientPicture();

        modelBuilder.ConfigurePayment();
        modelBuilder.ConfigurePriceList();
        modelBuilder.ConfigureReferringParty();
        modelBuilder.ConfigureTreatingPhysician();

        modelBuilder.ConfigureAntibiotic();
        modelBuilder.ConfigureCultureResult();
        modelBuilder.ConfigureSendOutRequest();
        modelBuilder.ConfigureSampleDisposition();
        modelBuilder.ConfigurePrintAudit();
        modelBuilder.ConfigureAttendanceEntry();
        modelBuilder.ConfigureFarm();
        modelBuilder.ConfigureFarmTransaction();
    }

    /// <inheritdoc />
    IQueryable<T> IAppDbContext.Set<T>()
        where T : class =>
        base.Set<T>();

    /// <inheritdoc />
    void IUnitOfWork.Add<T>(T entity)
        where T : class =>
        base.Set<T>().Add(entity);

    /// <inheritdoc />
    void IUnitOfWork.Update<T>(T entity)
        where T : class =>
        base.Set<T>().Update(entity);

    /// <inheritdoc />
    void IUnitOfWork.Remove<T>(T entity)
        where T : class =>
        base.Set<T>().Remove(entity);

    /// <inheritdoc />
    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        base.SaveChangesAsync(cancellationToken);
}
