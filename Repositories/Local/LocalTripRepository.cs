using PackMeUp.Models.DTO;
using PackMeUp.Models.SQLite;
using PackMeUp.Models.Supabase;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Repositories.Models;
using PackMeUp.Services.Interfaces;
using SQLite;

namespace PackMeUp.Repositories.Local
{
    public class LocalTripRepository : ITripRepository
    {
        private readonly SQLiteAsyncConnection _db;
        private readonly ISessionService _sessionService;


        public event Action<TripDTO, string>? TripChanged;

        public LocalTripRepository(SQLiteAsyncConnection db, ISessionService sessionService)
        {
            _db = db;
            _sessionService = sessionService;
        }

        public Task UnsubscribeFromTripChangesAsync() => Task.CompletedTask;

        public async Task<int> AddTripAsync(TripDTO trip)
        {
            var localTrip = new SQLiteTrip()
            {
                LocalUserId = trip.LocalUserId.ToString(),
                CreatedDate = trip.CreatedDate,
                ModifiedDate = trip.ModifiedDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                IsActive = trip.IsActive,
                IsInTrash = trip.IsInTrash
            };

            await _db.InsertAsync(localTrip);

            return localTrip.LocalTripId;
        }

        public async Task DeleteTripAsync(TripDTO trip)
        {
            var localTrip = new SQLiteTrip()
            {
                LocalTripId = trip.LocalTripId,
                LocalUserId = trip.LocalUserId.ToString(),
                CreatedDate = trip.CreatedDate,
                ModifiedDate = trip.ModifiedDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                IsActive = trip.IsActive,
                IsInTrash = trip.IsInTrash
            };

            await _db.DeleteAsync(localTrip);
        }

        public async Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync()
        {
            var sqliteTrips = await _db.Table<SQLiteTrip>().Where(x => x.LocalUserId == _sessionService.LocalUserId).ToListAsync();
            var sqlitePackingItems = await _db.Table<SQLitePackingItem>().Where(x => x.LocalUserId == _sessionService.LocalUserId).ToListAsync();

            var trips = sqliteTrips.Select(x => new TripDTO
            {
                LocalTripId = x.LocalTripId,
                LocalUserId = x.LocalUserId,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Destination = x.Destination,
                IsActive = x.IsActive,
                IsInTrash = x.IsInTrash
            });

            return trips.Select(tripDTO =>
            {
                var stat = new TripItemsStatsSupabase
                {
                    IsNotPackedCount = sqlitePackingItems.Where(x => x.LocalTripId == tripDTO.LocalTripId && !x.IsPacked).Count(),
                    IsPackedCount = sqlitePackingItems.Where(x => x.LocalTripId == tripDTO.LocalTripId && x.IsPacked).Count()
                };
                var summary = (stat.IsPackedCount + stat.IsNotPackedCount) == 0
                    ? "0 / 0"
                    : $"{stat.IsPackedCount} / {stat.IsPackedCount + stat.IsNotPackedCount}";
                return new TripWithStats(tripDTO, summary);
            }).ToList();

        }

        public async Task<TripDTO?> GetTripAsync(TripDTO trip)
        {
            var sqliteTrip = await _db.Table<SQLiteTrip>().FirstOrDefaultAsync(x => x.LocalUserId == trip.LocalUserId && x.LocalTripId == trip.LocalTripId);

            return new TripDTO
            {
                LocalUserId = sqliteTrip.LocalUserId,
                LocalTripId = sqliteTrip.LocalTripId,
                CreatedDate = sqliteTrip.CreatedDate,
                ModifiedDate = sqliteTrip.ModifiedDate,
                StartDate = sqliteTrip.StartDate,
                EndDate = sqliteTrip.EndDate,
                Destination = sqliteTrip.Destination,
                IsActive = sqliteTrip.IsActive,
                IsInTrash = sqliteTrip.IsInTrash
            };
        }

        public async Task UpdateTripAsync(TripDTO trip)
        {
            var localTrip = new SQLiteTrip()
            {
                LocalUserId = trip.LocalUserId,
                CreatedDate = trip.CreatedDate,
                ModifiedDate = trip.ModifiedDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                IsActive = trip.IsActive,
                IsInTrash = trip.IsInTrash
            };

            await _db.UpdateAsync(localTrip);
        }

        public Task SyncPendingChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task StartRealtimeAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsChannelCreatedAsync()
        {
            return (Task<bool>)Task.CompletedTask;
        }
    }
}
