using Packo.Services.ResponseObjects;

namespace Packo.Models.DTO
{
    public class DestinationWeatherDTO
    {
        public string Destination { get; set; }

        public CurrentWeather CurrentWeather { get; set; }

        public List<DailyForecastDTO> ForecastDays { get; set; } = [];
    }
}
