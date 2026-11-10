namespace RefLab.Presentation.Common;

/// <summary>
/// The laboratory's currency.
/// </summary>
/// <remarks>
/// <para>
/// The project owner fixed the laboratory currency as <c>EGP</c>. The
/// <see cref="RefLab.Domain.ValueObjects.Money"/> value object requires an
/// ISO 4217 currency code and deliberately has no default, so the currency is
/// a configuration decision that the owner makes rather than a value this
/// layer may invent. This constant is the single place in the presentation
/// layer where that decision is expressed.
/// </para>
/// <para>
/// The constant is used only to <em>supply</em> the currency to the Application
/// layer when a command requires one. Money is never formatted from this
/// constant: the display of an amount always uses the
/// <c>CurrencyCode</c> that the Application layer returns on the DTO, so that
/// the value shown is the value the domain actually stored.
/// </para>
/// </remarks>
public static class LabCurrency
{
    /// <summary>The ISO 4217 code of the laboratory currency.</summary>
    public const string Code = "EGP";
}
