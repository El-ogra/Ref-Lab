using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Attaches the composite order identifier to an order and records it.
/// </summary>
/// <remarks>
/// <para>
/// The identifier is the accession number under which the laboratory filed the
/// visit. It is issued once and never changes, and it is the value printed on
/// the specimen label as the barcode.
/// </para>
/// <para>
/// The identifier is assembled by the domain from its five positional
/// segments, so that the layout stays the domain's business and the caller
/// never formats an identifier of its own.
/// </para>
/// </remarks>
public sealed record AttachOrderIdentifierCommand(
    Guid OrderId,
    string LeadingSegment,
    string PersonKey,
    string MiddleSegment,
    string DisplayCode,
    char TrailingCharacter = '0') : IRequest<OrderIdentifierDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.RegisterPatient;
}

/// <summary>
/// Handles the attachment of the composite order identifier to an order.
/// </summary>
public sealed class AttachOrderIdentifierHandler
    : IRequestHandler<AttachOrderIdentifierCommand, OrderIdentifierDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public AttachOrderIdentifierHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<OrderIdentifierDto> Handle(
        AttachOrderIdentifierCommand request,
        CancellationToken cancellationToken)
    {
        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var identifier = IdentifierAllocator.Allocate(
            request.LeadingSegment,
            request.PersonKey,
            request.MiddleSegment,
            request.DisplayCode,
            request.TrailingCharacter);

        order.AttachLegacyIdentifier(identifier);

        _unitOfWork.Update(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderIdentifierDto(
            identifier.Value,
            identifier.PersonKey,
            identifier.DisplayCode,
            identifier.TrailingCharacter.ToString());
    }
}
