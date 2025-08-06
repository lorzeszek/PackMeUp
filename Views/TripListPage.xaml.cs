using PackMeUp.ViewModels;

namespace PackMeUp.Views;

public partial class TripListPage : ContentPage
{
    private readonly TripListViewModel _viewModel;

    public TripListPage(TripListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    //private async void OnTripSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    if (e.CurrentSelection.FirstOrDefault() is Trip selectedTrip)
    //    {
    //        await Navigation.PushAsync(new PackingListPage(selectedTrip));
    //    }
    //}
}
