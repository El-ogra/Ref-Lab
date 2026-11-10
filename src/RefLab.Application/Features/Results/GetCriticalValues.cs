using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Returns the critical limits of an analyte, as consulted before a result is released.
/// </summary>
/// <remarks>
/// Legacy evidence: the critical thresholds survive in the method sheets, in the form <c>Hgb &lt; 5</c> or <c>&gt; 20 g/dL</c> — a strict comparison.
/// </remarks>
public sealed record GetCriticalValuesQuery(int TestCode) : IRequest<CriticalValueDto>;


/// <summary>
/// Handles the GetCriticalValues use case.
/// </summary>
public sealed class GetCriticalValuesHandler : IRequestHandler<GetCriticalValuesQuery, CriticalValueDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetCriticalValuesHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<CriticalValueDto> Handle(GetCriticalValuesQuery request, CancellationToken cancellationToken)
    {
        var definition = _db.Set<TestDefinition>()
            .FirstOrDefault(candidate => candidate.Code == request.TestCode)
            ?? throw new NotFoundException(nameof(TestDefinition), request.TestCode);

        var critical = definition.CriticalInterval;

        var values = new CriticalValueDto(
            critical.LowCriticalLimit,
            critical.HighCriticalLimit,
            critical.CriticalRangeText,
            critical.CriticalRangeText,
            critical.CriticalExceptionText,
            critical.CriticalComment);

        return Task.FromResult<CriticalValueDto>(values);
    }
}
