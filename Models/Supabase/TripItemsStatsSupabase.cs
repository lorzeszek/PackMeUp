using System.Text.Json.Serialization;

namespace PackMeUp.Models.Supabase
{
    public class TripItemsStatsSupabase
    {
        [JsonPropertyName("tripid")]
        public int TripId { get; set; }

        [JsonPropertyName("ispackedcount")]
        public int IsPackedCount { get; set; }

        [JsonPropertyName("isnotpackedcount")]
        public int IsNotPackedCount { get; set; }
    }
}
