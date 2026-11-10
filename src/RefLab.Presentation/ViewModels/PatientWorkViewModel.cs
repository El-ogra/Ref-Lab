using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Application.Features.Patients;
using RefLab.Application.Features.Results;
using RefLab.Domain.Enums;
using RefLab.Presentation.Common;
using RefLab.Presentation.Navigation;
using RefLab.Presentation.ViewModels.Panels;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the patient work screen.
/// </summary>
/// <remarks>
/// <para>
/// The patient work screen is the clinical core of the application. It carries
/// the three surfaces the reference product kept together in one window: the
/// English test-catalogue panel on the left, the Arabic patient panel on the
/// right, and the results of the ordered tests underneath.
/// </para>
/// <para>
/// Every value the screen shows is returned by the Application layer. The
/// screen sends requests through the request executor and copies the responses
/// into its display state; it never computes a clinical value, a total or a
/// classification of its own.
/// </para>
/// <para>
/// The screen carries the binding keyboard shortcuts: F8 reviews the selected
/// result, F9 saves the result that is being entered, and F11 and F12 are the
/// two stages of the print operation.
/// </para>
/// </remarks>
public sealed partial class PatientWorkViewModel : ViewModelBase, INavigationAware
{
    /// <summary>The fixed length of the composite order identifier.</summary>
    private const int OrderIdentifierLength = 13;

    private readonly IErrorPresenter _errorPresenter;

    /// <summary>Initialises a new patient work view model.</summary>
    public PatientWorkViewModel(
        RequestExecutor executor,
        IErrorPresenter errorPresenter,
        MethodSheetPanelViewModel methodSheet,
        DerivationPanelViewModel derivation)
        : base(executor)
    {
        _errorPresenter = errorPresenter ?? throw new ArgumentNullException(nameof(errorPresenter));
        MethodSheet = methodSheet ?? throw new ArgumentNullException(nameof(methodSheet));
        Derivation = derivation ?? throw new ArgumentNullException(nameof(derivation));
    }

    /// <inheritdoc />
    public override string Title => "Patient work";

    /// <summary>The Arabic patient panel.</summary>
    public PatientPanelViewModel Patient { get; } = new();

    /// <summary>The English test-catalogue panel.</summary>
    public CataloguePanelViewModel Catalogue { get; } = new();

    /// <summary>The method sheet and critical values panel.</summary>
    public MethodSheetPanelViewModel MethodSheet { get; }

    /// <summary>The derivation panel.</summary>
    public DerivationPanelViewModel Derivation { get; }

    /// <summary>The patients found by the last search.</summary>
    public ObservableCollection<PatientDto> SearchResults { get; } = new();

    /// <summary>The cases returned by the follow-up query.</summary>
    public ObservableCollection<CaseFollowUpDto> Cases { get; } = new();

    /// <summary>The ordered tests of the current order.</summary>
    public ObservableCollection<OrderTestDto> Tests { get; } = new();

    /// <summary>The search term.</summary>
    [ObservableProperty]
    private string? _searchTerm;

    /// <summary>The search telephone number.</summary>
    [ObservableProperty]
    private string? _searchPhone;

    /// <summary>The patient found by the last search.</summary>
    [ObservableProperty]
    private PatientDto? _selectedPatient;

    /// <summary>The order identifier allocated for the current patient.</summary>
    [ObservableProperty]
    private string? _allocatedIdentifier;

    /// <summary>The thirteen characters of the order identifier, as entered by the operator.</summary>
    [ObservableProperty]
    private string? _identifierInput;

    /// <summary>The identifier of the current order.</summary>
    [ObservableProperty]
    private Guid? _orderId;

    /// <summary>The identifier of the current patient.</summary>
    [ObservableProperty]
    private Guid? _patientId;

    /// <summary>The test whose result is being entered.</summary>
    [ObservableProperty]
    private OrderTestDto? _selectedTest;

    /// <summary>The result value being entered.</summary>
    [ObservableProperty]
    private string? _resultValue;

    /// <summary>The unit of the result being entered.</summary>
    [ObservableProperty]
    private string? _resultUnit;

    /// <summary>The comment attached to the result being entered.</summary>
    [ObservableProperty]
    private string? _resultComment;

    /// <summary>The slot of the additional result being entered.</summary>
    [ObservableProperty]
    private int _additionalResultSlot;

    /// <summary>The value of the additional result being entered.</summary>
    [ObservableProperty]
    private string? _additionalResultValue;

    /// <summary>The amount settled when the results are handed over.</summary>
    [ObservableProperty]
    private decimal? _handOverAmount;

    /// <summary>The payment type used when the results are handed over.</summary>
    [ObservableProperty]
    private int _paymentType;

    /// <summary>The destination directory of the print operation.</summary>
    [ObservableProperty]
    private string _destinationDirectory = string.Empty;

    /// <summary>The category the printed report is filed under.</summary>
    [ObservableProperty]
    private ReportCategory _reportCategory = ReportCategory.Individual;

    /// <summary>The file the last print produced.</summary>
    [ObservableProperty]
    private string? _lastPrintedPath;

    /// <summary>The path of the patient picture being attached.</summary>
    [ObservableProperty]
    private string? _picturePath;

    /// <summary>Searches for a patient.</summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        await RunAsync(async () =>
        {
            var page = PageRequest.First();

            var result = await _executor.SendAsync(new SearchPatientsQuery(SearchTerm, SearchPhone, page));

            SearchResults.Clear();

            foreach (var patient in result.Items)
            {
                SearchResults.Add(patient);
            }

            StatusMessage = $"{result.TotalCount} patient(s) found.";
        }).ConfigureAwait(true);
    }

    /// <summary>Registers a patient and opens the order.</summary>
    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Patient.Name))
        {
            ErrorMessage = "A patient name is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var order = await _executor.SendAsync(new RegisterPatientCommand(
                Patient.Name,
                Patient.Sex,
                Patient.Age,
                Patient.AgeUnit,
                Patient.BirthDate.HasValue ? DateOnly.FromDateTime(Patient.BirthDate.Value) : null,
                Patient.Address,
                Patient.Phone,
                Patient.Mobile,
                Patient.Fax,
                Patient.Email,
                Patient.ReferringPhysicianCode,
                Patient.ReceptionNote,
                LabCurrency.Code,
                CreateOrder: true));

            OrderId = order.Id;
            PatientId = order.PatientId;

            Patient.LegacyIdentifier = order.LegacyIdentifier;
            Patient.Barcode = order.Barcode;
            Patient.CurrencyCode = order.CurrencyCode;
            Patient.RecordStatus = order.Status.ToString();

            await LoadOrderAsync().ConfigureAwait(true);

            StatusMessage = "The patient was registered.";
        }).ConfigureAwait(true);
    }

    /// <summary>Validates and allocates the order identifier of the current patient.</summary>
    /// <remarks>
    /// The Application layer's allocate use case receives the thirteen characters
    /// as their five positional segments and returns the identifier the domain
    /// assembled from them. The presentation layer splits the entered value on
    /// the published positional layout and never assembles an identifier of its
    /// own: the domain remains the authority on the layout.
    /// </remarks>
    [RelayCommand]
    private async Task AllocateIdentifierAsync()
    {
        var value = (IdentifierInput ?? string.Empty).Trim();

        if (value.Length != OrderIdentifierLength)
        {
            ErrorMessage = $"The order identifier is exactly {OrderIdentifierLength} characters long.";
            return;
        }

        await RunAsync(async () =>
        {
            var identifier = await _executor.SendAsync(new AllocateOrderIdentifierQuery(
                value.Substring(2, 2),
                value.Substring(0, 2),
                value.Substring(4, 5),
                value.Substring(9, 3),
                value[12]));

            AllocatedIdentifier = identifier.Value;
            Patient.LegacyIdentifier = identifier.Value;
            IdentifierInput = identifier.Value;

            StatusMessage = "The order identifier was allocated.";
        }).ConfigureAwait(true);
    }

    /// <summary>Opens the order of the selected patient.</summary>
    [RelayCommand]
    private async Task OpenSelectedPatientAsync()
    {
        if (SelectedPatient is null)
        {
            ErrorMessage = "Select a patient first.";
            return;
        }

        await RunAsync(async () =>
        {
            var orders = await _executor.SendAsync(new SearchPatientsQuery(SelectedPatient.Name, null, PageRequest.First()));

            var patient = orders.Items.FirstOrDefault(candidate => candidate.Id == SelectedPatient.Id) ?? SelectedPatient;

            PatientId = patient.Id;
            Patient.Name = patient.Name;
            Patient.Sex = patient.Sex;
            Patient.Age = patient.Age;
            Patient.AgeUnit = patient.AgeUnit ?? AgeUnit.Years;
            Patient.BirthDate = patient.BirthDate.HasValue ? patient.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null;
            Patient.Address = patient.Address;
            Patient.Phone = patient.Phone;
            Patient.Mobile = patient.Mobile;
            Patient.Fax = patient.Fax;
            Patient.Email = patient.Email;
            Patient.ReferringPhysicianCode = patient.ReferringPhysicianCode;
            Patient.ReceptionNote = patient.ReceptionNote;

            StatusMessage = "The patient is open.";
        }).ConfigureAwait(true);
    }

    /// <summary>Reloads the ordered tests of the current order.</summary>
    [RelayCommand]
    private async Task LoadOrderAsync()
    {
        if (OrderId is null)
        {
            return;
        }

        await RunAsync(async () =>
        {
            var order = await _executor.SendAsync(new GetPatientOrderQuery(OrderId.Value));

            OrderId = order.Id;
            PatientId = order.PatientId;

            Patient.LegacyIdentifier = order.LegacyIdentifier;
            Patient.Barcode = order.Barcode;
            Patient.CurrencyCode = order.CurrencyCode;
            Patient.RecordStatus = order.Status.ToString();

            Tests.Clear();

            foreach (var test in order.Tests)
            {
                Tests.Add(test);
            }
        }).ConfigureAwait(true);
    }

    /// <summary>Orders the selected catalogue tests for the current order.</summary>
    [RelayCommand]
    private async Task OrderTestsAsync()
    {
        if (OrderId is null)
        {
            ErrorMessage = "Register or open a patient first.";
            return;
        }

        var codes = Catalogue.SelectedTests.Select(test => test.Code).Distinct().ToArray();

        if (codes.Length == 0)
        {
            ErrorMessage = "Select at least one test.";
            return;
        }

        await RunAsync(async () =>
        {
            var ordered = await _executor.SendAsync(new OrderTestsCommand(OrderId.Value, codes));

            Tests.Clear();

            foreach (var test in ordered)
            {
                Tests.Add(test);
            }

            Catalogue.ClearSelection();

            StatusMessage = $"{ordered.Count} test(s) ordered.";
        }).ConfigureAwait(true);
    }

    /// <summary>Saves the result that is being entered.</summary>
    [RelayCommand]
    private async Task SaveResultAsync()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ResultValue))
        {
            ErrorMessage = "A result value is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var updated = await _executor.SendAsync(new EnterResultCommand(
                SelectedTest.Id,
                ResultValue,
                ResultUnit,
                ResultComment));

            ReplaceTest(updated);

            ResultValue = string.Empty;
            ResultComment = string.Empty;

            StatusMessage = "The result was saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Saves an additional result of the selected test.</summary>
    [RelayCommand]
    private async Task SaveAdditionalResultAsync()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        await RunAsync(async () =>
        {
            var updated = await _executor.SendAsync(new EnterAdditionalResultCommand(
                SelectedTest.Id,
                AdditionalResultSlot,
                AdditionalResultValue));

            ReplaceTest(updated);

            AdditionalResultValue = string.Empty;

            StatusMessage = "The additional result was saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Reviews the selected result.</summary>
    [RelayCommand]
    private async Task ReviewAsync()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        await RunAsync(async () =>
        {
            var updated = await _executor.SendAsync(new ReviewResultCommand(SelectedTest.Id));

            ReplaceTest(updated);

            StatusMessage = "The result was reviewed.";
        }).ConfigureAwait(true);
    }

    /// <summary>Hands the results of the current order over.</summary>
    [RelayCommand]
    private async Task HandOverAsync()
    {
        if (OrderId is null)
        {
            ErrorMessage = "Register or open a patient first.";
            return;
        }

        await RunAsync(async () =>
        {
            var handed = await _executor.SendAsync(new HandOverResultsCommand(OrderId.Value, HandOverAmount, PaymentType));

            foreach (var test in handed)
            {
                ReplaceTest(test);
            }

            StatusMessage = "The results were handed over.";
        }).ConfigureAwait(true);
    }

    /// <summary>Generates the barcode of the current order.</summary>
    [RelayCommand]
    private async Task GenerateBarcodeAsync()
    {
        if (OrderId is null)
        {
            ErrorMessage = "Register or open a patient first.";
            return;
        }

        await RunAsync(async () =>
        {
            var barcode = await _executor.SendAsync(new GeneratePatientBarcodeCommand(OrderId.Value));

            Patient.Barcode = barcode.Barcode;

            StatusMessage = "The barcode was generated.";
        }).ConfigureAwait(true);
    }

    /// <summary>Attaches a patient picture to the current patient.</summary>
    [RelayCommand]
    private async Task AttachPictureAsync()
    {
        if (PatientId is null)
        {
            ErrorMessage = "Register or open a patient first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PicturePath))
        {
            ErrorMessage = "Choose a picture file first.";
            return;
        }

        await RunAsync(async () =>
        {
            var picture = await _executor.SendAsync(new AttachPatientPictureCommand(
                PatientId.Value,
                1,
                PicturePath,
                null,
                OrderId));

            Patient.Pictures.Add(picture);

            PicturePath = string.Empty;

            StatusMessage = "The picture was attached.";
        }).ConfigureAwait(true);
    }

    /// <summary>Loads the case list for the follow-up view.</summary>
    [RelayCommand]
    private async Task LoadCasesAsync()
    {
        await RunAsync(async () =>
        {
            var cases = await _executor.SendAsync(new FollowUpCaseQuery(null, null, true));

            Cases.Clear();

            foreach (var item in cases)
            {
                Cases.Add(item);
            }

            StatusMessage = $"{cases.Count} open case(s).";
        }).ConfigureAwait(true);
    }

    /// <summary>Prepares the print operation and shows the destination.</summary>
    /// <remarks>This is the F11 stage: the destination and the category are chosen and confirmed.</remarks>
    [RelayCommand]
    private void PreparePrint()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DestinationDirectory))
        {
            ErrorMessage = "Choose a destination directory first.";
            return;
        }

        StatusMessage = $"Ready to print to {DestinationDirectory}.";
    }

    /// <summary>Prints the selected result.</summary>
    /// <remarks>This is the F12 stage: the report is produced.</remarks>
    [RelayCommand]
    private async Task PrintAsync()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DestinationDirectory))
        {
            ErrorMessage = "Choose a destination directory first.";
            return;
        }

        await RunAsync(async () =>
        {
            var printed = await _executor.SendAsync(new PrintResultReportCommand(
                SelectedTest.Id,
                ReportCategory,
                DestinationDirectory));

            LastPrintedPath = printed.FullPath;

            await LoadOrderAsync().ConfigureAwait(true);

            StatusMessage = $"The report was written to {LastPrintedPath}.";
        }).ConfigureAwait(true);
    }

    /// <summary>Reloads the catalogue listing.</summary>
    [RelayCommand]
    private async Task LoadCatalogueAsync()
    {
        await RunAsync(async () =>
        {
            var page = PageRequest.First(200);

            var result = await _executor.SendAsync(new GetTestCatalogueQuery(Catalogue.Filter, null, true, page));

            Catalogue.Tests.Clear();

            foreach (var test in result.Items)
            {
                if (Catalogue.SendOutOnly && !test.IsSendOut)
                {
                    continue;
                }

                Catalogue.Tests.Add(test);
            }

            StatusMessage = $"{result.TotalCount} test(s) in the catalogue.";
        }).ConfigureAwait(true);
    }

    /// <summary>Adds the selected catalogue test to the current order.</summary>
    [RelayCommand]
    private void AddSelectedTest()
    {
        if (Catalogue.SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        if (!Catalogue.SelectedTests.Any(test => test.Code == Catalogue.SelectedTest.Code))
        {
            Catalogue.SelectedTests.Add(Catalogue.SelectedTest);
        }
    }

    /// <summary>Removes the selected catalogue test from the current order.</summary>
    [RelayCommand]
    private void RemoveSelectedTest()
    {
        if (Catalogue.SelectedTest is null)
        {
            return;
        }

        var existing = Catalogue.SelectedTests.FirstOrDefault(test => test.Code == Catalogue.SelectedTest.Code);

        if (existing is not null)
        {
            Catalogue.SelectedTests.Remove(existing);
        }
    }

    /// <summary>Clears the patient panel and the results.</summary>
    [RelayCommand]
    private void ClearPatient()
    {
        Patient.Name = string.Empty;
        Patient.Age = null;
        Patient.Address = null;
        Patient.Phone = null;
        Patient.Mobile = null;
        Patient.Fax = null;
        Patient.Email = null;
        Patient.ReferringPhysicianCode = null;
        Patient.ReceptionNote = null;
        Patient.LegacyIdentifier = null;
        Patient.Barcode = null;
        Patient.RecordStatus = null;
        Patient.Pictures.Clear();

        Tests.Clear();
        Cases.Clear();

        OrderId = null;
        PatientId = null;
        SelectedPatient = null;
        SelectedTest = null;
        AllocatedIdentifier = null;

        StatusMessage = "The panel was cleared.";
    }

    /// <inheritdoc />
    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is Guid orderId)
        {
            OrderId = orderId;

            _ = LoadOrderAsync();
        }
        else if (parameter is Guid patientId)
        {
            PatientId = patientId;
        }
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
    }

    private void ReplaceTest(OrderTestDto updated)
    {
        var index = -1;

        for (var position = 0; position < Tests.Count; position++)
        {
            if (Tests[position].Id == updated.Id)
            {
                index = position;
                break;
            }
        }

        if (index >= 0)
        {
            Tests[index] = updated;
        }
        else
        {
            Tests.Add(updated);
        }

        if (SelectedTest is not null && SelectedTest.Id == updated.Id)
        {
            SelectedTest = updated;
        }
    }
}
