using PackMeUp.Helpers;
using PackMeUp.Models;
using PackMeUp.Repositories.Enums;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;
using Supabase.Realtime;
using System.Reactive.Linq;

namespace PackMeUp.Repositories.Supabase
{
    public class SupabasePackingItemRepository : IPackingItemRepository
    {
        public readonly ISupabaseService _supabase;
        private readonly List<RealtimeChannel> _channels = new();
        //private int? _currentTripId;
        private bool _isSubscribed;
        public event Action<PackingItemChange>? PackingItemChanged;

        public SupabasePackingItemRepository(ISupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task AddPackingItemAsync(PackingItem item)
        {
            try
            {
                await _supabase.Client.From<PackingItem>().Insert(item);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeletePackingItemAsync(PackingItem item)
        {
            try
            {
                await _supabase.Client.From<PackingItem>().Delete(item);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IReadOnlyList<PackingItem>> GetPackingItemsForTripAsync(int tripId)
        {
            try
            {
                //await _supabase.EnsureRealtimeConnectedAsync();

                var result = await _supabase.Client
                    .From<PackingItem>()
                    .Where(x => x.TripId == tripId)
                    .Get();

                return result.Models;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task StartRealtimeAsync()
        {
            if (_isSubscribed)
                return;

            _isSubscribed = true;

            await _supabase.EnsureRealtimeConnectedAsync();

            _channels.AddRange(
                await RealtimeSubscriptionHelper.SubscribeTableChanges<PackingItem>(
                    _supabase.Client,

                    onInsert: item =>
                        PackingItemChanged?.Invoke(
                            new PackingItemChange(PackingItemChangeType.Insert, item)
                        ),

                    onUpdate: item =>
                        PackingItemChanged?.Invoke(
                            new PackingItemChange(PackingItemChangeType.Update, item)
                        ),

                    onDelete: item =>
                        PackingItemChanged?.Invoke(
                            new PackingItemChange(PackingItemChangeType.Delete, item)
                        )
                )
            );
        }

        public Task SyncPendingChangesAsync()
        {
            return Task.CompletedTask;
        }

        public async Task UnsubscribeFromPackingItemChangesAsync()
        {
            if (_channels.Count == 0)
                return;

            foreach (var channel in _channels)
            {
                channel.Unsubscribe();
            }

            _channels.Clear();
            //_currentTripId = null;
        }

        public async Task UpdatePackingItemAsync(PackingItem item)
        {
            try
            {
                //await _supabase.EnsureRealtimeConnectedAsync();
                await _supabase.Client.From<PackingItem>().Update(item);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
