using PackMeUp.ViewModels;

namespace PackMeUp.Views;

public partial class PackingListPage : ContentPage, IQueryAttributable
{
    private readonly PackingListViewModel _viewModel;

    public PackingListPage(PackingListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("tripId", out var tripIdObj) && tripIdObj is string tripId)
        {
            _ = _viewModel.LoadTripItemsAsync(tripId);
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
