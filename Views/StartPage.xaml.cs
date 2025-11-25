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

    //private async void LoginWithGoogle_Clicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var options = new SignInWithSSOOptions
    //        {
    //            RedirectTo = "packmeup://auth-callback"
    //        };

    //        SSOResponse? response = await _viewModel._supabase.Client.Auth.SignInWithSSO("google.com", options);

    //        if (response != null)
    //        {
    //            // Po powrocie użytkownik jest zalogowany
    //            var user = _viewModel._supabase.Client.Auth.CurrentUser;
    //            await DisplayAlert("Sukces", $"Zalogowano jako {user.Email}", "OK");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Błąd logowania", ex.Message, "OK");
    //    }
    //}
}