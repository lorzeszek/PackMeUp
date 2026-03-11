using PackMeUp.Models.DTO;

namespace PackMeUp.Repositories.Models
{
    public class TripWithStats
    {
        public TripDTO Trip { get; }
        public string PackingSummary { get; }

        public TripWithStats(TripDTO trip, string packingSummary)
        {
            Trip = trip;
            PackingSummary = packingSummary;
        }
    }
}
