using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Derives a ratio result from two results already on an order and enters it
/// on the ratio row of the same order.
/// </summary>
/// <remarks>
/// <para>
/// Some analytes are reported as the ratio of two others rather than as a
/// measurement of their own — the AST/ALT ratio and the albumin/globulin
/// ratio are the two the laboratory reports. The reference system shows such a
/// row on the result sheet as a row of its own, with its own normal range and
/// its own high or low flag, and the ratio row is therefore an ordinary
/// requested test that carries a measured value.
/// </para>
/// <para>
/// The value on that row is not typed by the operator: it is computed from the
/// two results that are already on the order, so that the reported ratio can
/// always be reproduced from the results it was derived from. The derivation
/// rule itself lives in the domain, in
/// <see cref="ClinicalDerivationPolicy.DeriveRatio"/>.
/// </para>
/// <para>
/// The ratio row is classified against its own frozen reference interval by the
/// existing classification rule, exactly like any other result.
/// </para>
/// </remarks>
public sealed record EnterDerivedRatioCommand(
    Guid OrderId,
    int NumeratorTestCode,
    int DenominatorTestCode,
    int RatioTestCode,
    string? Comment = null) : IRequest<OrderTestDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.EnterResults;
}

/// <summary>
/// Handles the entry of a derived ratio result.
/// </summary>
public sealed class EnterDerivedRatioHandler : IRequestHandler<EnterDerivedRatioCommand, OrderTestDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public EnterDerivedRatioHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<OrderTestDto> Handle(EnterDerivedRatioCommand request, CancellationToken cancellationToken)
    {
        var tests = _db.Set<OrderTest>()
            .Where(test => test.OrderId == request.OrderId)
            .ToArray();

        var numerator = Find(tests, request.NumeratorTestCode);
        var denominator = Find(tests, request.DenominatorTestCode);
        var ratio = Find(tests, request.RatioTestCode);

        if (numerator.Result is null || !numerator.Result.IsNumeric)
        {
            throw new ConflictException(
                $"The result of test {request.NumeratorTestCode} has not been entered as a number.");
        }

        if (denominator.Result is null || !denominator.Result.IsNumeric)
        {
            throw new ConflictException(
                $"The result of test {request.DenominatorTestCode} has not been entered as a number.");
        }

        var derived = ClinicalDerivationPolicy.DeriveRatio(
            numerator.TestSimpleName,
            denominator.TestSimpleName,
            numerator.Result.NumericValue!.Value,
            denominator.Result.NumericValue!.Value);

        var now = _clock.UtcNow;
        var operatorName = _currentUser.OperatorName ?? "unknown";

        ratio.EnterResult(derived.ToResultValue(), now, operatorName);

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            ratio.SetComment(request.Comment, now, operatorName);
        }

        _unitOfWork.Update(ratio);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderTestDto.FromDomain(ratio);
    }

    private static OrderTest Find(IReadOnlyCollection<OrderTest> tests, int testCode) =>
        tests.FirstOrDefault(test => test.TestCode == testCode)
        ?? throw new NotFoundException(nameof(OrderTest), testCode);
}
