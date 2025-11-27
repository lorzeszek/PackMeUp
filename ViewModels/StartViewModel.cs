using CommunityToolkit.Mvvm.Input;
using PackMeUp.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        private readonly IGoogleAuthService _googleAuthService;
        public IRelayCommand LoginWithGoogleCommand => new AsyncRelayCommand(LoginWithGoogle);

        public StartViewModel(ISupabaseService supabase, ISessionService sessionService, IGoogleAuthService googleAuthService) : base(supabase, sessionService)
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
                        //_sessionService.SetUser(session.User);

                        // Możesz teraz np. ustawić w ViewModel flagę:
                        //IsLoggedIn = true;
                        //var LoggedInUserName = user?.Email ?? user?.Id;

                        //await Shell.Current.GoToAsync(nameof(TripListPage));
                    }
                }
            }
            catch (Exception ex)
            {
                // obsługa błędu
            }
        }
    }
}
