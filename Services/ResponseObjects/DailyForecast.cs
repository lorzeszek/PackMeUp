using System.Text.Json.Serialization;

namespace Packo.Services.ResponseObjects
{
    public class DailyForecast
    {
        [JsonPropertyName("time")]
        public List<string> Dates { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<decimal?> MaxTemps { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<decimal?> MinTemps { get; set; }

        [JsonPropertyName("weather_code")]
        public List<int?> WeatherCodes { get; set; }
    }
}
