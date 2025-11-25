using CommunityToolkit.Mvvm.Input;
using PackMeUp.Interfaces;
using PackMeUp.Services;
using PackMeUp.Views;

namespace PackMeUp.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        private readonly IGoogleAuthService _googleAuthService;
        public IRelayCommand LoginWithGoogleCommand => new AsyncRelayCommand(LoginWithGoogle);

        public StartViewModel(ISupabaseService supabase, IGoogleAuthService googleAuthService) : base(supabase)
        {
            _googleAuthService = googleAuthService;
        }

        private async Task LoginWithGoogle()
        {
            try
            {
                var token = await _googleAuthService.SignInWithGoogleAsync();

                if (token != null)
                {
                    // Tutaj możesz np. zalogować użytkownika w Supabase:
                    var session = await _supabase.Client.Auth.SignInWithIdToken(Supabase.Gotrue.Constants.Provider.Google, token);

                    if (session != null)
                    {
                        // Zalogowany użytkownik jest dostępny:
                        var user = session.User;

                        // Możesz teraz np. ustawić w ViewModel flagę:
                        //IsLoggedIn = true;
                        //LoggedInUserName = user?.Email ?? user?.Id;

                        // Albo nawigować do innego widoku
                        await Shell.Current.GoToAsync(nameof(TripListPage));
                        //await Shell.Current.GoToAsync("//HomePage");
                    }

                    //Debug.WriteLine($"Google Id Token: {token}");
                }
                //else
                //{
                //    Debug.WriteLine("Logowanie przerwane lub nieudane");
                //}

                //var authResult = await WebAuthenticator.AuthenticateAsync(
                //    new Uri("https://dahwafdelpczzbgvlelc.supabase.co/auth/v1/authorize?provider=google&redirect_to=packmeup://auth-callback"),
                //    new Uri("packmeup://auth-callback")
                //);

                //var idToken = authResult?.Properties["access_token"];
                //var user = await _supabase.Client.Auth.SignInWithIdToken(idToken);
            }
            catch (Exception ex)
            {
                // obsługa błędu
            }
        }

        //private async void LoginWithGoogle_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var options = new SignInWithSSOOptions
        //        {
        //            RedirectTo = "packmeup://auth-callback"
        //        };

        //        SSOResponse? response = await _supabase.Client.Auth.SignInWithSSO("google.com", options);

        //        if (response != null)
        //        {
        //            // Po powrocie użytkownik jest zalogowany
        //            var user = _supabase.Client.Auth.CurrentUser;
        //            await DisplayAlert("Sukces", $"Zalogowano jako {user.Email}", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Błąd logowania", ex.Message, "OK");
        //    }
        //}
    }
}
