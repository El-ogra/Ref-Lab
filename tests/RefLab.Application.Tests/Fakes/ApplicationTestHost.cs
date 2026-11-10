using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Abstractions;

namespace RefLab.Application.Tests.Fakes;

/// <summary>
/// Builds the Application layer over the in-memory fakes and exposes it
/// through the real MediatR pipeline.
/// </summary>
/// <remarks>
/// Running the whole pipeline — validation, authorization, domain-exception
/// translation and logging — means the tests exercise the application exactly
/// as the presentation layer will call it.
/// </remarks>
public sealed class ApplicationTestHost : IDisposable
{
    /// <summary>Initialises a new host.</summary>
    /// <param name="grantAllPermissions">
    /// Whether the signed-in operator holds every permission.
    /// </param>
    public ApplicationTestHost(bool grantAllPermissions = true)
    {
        Db = new FakeAppDbContext();
        UnitOfWork = new FakeUnitOfWork(Db);
        User = new FakeCurrentUser { GrantAllPermissions = grantAllPermissions };
        Clock = new FakeClock();
        PasswordHasher = new FakePasswordHasher();
        Renderer = new FakeReportRenderer();
        Pdf = new FakePdfExporter();
        Sequence = new FakePrintSequenceGenerator();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<IAppDbContext>(Db);
        services.AddSingleton<IUnitOfWork>(UnitOfWork);
        services.AddSingleton(User);
        services.AddSingleton<ICurrentUser>(User);
        services.AddSingleton(Clock);
        services.AddSingleton<IClock>(Clock);
        services.AddSingleton(PasswordHasher);
        services.AddSingleton<IPasswordHasher>(PasswordHasher);
        services.AddSingleton(Renderer);
        services.AddSingleton<IReportRenderer>(Renderer);
        services.AddSingleton(Pdf);
        services.AddSingleton<IPdfExporter>(Pdf);
        services.AddSingleton(Sequence);
        services.AddSingleton<IPrintSequenceGenerator>(Sequence);

        services.AddApplicationLayer();

        Services = services.BuildServiceProvider();
        Sender = Services.GetRequiredService<ISender>();
    }

    /// <summary>The in-memory store.</summary>
    public FakeAppDbContext Db { get; }

    /// <summary>The in-memory unit of work.</summary>
    public FakeUnitOfWork UnitOfWork { get; }

    /// <summary>The signed-in operator.</summary>
    public FakeCurrentUser User { get; }

    /// <summary>The frozen clock.</summary>
    public FakeClock Clock { get; }

    /// <summary>The password hasher.</summary>
    public FakePasswordHasher PasswordHasher { get; }

    /// <summary>The report renderer.</summary>
    public FakeReportRenderer Renderer { get; }

    /// <summary>The PDF exporter.</summary>
    public FakePdfExporter Pdf { get; }

    /// <summary>The print sequence generator.</summary>
    public FakePrintSequenceGenerator Sequence { get; }

    /// <summary>The built service provider.</summary>
    public IServiceProvider Services { get; }

    /// <summary>The MediatR sender running the full pipeline.</summary>
    public ISender Sender { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
