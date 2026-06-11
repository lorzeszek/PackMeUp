using PackMeUp.Extensions;
using PackMeUp.Interfaces;
using PackMeUp.Models.DTO;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        //public ObservableRangeCollection<string> Destinations { get; } = [];
        public ObservableRangeCollection<DestinationWeatherDTO> Destinations { get; } = [];

        private readonly WeatherService _weatherService;

        public WeatherViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService, WeatherService weatherService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
            _weatherService = weatherService;
        }

        private async Task LoadWeatherForDestinationsAsync(IReadOnlyList<string> destinations)
        {
            Destinations.Clear();

            if (destinations == null || destinations.Count == 0)
                return;

            await Parallel.ForEachAsync(destinations, async (destination, ct) =>
            {
                var weather = await GetWeatherForDestination(destination);

                if (weather != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Destinations.Add(weather);
                    });
                }
            });

            //var tasks = destinations.Select(GetWeatherForDestination);

            //var results = await Task.WhenAll(tasks);

            //Destinations.AddRange(results.Where(x => x != null)!);
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

        //private async Task GetWeatherForDestination(string destination)
        //{
        //    var location = await _weatherService.GetLocationAsync(destination);


        //    //var location = await Geolocation.Default.GetLocationAsync();

        //    if (location != null)
        //    {
        //        double lat = location.Latitude;
        //        double lon = location.Longitude;

        //        var resultCurrent = await _weatherService.GetWeatherAsync(latitude: lat, longitude: lon);
        //        var resultDaily = await _weatherService.GetDailyWeatherAsync(latitude: lat, longitude: lon);
        //    }


        //}

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
