using PackMeUp.ViewModels;

namespace PackMeUp.Views;

public partial class PackingListPage : ContentPage//, IQueryAttributable
{
    private readonly PackingListViewModel _viewModel;

    public PackingListPage(PackingListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    //public async void ApplyQueryAttributes(IDictionary<string, object> query)
    //{
    //    if (query.TryGetValue("tripId", out var tripIdObj) && tripIdObj is string tripId)
    //    {
    //        await _viewModel.InitializeRealtimeAsync(tripId);
    //    }
    //}

    //public void ApplyQueryAttributes(IDictionary<string, object> query)
    //{
    //    if (query.TryGetValue("tripId", out var tripIdObj) && tripIdObj is string tripId)
    //    {
    //        _ = _viewModel.InitializeRealtimeAsync(tripId);
    //    }
    //}

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await _viewModel.InitializeRealtimeAsync();
    //}

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
