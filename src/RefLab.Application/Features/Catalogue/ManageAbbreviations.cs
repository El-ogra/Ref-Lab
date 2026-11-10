using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Adds an abbreviation to the dictionary, or amends the expansion of an existing one.
/// </summary>
/// <remarks>
/// Legacy evidence: the abbreviation dictionary holds 650 entries and is unchanged between the two legacy versions.
/// </remarks>
public sealed record ManageAbbreviationsCommand(int Id, string Text, string Meaning) : IRequest<AbbreviationDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the ManageAbbreviations use case.
/// </summary>
public sealed class ManageAbbreviationsHandler : IRequestHandler<ManageAbbreviationsCommand, AbbreviationDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManageAbbreviationsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<AbbreviationDto> Handle(ManageAbbreviationsCommand request, CancellationToken cancellationToken)
    {
        var abbreviation = _db.Set<Abbreviation>()
            .FirstOrDefault(candidate => candidate.Id == request.Id);

        if (abbreviation is null)
        {
            abbreviation = Abbreviation.Create(request.Id, request.Text, request.Meaning);
            _unitOfWork.Add(abbreviation);
        }
        else
        {
            abbreviation.UpdateMeaning(request.Meaning);
            _unitOfWork.Update(abbreviation);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return AbbreviationDto.FromDomain(abbreviation);
    }
}
