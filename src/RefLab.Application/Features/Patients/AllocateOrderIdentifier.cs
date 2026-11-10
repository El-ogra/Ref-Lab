using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Allocates the composite order identifier for a laboratory order.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the identifier is 13 characters long. In the legacy
/// multi-branch product its ninth position carried the branch code; the
/// laboratory is now single-site, so the position is no longer used and the
/// freed position is absorbed by the adjacent segment whose meaning was never
/// established.
/// </para>
/// <para>
/// The semantics of the leading segment, the middle segment and the trailing
/// character were never established, so the caller supplies them verbatim.
/// </para>
/// </remarks>
public sealed record AllocateOrderIdentifierQuery(
    string PersonKey,
    string LeadingSegment,
    string MiddleSegment,
    string DisplayCode,
    char TrailingCharacter = '0') : IRequest<OrderIdentifierDto>;

/// <summary>
/// Handles the allocation of the composite order identifier.
/// </summary>
public sealed class AllocateOrderIdentifierHandler
    : IRequestHandler<AllocateOrderIdentifierQuery, OrderIdentifierDto>
{
    /// <inheritdoc />
    public Task<OrderIdentifierDto> Handle(
        AllocateOrderIdentifierQuery request,
        CancellationToken cancellationToken)
    {
        var identifier = IdentifierAllocator.Allocate(
            request.LeadingSegment,
            request.PersonKey,
            request.MiddleSegment,
            request.DisplayCode,
            request.TrailingCharacter);

        var dto = new OrderIdentifierDto(
            identifier.Value,
            identifier.PersonKey,
            identifier.DisplayCode,
            identifier.TrailingCharacter.ToString());

        return Task.FromResult(dto);
    }
}
