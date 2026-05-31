using PackMeUp.Helpers;
using PackMeUp.Models.DTO;
using PackMeUp.Models.Supabase;
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
        private readonly ISessionService _sessionService;

        public readonly List<RealtimeChannel> _channels = new();
        private bool _isSubscribed;
        public event Action<PackingItemChange>? PackingItemChanged;

        public SupabasePackingItemRepository(ISupabaseService supabase, ISessionService sessionService)
        {
            _supabase = supabase;
            _sessionService = sessionService;
        }

        public async Task<int> AddPackingItemAsync(PackingItemDTO item)
        {
            try
            {
                PackingItemSupabase mappedPackingItem = Mappers.MapToPackingItemSupabase(item);

                await _supabase.Client.From<PackingItemSupabase>().Insert(mappedPackingItem);

                return mappedPackingItem.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task AddPackingItemsAsync(List<PackingItemDTO> items)
        {
            var localPackingItems = items.Select(x => Mappers.MapToPackingItemSupabase(x)).ToList();

            try
            {
                await _supabase.Client.From<PackingItemSupabase>().Insert(localPackingItems);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeletePackingItemAsync(PackingItemDTO item)
        {
            try
            {
                PackingItemSupabase mappedPackingItem = Mappers.MapToPackingItemSupabase(item);

                await _supabase.Client.From<PackingItemSupabase>().Delete(mappedPackingItem);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IReadOnlyList<PackingItemDTO>> GetPackingItemsForTripAsync(int localTripId, int remoteTripId)
        {
            try
            {
                var result = await _supabase.Client
                    .From<PackingItemSupabase>()
                    .Where(x => x.TripId == remoteTripId)
                    .Get();

                return result.Models.Any() ? result.Models.Select(x => Mappers.MapToPackingItemDTO(x)).ToList() : new List<PackingItemDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task StartRealtimeAsync()
        {
            if (_isSubscribed || (Connectivity.Current.NetworkAccess != NetworkAccess.Internet || !_sessionService.IsAuthenticated))
                return;

            _isSubscribed = true;

            await _supabase.EnsureRealtimeConnectedAsync();

            _channels.AddRange(
                await RealtimeSubscriptionHelper.SubscribeTableChanges<PackingItemSupabase>(
                    _supabase.Client,

                    onInsert: item =>
                        PackingItemChanged?.Invoke(new PackingItemChange(PackingItemChangeType.Insert, Mappers.MapToPackingItemDTO(item))),

                    onUpdate: item =>
                        PackingItemChanged?.Invoke(new PackingItemChange(PackingItemChangeType.Update, Mappers.MapToPackingItemDTO(item))),

                    onDelete: item =>
                        PackingItemChanged?.Invoke(new PackingItemChange(PackingItemChangeType.Delete, Mappers.MapToPackingItemDTO(item)))
                )
            );
        }

        public Task SyncPendingChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task UpdatePendingPackingItems(int localTripId, int remoteTripId)
        {
            return Task.CompletedTask;
        }

        public async Task<bool> IsChannelCreatedAsync()
        {
            return _channels.Any();
        }

        public async Task UnsubscribeFromPackingItemChangesAsync()
        {
            if (_channels.Count == 0)
                return;

            foreach (var channel in _channels)
            {
                channel.Unsubscribe();
            }

            _isSubscribed = false;
            _channels.Clear();
        }

        public async Task UpdatePackingItemAsync(PackingItemDTO item)
        {
            try
            {
                PackingItemSupabase mappedPackingItem = Mappers.MapToPackingItemSupabase(item);
                await _supabase.Client.From<PackingItemSupabase>().Update(mappedPackingItem);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
