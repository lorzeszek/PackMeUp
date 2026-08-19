using System.Text.Json.Serialization;

namespace PackMeUp.Services.ResponseObjects
{
    public class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult>? Results { get; set; }
    }
}
