using System.Text.Json.Serialization;

namespace PackMeUp.Services.ResponseObjects
{
    public class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }

        [JsonPropertyName("daily")]
        public DailyForecast Daily { get; set; }
    }
}
