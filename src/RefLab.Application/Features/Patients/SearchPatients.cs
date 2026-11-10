using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Searches the patient register.
/// </summary>
public sealed record SearchPatientsQuery(string? Name, string? Phone, PageRequest Page)
    : IRequest<PagedResult<PatientDto>>;

/// <summary>
/// Handles the patient search.
/// </summary>
public sealed class SearchPatientsHandler : IRequestHandler<SearchPatientsQuery, PagedResult<PatientDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public SearchPatientsHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<PagedResult<PatientDto>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Patient>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(patient => patient.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phone = request.Phone.Trim();
            query = query.Where(patient =>
                (patient.Phone != null && patient.Phone.Contains(phone)) ||
                (patient.Mobile != null && patient.Mobile.Contains(phone)));
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(patient => patient.Name)
            .Skip(request.Page.Skip)
            .Take(request.Page.Take)
            .Select(patient => PatientDto.FromDomain(patient))
            .ToArray();

        return Task.FromResult(new PagedResult<PatientDto>(
            items,
            totalCount,
            request.Page.PageNumber,
            request.Page.PageSize));
    }
}
