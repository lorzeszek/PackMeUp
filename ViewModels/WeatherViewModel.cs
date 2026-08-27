using Packo.Extensions;
using Packo.Interfaces;
using Packo.Models.DTO;
using Packo.Repositories.Interfaces;
using Packo.Services;
using Packo.Services.Interfaces;

namespace Packo.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        public ObservableRangeCollection<DestinationWeatherDTO> Destinations { get; } = new ObservableRangeCollection<DestinationWeatherDTO>();

        private readonly WeatherService _weatherService;

        public WeatherViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService, WeatherService weatherService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
            _weatherService = weatherService;
        }

        private async Task LoadWeatherForDestinationsAsync(IReadOnlyList<string> destinations)
        {
            if (destinations == null || destinations.Count == 0)
                return;

            await Parallel.ForEachAsync(destinations, async (destination, ct) =>
            {
                if (Destinations.Any(d => d.Destination == destination))
                {
                    return;
                }
                else
                {
                    var weather = await GetWeatherForDestination(destination);

                    if (weather != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Destinations.Add(weather);
                        });
                    }
                }
            });

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var existingWeatherToRemove = Destinations.Where(d => !destinations.Contains(d.Destination)).ToList();
                if (existingWeatherToRemove != null)
                {
                    Destinations.RemoveRange(existingWeatherToRemove);
                }
            });
        }


        private async Task<DestinationWeatherDTO?> GetWeatherForDestination(string destination)
        {
            var location = await _weatherService.GetLocationAsync(destination);

            if (location == null)
                return null;

            var current = await _weatherService.GetWeatherAsync(
                location.Latitude,
                location.Longitude);

            var daily = await _weatherService.GetDailyWeatherAsync(
                location.Latitude,
                location.Longitude);

            if (current == null || daily == null)
                return null;

            var forecastDays = daily.Dates
                .Select((date, index) => new { date, index })
                .Where(x =>
                    daily.MaxTemps[x.index].HasValue &&
                    daily.MinTemps[x.index].HasValue &&
                    daily.WeatherCodes[x.index].HasValue)
                .Select(x => new DailyForecastDTO
                {
                    Date = DateTime.Parse(x.date),
                    MaxTemp = daily.MaxTemps[x.index]!.Value,
                    MinTemp = daily.MinTemps[x.index]!.Value,
                    WeatherCode = daily.WeatherCodes[x.index]!.Value
                })
                .ToList();

            return new DestinationWeatherDTO
            {
                Destination = destination,
                CurrentWeather = current,
                ForecastDays = forecastDays
            };
        }

        public async Task OnAppearingAsync()
        {
            var destinations = await _tripRepository.GetActiveDestinationsAsync();
            await LoadWeatherForDestinationsAsync(destinations);
        }

        protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            //await OnAppearingAsync();
        }
    }
}
