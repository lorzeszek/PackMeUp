using PackMeUp.Extensions;
using PackMeUp.Interfaces;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        public ObservableRangeCollection<string> Destinations { get; } = new();

        public WeatherViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
        }

        private async Task LoadDestinationsAsync(IReadOnlyList<string> destinations)
        {
            Destinations.Clear();

            foreach (var destination in destinations)
            {
                Destinations.Add(destination);
            }
        }

        public async Task OnAppearingAsync()
        {
            var destinations = await _tripRepository.GetActiveDestinationsAsync();
            await LoadDestinationsAsync(destinations);
        }

        protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            await OnAppearingAsync();
        }
    }
}
