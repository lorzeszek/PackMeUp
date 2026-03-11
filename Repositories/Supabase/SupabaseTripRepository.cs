using PackMeUp.Helpers;
using PackMeUp.Models.DTO;
using PackMeUp.Models.Supabase;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Repositories.Models;
using PackMeUp.Services.Interfaces;
using Supabase.Realtime;
using System.Text.Json;

namespace PackMeUp.Repositories.Supabase
{
    public class SupabaseTripRepository : ITripRepository
    {
        public readonly ISupabaseService _supabase;
        private readonly List<RealtimeChannel> _channels = new();
        private bool _isSubscribed;
        public event Action<TripDTO, string>? TripChanged;


        public SupabaseTripRepository(ISupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task StartRealtimeAsync()
        {
            if (_isSubscribed)
                return;

            _isSubscribed = true;

            await _supabase.EnsureRealtimeConnectedAsync();

            _channels.AddRange(
                await RealtimeSubscriptionHelper.SubscribeTableChanges<TripSupabase>(
                    _supabase.Client,
                    onInsert: trip => TripChanged?.Invoke(Mappers.MapToTripDTO(trip), "INSERT"),
                    onUpdate: trip => TripChanged?.Invoke(Mappers.MapToTripDTO(trip), "UPDATE"),
                    onDelete: trip => TripChanged?.Invoke(Mappers.MapToTripDTO(trip), "DELETE")
                )
            );
        }

        public async Task<bool> IsChannelCreatedAsync()
        {
            return _channels.Any();
        }

        public async Task UnsubscribeFromTripChangesAsync()
        {
            if (_channels == null) return;

            foreach (var channel in _channels)
            {
                channel.Unsubscribe();
            }

            _isSubscribed = false;
            _channels.Clear();
        }

        public async Task<int> AddTripAsync(TripDTO trip)
        {
            try
            {
                TripSupabase mappedTrip = Mappers.MapToTripSupabase(trip);

                await _supabase.Client.From<TripSupabase>().Insert(mappedTrip);

                return mappedTrip.Id;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        public async Task DeleteTripAsync(TripDTO trip)
        {
            try
            {
                TripSupabase mappedTrip = Mappers.MapToTripSupabase(trip);

                await _supabase.Client.From<TripSupabase>().Delete(mappedTrip);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TripSupabase?> GetTripAsync(TripDTO trip)
        {
            try
            {
                var response = await _supabase.Client.From<TripSupabase>().Where(x => x.Id == trip.RemoteTripId).Get();

                return response.Models.FirstOrDefault();
            }
            catch (Exception)
            {
                // throw new RepositoryException("Failed to delete trip", ex);
                throw;
            }
        }

        public async Task UpdateTripAsync(TripDTO trip)
        {
            try
            {
                TripSupabase mappedTrip = Mappers.MapToTripSupabase(trip);

                await _supabase.Client
                        .From<TripSupabase>()
                        .Update(mappedTrip);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TripItemsStatsSupabase>> GetTripStatsAsync()
        {
            var response = await _supabase.Client.Rpc("count_items_stats_for_all_trips", null);

            if (response?.Content == null)
                return new List<TripItemsStatsSupabase>();

            return JsonSerializer.Deserialize<List<TripItemsStatsSupabase>>(response.Content)
                   ?? new List<TripItemsStatsSupabase>();
        }

        public async Task<IReadOnlyList<TripSupabase>> GetActiveTripsAsync()
        {
            var response = await _supabase.Client
                .From<TripSupabase>()
                .Where(x => x.IsActive && !x.IsInTrash)
                .Get();

            return response.Models ?? new List<TripSupabase>();
        }

        public async Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync()
        {
            // 1. Pobranie aktywnych tripów
            var tripsResponse = await _supabase.Client
                    .From<TripSupabase>()
                    .Select("*")
                    .Where(x => x.IsActive == true && x.IsInTrash == false)
                    .Get();

            var trips = tripsResponse.Models ?? new List<TripSupabase>();

            // 2. Wywołanie RPC do pobrania statystyk
            var statsResponse = await _supabase.Client.Rpc("count_items_stats_for_all_trips", new { });

            var stats = statsResponse?.Content == null
                ? new List<TripItemsStatsSupabase>()
                : JsonSerializer.Deserialize<List<TripItemsStatsSupabase>>(statsResponse.Content)
                  ?? new List<TripItemsStatsSupabase>();

            // 3. Połączenie tripów ze statystykami
            var result = trips.Select(trip =>
            {
                var stat = stats.FirstOrDefault(s => s.TripId == trip.Id);
                var summary = stat == null
                    ? "0 / 0"
                    : $"{stat.IsPackedCount} / {stat.IsPackedCount + stat.IsNotPackedCount}";
                var tripDTO = Mappers.MapToTripDTO(trip);
                return new TripWithStats(tripDTO, summary);
            }).ToList();

            return result;
        }

        public Task SyncPendingChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
