using System.Text.Json.Serialization;

namespace Packo.Services.ResponseObjects
{
    public class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult>? Results { get; set; }
    }
}
