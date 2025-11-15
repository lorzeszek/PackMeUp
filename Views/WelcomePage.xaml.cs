namespace PackMeUp.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void StartButton_Clicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TripListPage));
    }
}