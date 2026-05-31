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

    private void OnStartDateIconTapped(object sender, TappedEventArgs e)
    {
#if ANDROID
        (StartDatePicker.Handler?.PlatformView as Android.Views.View)?.PerformClick();
#else
        StartDatePicker.Focus();
#endif
    }

    private void OnEndDateIconTapped(object sender, TappedEventArgs e)
    {
#if ANDROID
        (EndDatePicker.Handler?.PlatformView as Android.Views.View)?.PerformClick();
#else
        EndDatePicker.Focus();
#endif
    }
}