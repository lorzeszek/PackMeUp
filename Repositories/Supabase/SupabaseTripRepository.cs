using PackMeUp.Helpers;
using PackMeUp.Models;
using PackMeUp.Models.DTO;
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
                await RealtimeSubscriptionHelper.SubscribeTableChanges<Trip>(
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

        public async Task AddTripAsync(TripDTO trip)
        {
            try
            {
                Trip mappedTrip = Mappers.MapToTrip(trip);

                await _supabase.Client.From<Trip>().Insert(mappedTrip);
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
                Trip mappedTrip = Mappers.MapToTrip(trip);

                await _supabase.Client.From<Trip>().Delete(mappedTrip);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Trip?> GetTripAsync(TripDTO trip)
        {
            try
            {
                Trip mappedTrip = Mappers.MapToTrip(trip);

                var response = await _supabase.Client.From<Trip>().Where(x => x.Id == mappedTrip.Id).Get();

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
                Trip mappedTrip = Mappers.MapToTrip(trip);

                await _supabase.Client
                        .From<Trip>()
                        .Update(mappedTrip);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TripItemsStats>> GetTripStatsAsync()
        {
            var response = await _supabase.Client.Rpc("count_items_stats_for_all_trips", null);

            if (response?.Content == null)
                return new List<TripItemsStats>();

            return JsonSerializer.Deserialize<List<TripItemsStats>>(response.Content)
                   ?? new List<TripItemsStats>();
        }

        public async Task<IReadOnlyList<Trip>> GetActiveTripsAsync()
        {
            var response = await _supabase.Client
                .From<Trip>()
                .Where(x => x.IsActive && !x.IsInTrash)
                .Get();

            return response.Models ?? new List<Trip>();
        }

        public async Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync()
        {
            // 1. Pobranie aktywnych tripów
            var tripsResponse = await _supabase.Client
                    .From<Trip>()
                    .Select("*")
                    .Where(x => x.IsActive == true && x.IsInTrash == false)
                    .Get();

            var trips = tripsResponse.Models ?? new List<Trip>();

            // 2. Wywołanie RPC do pobrania statystyk
            var statsResponse = await _supabase.Client.Rpc("count_items_stats_for_all_trips", new { });

            var stats = statsResponse?.Content == null
                ? new List<TripItemsStats>()
                : JsonSerializer.Deserialize<List<TripItemsStats>>(statsResponse.Content)
                  ?? new List<TripItemsStats>();

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
