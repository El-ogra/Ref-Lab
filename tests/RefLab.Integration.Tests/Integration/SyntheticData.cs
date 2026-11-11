namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// The synthetic data the integration tests use.
/// </summary>
/// <remarks>
/// Every value the tests write is synthetic. No real patient name, no real
/// identifier and no real result of a real person appears anywhere in the
/// suite. The names are prefixed with <c>P6-</c> so that a row that escapes
/// into a laboratory database is recognisable as test data.
/// </remarks>
public static class SyntheticData
{
    private static int _catalogueId = 50_000;
    private static int _operatorId = 100;

    /// <summary>A unique fragment used to build a synthetic name.</summary>
    /// <returns>A short unique fragment.</returns>
    public static string Unique() => Guid.NewGuid().ToString("N")[..8];

    /// <summary>The name of a synthetic patient.</summary>
    /// <param name="suffix">A unique fragment.</param>
    /// <returns>The synthetic patient name.</returns>
    public static string PatientName(string suffix) => $"P6-Synthetic-Patient-{suffix}";

    /// <summary>
    /// The next identifier for a seeded catalogue row.
    /// </summary>
    /// <remarks>
    /// The identifier is drawn from one counter for the whole test run, so two
    /// tests never collide on the primary key of a shared database.
    /// </remarks>
    /// <returns>A unique catalogue identifier.</returns>
    public static int NextCatalogueId() => Interlocked.Increment(ref _catalogueId);

    /// <summary>
    /// The next identifier for a seeded operator.
    /// </summary>
    /// <returns>A unique operator identifier.</returns>
    public static int NextOperatorId() => Interlocked.Increment(ref _operatorId);

    /// <summary>The password every synthetic operator is created with.</summary>
    /// <remarks>
    /// The password is a test value. It is written to the test database only,
    /// and the database is discarded when the run ends.
    /// </remarks>
    public const string OperatorPassword = "P6-Synthetic!Pass#2026";
}
