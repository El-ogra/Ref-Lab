using RefLab.Application.Features.Catalogue;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Tests.ViewModels;

/// <summary>
/// The catalogue panel of the patient work screen — the panel that carries the
/// catalogue listing and the tests selected for the current order.
/// </summary>
/// <remarks>
/// The panel carries the three actions of the reference system's own catalogue
/// panel — <c>Add select test</c>, <c>Delete</c> and <c>All</c>. The bindings
/// themselves are declared in <c>CataloguePanelControl.xaml</c> and can only be
/// exercised on Windows; the behaviour of the commands behind them is
/// exercised here.
/// </remarks>
public sealed class CataloguePanelViewModelTests
{
    [Fact]
    public void The_add_command_adds_the_highlighted_catalogue_test_to_the_selection()
    {
        var viewModel = new CataloguePanelViewModel();
        var test = CreateTest(56, "Complete Blood Count");

        viewModel.Tests.Add(test);
        viewModel.SelectedTest = test;

        viewModel.AddSelectedTestCommand.Execute(null);

        Assert.Single(viewModel.SelectedTests);
        Assert.Equal(56, viewModel.SelectedTests[0].Code);
    }

    [Fact]
    public void The_add_command_does_not_add_the_same_test_twice()
    {
        var viewModel = new CataloguePanelViewModel();
        var test = CreateTest(56, "Complete Blood Count");

        viewModel.Tests.Add(test);
        viewModel.SelectedTest = test;

        viewModel.AddSelectedTestCommand.Execute(null);
        viewModel.AddSelectedTestCommand.Execute(null);

        Assert.Single(viewModel.SelectedTests);
    }

    [Fact]
    public void The_remove_command_removes_the_highlighted_selected_test()
    {
        var viewModel = new CataloguePanelViewModel();
        var test = CreateTest(56, "Complete Blood Count");

        viewModel.SelectedTests.Add(test);
        viewModel.SelectedOrderTest = test;

        viewModel.RemoveSelectedTestCommand.Execute(null);

        Assert.Empty(viewModel.SelectedTests);
    }

    [Fact]
    public void The_add_all_command_adds_every_catalogue_test_to_the_selection()
    {
        var viewModel = new CataloguePanelViewModel();

        viewModel.Tests.Add(CreateTest(56, "Complete Blood Count"));
        viewModel.Tests.Add(CreateTest(57, "Liver Profile"));

        viewModel.AddAllTestsCommand.Execute(null);

        Assert.Equal(2, viewModel.SelectedTests.Count);
        Assert.Equal(56, viewModel.SelectedTests[0].Code);
        Assert.Equal(57, viewModel.SelectedTests[1].Code);
    }

    [Fact]
    public void The_add_all_command_does_not_duplicate_a_test_already_selected()
    {
        var viewModel = new CataloguePanelViewModel();
        var test = CreateTest(56, "Complete Blood Count");

        viewModel.Tests.Add(test);
        viewModel.Tests.Add(CreateTest(57, "Liver Profile"));
        viewModel.SelectedTests.Add(test);

        viewModel.AddAllTestsCommand.Execute(null);

        Assert.Equal(2, viewModel.SelectedTests.Count);
    }

    private static TestDefinitionDto CreateTest(int code, string name) =>
        new(
            code,
            name,
            name,
            null,
            null,
            11,
            "BLOOD PICTURE",
            1,
            true,
            false,
            null,
            "EDTA Blood",
            150m,
            100m,
            200m,
            false,
            true,
            null,
            null);
}
