using PackMeUp.Models.Pages;
using PackMeUp.ViewModels;

namespace PackMeUp.Views;

public partial class TripListPage : BasePage// ContentPage
{
    private readonly TripListViewModel _viewModel;

    public TripListPage(TripListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.DisposeRealtimeAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TripListViewModel vm)
            await vm.OnAppearingAsync();
    }

    //private async void OnTripSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    if (e.CurrentSelection.FirstOrDefault() is Trip selectedTrip)
    //    {
    //        await Navigation.PushAsync(new PackingListPage(selectedTrip));
    //    }
    //}
}
