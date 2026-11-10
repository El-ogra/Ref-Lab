using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Returns the method sheet of an analyte: its sample type, its clinical description and its critical values.
/// </summary>
/// <remarks>
/// Legacy evidence: the method sheets carried the sample type, the measurement principle, the critical thresholds and the clinical significance of every test.
/// </remarks>
public sealed record GetMethodSheetQuery(int TestCode) : IRequest<MethodSheetDto>;


/// <summary>
/// Handles the GetMethodSheet use case.
/// </summary>
public sealed class GetMethodSheetHandler : IRequestHandler<GetMethodSheetQuery, MethodSheetDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetMethodSheetHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<MethodSheetDto> Handle(GetMethodSheetQuery request, CancellationToken cancellationToken)
    {
        var definition = _db.Set<TestDefinition>()
            .FirstOrDefault(candidate => candidate.Code == request.TestCode)
            ?? throw new NotFoundException(nameof(TestDefinition), request.TestCode);

        var critical = definition.CriticalInterval;

        var sheet = new MethodSheetDto(
            definition.Code,
            definition.Name,
            definition.SampleType,
            definition.Description,
            definition.Effect,
            definition.Usage,
            new[]
            {
                new CriticalValueDto(
                    critical.LowCriticalLimit,
                    critical.HighCriticalLimit,
                    critical.CriticalRangeText,
                    critical.CriticalRangeText,
                    critical.CriticalExceptionText,
                    critical.CriticalComment),
            });

        return Task.FromResult<MethodSheetDto>(sheet);
    }
}
