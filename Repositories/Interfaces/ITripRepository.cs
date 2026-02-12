using PackMeUp.Models;
using PackMeUp.Models.DTO;
using PackMeUp.Repositories.Models;

namespace PackMeUp.Repositories.Interfaces
{
    public interface ITripRepository
    {
        Task AddTripAsync(TripDTO trip);
        Task<Trip?> GetTripAsync(TripDTO trip);
        Task UpdateTripAsync(TripDTO trip);
        Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync();
        Task DeleteTripAsync(TripDTO trip);
        Task UnsubscribeFromTripChangesAsync(); // opcjonalnie
        Task SyncPendingChangesAsync();
        Task StartRealtimeAsync();
        Task<bool> IsChannelCreatedAsync();

        event Action<TripDTO, string>? TripChanged;
    }
}
