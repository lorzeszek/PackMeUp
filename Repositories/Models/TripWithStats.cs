using PackMeUp.Models;

namespace PackMeUp.Repositories.Models
{
    public class TripWithStats
    {
        public Trip Trip { get; }
        public string PackingSummary { get; }

        public TripWithStats(Trip trip, string packingSummary)
        {
            Trip = trip;
            PackingSummary = packingSummary;
        }
    }
}
