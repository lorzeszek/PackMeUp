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
            }
            catch (Exception ex)
            {
                // obsługa błędu
            }
        }
    }
}
