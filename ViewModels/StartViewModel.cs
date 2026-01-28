using CommunityToolkit.Mvvm.Input;
using PackMeUp.Interfaces;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        private readonly IGoogleAuthService _googleAuthService;

        public IRelayCommand LoginWithGoogleCommand => new AsyncRelayCommand(LoginWithGoogle);

        public StartViewModel(ISupabaseService supabase, ISessionService sessionService, IGoogleAuthService googleAuthService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository) : base(supabase, sessionService, packingItemRepository, tripRepository)
        {
            _googleAuthService = googleAuthService;
        }

        //public async Task InitializeAsync()
        //{
        //    await Session.InitializeAsync();

        //    if (Session.IsLoggedIn)
        //    {
        //        await Shell.Current.GoToAsync("//TripList");
        //        return;
        //    }

        //    // jeśli NIE zalogowany → zostajemy na StartPage
        //    // tu będzie AI onboarding
        //}

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

                //await _tripRepository.UnsubscribeFromTripChangesAsync();

                //await _packingItemRepository.UnsubscribeFromPackingItemChangesAsync();

                await _tripRepository.StartRealtimeAsync();

                await _packingItemRepository.StartRealtimeAsync();

            }
            catch (Exception ex)
            {
                // obsługa błędu
            }
        }


        //protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        //{
        //    if (Session.IsLoggedIn)
        //    {
        //        await Shell.Current.GoToAsync(nameof(TripListPage));
        //    }
        //}
    }
}
