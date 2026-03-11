using PackMeUp.Models.DTO;
using PackMeUp.Repositories.Enums;

namespace PackMeUp.Repositories.Interfaces
{
    public interface IPackingItemRepository
    {
        Task<IReadOnlyList<PackingItemDTO>> GetPackingItemsForTripAsync(int localTripId, int remoteTripId);
        Task<int> AddPackingItemAsync(PackingItemDTO item);
        Task UpdatePackingItemAsync(PackingItemDTO item);
        Task DeletePackingItemAsync(PackingItemDTO item);
        Task UnsubscribeFromPackingItemChangesAsync();
        Task SyncPendingChangesAsync();
        Task StartRealtimeAsync();
        Task<bool> IsChannelCreatedAsync();
        Task UpdatePendingPackingItems(int localTripId, int remoteTripId);

        event Action<PackingItemChange>? PackingItemChanged;
    }
}
