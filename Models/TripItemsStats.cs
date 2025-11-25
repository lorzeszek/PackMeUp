using System.Text.Json.Serialization;

namespace PackMeUp.Models
{
    public class TripItemsStats
    {
        [JsonPropertyName("tripid")]
        public int TripId { get; set; }

        [JsonPropertyName("ispackedcount")]
        public int IsPackedCount { get; set; }

        [JsonPropertyName("isnotpackedcount")]
        public int IsNotPackedCount { get; set; }
    }
}
