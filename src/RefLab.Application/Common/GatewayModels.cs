namespace RefLab.Application.Common;

/// <summary>
/// The outcome of an SMS dispatch attempt.
/// </summary>
public sealed record SmsSendResult(bool Accepted, string? ProviderReference, string? Error)
{
    /// <summary>A successful dispatch.</summary>
    public static SmsSendResult Success(string? providerReference = null) =>
        new(true, providerReference, null);

    /// <summary>A rejected dispatch.</summary>
    public static SmsSendResult Failure(string error) => new(false, null, error);
}

/// <summary>
/// One result line to publish to the patient's web result service.
/// </summary>
public sealed record NatighResultLine(string TestName, string? Value, string? Unit, string? Flag);

/// <summary>
/// The payload published to the patient's web result service.
/// </summary>
public sealed record NatighPublicationRequest(
    string PatientName,
    string? OrderIdentifier,
    IReadOnlyList<NatighResultLine> Results);

/// <summary>
/// The outcome of a web publication attempt.
/// </summary>
public sealed record NatighPublicationResult(bool Published, string? Reference, string? Error)
{
    /// <summary>A successful publication.</summary>
    public static NatighPublicationResult Success(string? reference = null) => new(true, reference, null);

    /// <summary>A rejected publication.</summary>
    public static NatighPublicationResult Failure(string error) => new(false, null, error);
}
