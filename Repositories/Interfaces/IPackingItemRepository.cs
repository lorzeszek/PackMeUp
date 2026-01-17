using PackMeUp.Models;
using PackMeUp.Repositories.Enums;

namespace PackMeUp.Repositories.Interfaces
{
    public interface IPackingItemRepository
    {
        Task<IReadOnlyList<PackingItem>> GetPackingItemsForTripAsync(int tripId);
        Task AddPackingItemAsync(PackingItem item);
        Task UpdatePackingItemAsync(PackingItem item);
        Task DeletePackingItemAsync(PackingItem item);
        Task UnsubscribeFromPackingItemChangesAsync();
        Task SyncPendingChangesAsync();
        Task StartRealtimeAsync();
        Task<bool> IsChannelCreatedAsync();

        event Action<PackingItemChange>? PackingItemChanged;
    }
}
