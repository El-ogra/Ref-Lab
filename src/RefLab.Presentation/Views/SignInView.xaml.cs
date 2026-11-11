using System.Windows;
using System.Windows.Controls;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Views;

public partial class SignInView : UserControl
{
    public SignInView(SignInViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();

        DataContext = viewModel;

        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SignInViewModel.Password))
            {
                if (MaskedPasswordBox.Password != viewModel.Password)
                {
                    MaskedPasswordBox.Password = viewModel.Password ?? string.Empty;
                }
            }
        };
    }

    private void MaskedPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is SignInViewModel viewModel)
        {
            viewModel.Password = MaskedPasswordBox.Password;
        }
    }
}