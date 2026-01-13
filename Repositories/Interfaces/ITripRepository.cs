using PackMeUp.Models;
using PackMeUp.Repositories.Models;

namespace PackMeUp.Repositories.Interfaces
{
    public interface ITripRepository
    {
        Task AddTripAsync(Trip trip);
        Task<Trip?> GetTripAsync(Trip trip);
        Task UpdateTripAsync(Trip trip);
        Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync();
        Task DeleteTripAsync(Trip trip);
        Task UnsubscribeFromTripChangesAsync(); // opcjonalnie
        Task SyncPendingChangesAsync();
        Task StartRealtimeAsync();

        event Action<Trip, string>? TripChanged;
    }
}
