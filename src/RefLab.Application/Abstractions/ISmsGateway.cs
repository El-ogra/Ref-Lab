using RefLab.Application.Common;

namespace RefLab.Application.Abstractions;

/// <summary>
/// Sends an outbound SMS notification.
/// </summary>
/// <remarks>
/// The gateway provider, protocol and delivery-status handling are not
/// established in the legacy evidence, so this abstraction stays deliberately
/// thin and the provider is an infrastructure concern.
/// </remarks>
public interface ISmsGateway
{
    /// <summary>Sends a message to the given mobile number.</summary>
    Task<SmsSendResult> SendAsync(
        string mobileNumber,
        string message,
        CancellationToken cancellationToken = default);
}
