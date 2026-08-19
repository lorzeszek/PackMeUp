using PackMeUp.Services.ResponseObjects;

namespace PackMeUp.Models.DTO
{
    public class DestinationWeatherDTO
    {
        public string Destination { get; set; }

        public CurrentWeather CurrentWeather { get; set; }

        public List<DailyForecastDTO> ForecastDays { get; set; } = [];
    }
}
