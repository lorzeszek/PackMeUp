using PackMeUp.ViewModels;

namespace PackMeUp.Views;

public partial class StartPage : ContentPage
{
    private readonly StartViewModel _viewModel;
    public StartPage(StartViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void StartButton_Clicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TripListPage));
    }
}