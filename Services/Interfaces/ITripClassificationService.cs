using Packo.Services.ResponseObjects;

namespace Packo.Services.Interfaces
{
    public interface ITripClassificationService
    {
        Task<TripClassificationData> ClassifyTripAsync(string destination, DateTime start, DateTime end, List<string> selectedTransportTypes);
    }
}
