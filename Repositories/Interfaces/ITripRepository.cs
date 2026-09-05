using Packo.Models.DTO;
using Packo.Models.SQLite;
using Packo.Repositories.Models;

namespace Packo.Repositories.Interfaces
{
    public interface ITripRepository
    {
        Task<int> AddTripAsync(TripDTO trip);
        Task<TripDTO?> GetTripAsync(TripDTO trip);
        Task<TripDTO?> GetLocalTripAsync(int localTripId, string localUserId);
        Task UpdateTripAsync(TripDTO trip);
        Task<IReadOnlyList<string>> GetActiveDestinationsAsync();
        Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync();
        Task DeleteTripAsync(TripDTO trip);
        Task DeleteLocalTrips();
        Task UnsubscribeFromTripChangesAsync(); // opcjonalnie
        Task SyncPendingChangesAsync();
        Task StartRealtimeAsync();
        Task<bool> IsChannelCreatedAsync();
        Task SaveDocAsync(TripDocumentDTO document);
        Task<List<SQLiteTripDocument>> GetByTripDocsAsync();
        Task DeleteDocAsync(Guid id);
        event Action<TripDTO, string>? TripChanged;
    }
}
