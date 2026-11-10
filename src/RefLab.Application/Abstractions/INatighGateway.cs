using RefLab.Application.Common;

namespace RefLab.Application.Abstractions;

/// <summary>
/// Publishes a laboratory result to the patient's web result service.
/// </summary>
public interface INatighGateway
{
    /// <summary>Publishes the supplied result lines to the patient's account.</summary>
    Task<NatighPublicationResult> PublishAsync(
        NatighPublicationRequest request,
        CancellationToken cancellationToken = default);
}
