using Packo.Models.Pages;
using Packo.ViewModels;

namespace Packo.Views;

public partial class SettingsPage : BasePage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
