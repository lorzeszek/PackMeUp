using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PackMeUp.Interfaces;
using PackMeUp.Messages;
using PackMeUp.Popups;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public partial class ShellHeaderViewModel : BaseViewModel
    {
        //private readonly ISessionService _sessionService;
        //private readonly IGoogleAuthService _googleAuthService;
        //private readonly ISupabaseService _supabaseService;

        //public ISessionService Session => _sessionService;

        public IRelayCommand LoginWithGoogleCommand => new AsyncRelayCommand(LoginWithGoogle);
        public IRelayCommand LogoutCommand => new AsyncRelayCommand(Logout);

        public ShellHeaderViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {


        }
        //public ShellHeaderViewModel(ISessionService sessionService, IGoogleAuthService googleAuthService, ISupabaseService supabaseService)
        //{
        //    _sessionService = sessionService;
        //    _googleAuthService = googleAuthService;
        //    _supabaseService = supabaseService;
        //}

        private async Task LoginWithGoogle()
        {
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
                            Session.SetUser(session.User);

                            await _tripRepository.StartRealtimeAsync();

                            await _packingItemRepository.StartRealtimeAsync();

                            await _tripRepository.SyncPendingChangesAsync();

                            var trips = await _tripRepository.GetActiveTripsWithStatsAsync();

                            foreach (var trip in trips)
                            {
                                await _packingItemRepository.UpdatePendingPackingItems(trip.Trip.LocalTripId, trip.Trip.RemoteTripId);
                                await _packingItemRepository.SyncPendingChangesAsync();
                            }

                            // Send message to notify TripListViewModel about login completion
                            WeakReferenceMessenger.Default.Send(new LoginCompletedMessage());

                            // Navigate to TripList tab after successful login
                            await Shell.Current.GoToAsync("//TripList");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // obsługa błędu
                }
            }
        }

        private async Task Logout()
        {
            try
            {
                var popup = new ConfirmPopup("Log out", "Do you want to log out?");

                var result = await Application.Current.MainPage.ShowPopupAsync<bool>(popup);

                if (result.Result)
                {
                    await _tripRepository.UnsubscribeFromTripChangesAsync();
                    await _packingItemRepository.UnsubscribeFromPackingItemChangesAsync();
                    await _supabase.Client.Auth.SignOut();
                    await Shell.Current.GoToAsync("//Home");
                }
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }
    }
}
