using PackMeUp.Models.Pages;
using PackMeUp.ViewModels;

namespace PackMeUp.Views;

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
