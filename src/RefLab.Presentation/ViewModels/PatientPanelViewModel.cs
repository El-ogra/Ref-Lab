using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Enums;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The display state of the Arabic patient panel.
/// </summary>
/// <remarks>
/// <para>
/// The panel holds display state only. It contains no command and no business
/// logic: every value it shows is copied from the DTOs returned by the
/// Application layer, or entered by the operator and handed to the parent
/// view model.
/// </para>
/// <para>
/// The panel is the Arabic surface of the application and is laid out
/// right to left. The identifiers and the amounts inside it keep a left to
/// right direction of their own so that the thirteen-character order
/// identifier and the decimal amounts read unambiguously.
/// </para>
/// </remarks>
public sealed partial class PatientPanelViewModel : ObservableObject
{
    /// <summary>The name of the patient.</summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>The sex of the patient.</summary>
    [ObservableProperty]
    private Sex _sex = Sex.Male;

    /// <summary>The age of the patient.</summary>
    [ObservableProperty]
    private decimal? _age;

    /// <summary>The unit the age is expressed in.</summary>
    [ObservableProperty]
    private AgeUnit _ageUnit = AgeUnit.Years;

    /// <summary>The date of birth of the patient.</summary>
    [ObservableProperty]
    private DateTime? _birthDate;

    /// <summary>The address of the patient.</summary>
    [ObservableProperty]
    private string? _address;

    /// <summary>The telephone number of the patient.</summary>
    [ObservableProperty]
    private string? _phone;

    /// <summary>The mobile number of the patient.</summary>
    [ObservableProperty]
    private string? _mobile;

    /// <summary>The fax number of the patient.</summary>
    [ObservableProperty]
    private string? _fax;

    /// <summary>The electronic mail address of the patient.</summary>
    [ObservableProperty]
    private string? _email;

    /// <summary>The code of the treating physician.</summary>
    [ObservableProperty]
    private string? _referringPhysicianCode;

    /// <summary>
    /// The code of the referring party the order was received from.
    /// </summary>
    /// <remarks>
    /// The referring party is the counterparty the laboratory received the
    /// order from — a clinic, a practice or another laboratory. The commission
    /// the laboratory pays on the order is computed on the base amount of the
    /// orders it received from that party, so the code is recorded with the
    /// order when the order is created.
    /// </remarks>
    [ObservableProperty]
    private string? _referringPartyCode;

    /// <summary>The reception note of the patient.</summary>
    [ObservableProperty]
    private string? _receptionNote;

    /// <summary>The thirteen-character order identifier.</summary>
    [ObservableProperty]
    private string? _legacyIdentifier;

    /// <summary>The barcode of the order.</summary>
    [ObservableProperty]
    private string? _barcode;

    /// <summary>The status of the order record.</summary>
    [ObservableProperty]
    private string? _recordStatus;

    /// <summary>The currency of the order.</summary>
    [ObservableProperty]
    private string _currencyCode = Common.LabCurrency.Code;

    /// <summary>Whether the patient is a special case.</summary>
    [ObservableProperty]
    private bool _isVip;

    /// <summary>The patient pictures attached to this patient.</summary>
    public ObservableCollection<PatientPictureDto> Pictures { get; } = new();
}
