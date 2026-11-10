using RefLab.Domain.Enums;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Features.SendOut;

/// <summary>
/// A send-out request as presented to the application layer.
/// </summary>
public sealed record SendOutRequestDto(
    Guid Id,
    Guid OrderTestId,
    string DestinationLabName,
    bool IsDispatched,
    bool IsSettled,
    SendOutState State,
    DateTimeOffset? DispatchedAt,
    DateTimeOffset? SettledAt,
    decimal? OutPrice,
    decimal? OutPriceBeforeDiscount)
{
    /// <summary>Projects a send-out request onto its DTO.</summary>
    public static SendOutRequestDto FromDomain(SendOutRequest request) =>
        new(
            request.Id,
            request.OrderTestId,
            request.DestinationLabName,
            request.IsDispatched,
            request.IsSettled,
            request.State,
            request.DispatchedAt,
            request.SettledAt,
            request.OutPrice,
            request.OutPriceBeforeDiscount);
}

/// <summary>
/// A sample disposition as presented to the application layer.
/// </summary>
public sealed record SampleDispositionDto(
    Guid Id,
    Guid OrderTestId,
    SampleDispositionAction Action,
    string? DestinationLabName,
    DateTimeOffset RecordedAt,
    string RecordedBy)
{
    /// <summary>Projects a sample disposition onto its DTO.</summary>
    public static SampleDispositionDto FromDomain(SampleDisposition disposition) =>
        new(
            disposition.Id,
            disposition.OrderTestId,
            disposition.Action,
            disposition.DestinationLabName,
            disposition.RecordedAt,
            disposition.RecordedBy);
}
