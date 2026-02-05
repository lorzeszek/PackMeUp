using PackMeUp.Models.Pages;
using PackMeUp.ViewModels;

namespace PackMeUp.Views;

public partial class TripSetupPage : BasePage
{
    private readonly TripSetupViewModel _viewModel;

    public TripSetupPage(TripSetupViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

}