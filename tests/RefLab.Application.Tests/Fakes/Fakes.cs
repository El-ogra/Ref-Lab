using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Identity;

namespace RefLab.Application.Tests.Fakes;

/// <summary>
/// An in-memory unit of work over the fake store.
/// </summary>
public sealed class FakeUnitOfWork : IUnitOfWork
{
    private readonly FakeAppDbContext _context;

    /// <summary>Initialises a new unit of work over the supplied store.</summary>
    public FakeUnitOfWork(FakeAppDbContext context)
    {
        _context = context;
    }

    /// <summary>The number of commits performed.</summary>
    public int SaveChangesCallCount => _context.SaveChangesCallCount;

    /// <inheritdoc />
    public void Add<T>(T entity)
        where T : class => _context.AddEntity(entity);

    /// <inheritdoc />
    public void Update<T>(T entity)
        where T : class => _context.UpdateEntity(entity);

    /// <inheritdoc />
    public void Remove<T>(T entity)
        where T : class => _context.RemoveEntity(entity);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_context.RecordSaveChanges());
}

/// <summary>
/// A clock frozen at a known instant.
/// </summary>
public sealed class FakeClock : IClock
{
    /// <summary>The frozen instant.</summary>
    public DateTimeOffset UtcNow { get; set; } = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);
}

/// <summary>
/// The signed-in operator.
/// </summary>
public sealed class FakeCurrentUser : ICurrentUser
{
    /// <summary>The identifier of the signed-in operator.</summary>
    public int? OperatorId { get; set; } = 1;

    /// <summary>The name of the signed-in operator.</summary>
    public string? OperatorName { get; set; } = "lab.tech";

    /// <summary>The permissions the operator holds.</summary>
    public HashSet<Permission> Permissions { get; } = new();

    /// <summary>Whether the operator holds every permission.</summary>
    public bool GrantAllPermissions { get; set; }

    /// <inheritdoc />
    public bool IsAuthenticated => OperatorId.HasValue;

    /// <inheritdoc />
    public bool HasPermission(Permission permission) =>
        GrantAllPermissions || Permissions.Contains(permission);
}

/// <summary>
/// A reversible password hasher for the test suite.
/// </summary>
public sealed class FakePasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string Hash(string password) => "hash:" + password;

    /// <inheritdoc />
    public bool Verify(string password, string hash) => hash == "hash:" + password;
}

/// <summary>
/// A report renderer that records what it was asked to render.
/// </summary>
public sealed class FakeReportRenderer : IReportRenderer
{
    /// <summary>The number of render calls.</summary>
    public int CallCount { get; private set; }

    /// <summary>The last render request.</summary>
    public ReportRenderRequest? LastRequest { get; private set; }

    /// <inheritdoc />
    public ReportDocument RenderLaboratoryReport(ReportRenderRequest request)
    {
        CallCount++;
        LastRequest = request;

        return new ReportDocument(
            "BIOCHEMISTRY REPORT",
            null,
            request.Sections,
            null,
            null);
    }
}

/// <summary>
/// A PDF exporter that records the destination directory.
/// </summary>
public sealed class FakePdfExporter : IPdfExporter
{
    /// <summary>The number of export calls.</summary>
    public int CallCount { get; private set; }

    /// <summary>The last destination directory.</summary>
    public string? LastDestinationDirectory { get; private set; }

    /// <summary>The category reported by the last export.</summary>
    public string LastCategory { get; set; } = "Individual";

    /// <inheritdoc />
    public Task<ExportedDocument> ExportAsync(
        ReportDocument document,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        LastDestinationDirectory = destinationDirectory;

        var fileName = document.Title.Replace(' ', '_') + ".pdf";

        return Task.FromResult(new ExportedDocument(
            fileName,
            Path.Combine(destinationDirectory, fileName),
            new byte[] { 0x25, 0x50, 0x44, 0x46 },
            LastCategory));
    }
}

/// <summary>
/// A print sequence generator that starts at the legacy sequence origin.
/// </summary>
public sealed class FakePrintSequenceGenerator : IPrintSequenceGenerator
{
    private long _next = 1984;

    /// <inheritdoc />
    public long Next() => _next++;
}
