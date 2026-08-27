using System.Text.Json.Serialization;

namespace Packo.Services.ResponseObjects
{
    public class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }

        [JsonPropertyName("daily")]
        public DailyForecast Daily { get; set; }
    }
}
