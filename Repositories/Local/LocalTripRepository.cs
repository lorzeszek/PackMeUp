using PackMeUp.Models;
using PackMeUp.Models.SQLite;
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


        public event Action<Trip, string>? TripChanged;

        public LocalTripRepository(SQLiteAsyncConnection db, ISessionService sessionService)
        {
            _db = db;
            _sessionService = sessionService;
        }

        public Task UnsubscribeFromTripChangesAsync() => Task.CompletedTask;

        public async Task AddTripAsync(Trip trip)
        {
            var localTrip = new SQLiteTrip()
            {
                ClientId = trip.ClientId.ToString(),
                CreatedDate = trip.CreatedDate,
                ModifiedDate = trip.ModifiedDate,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                IsActive = trip.IsActive,
                IsInTrash = trip.IsInTrash
            };

            await _db.InsertAsync(localTrip);
        }

        public async Task DeleteTripAsync(Trip trip)
        {
            var localTrip = new SQLiteTrip()
            {
                ClientId = trip.ClientId.ToString(),
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
            var sqliteTripsAll = await _db.Table<SQLiteTrip>().ToListAsync();
            var sqliteTrips = await _db.Table<SQLiteTrip>().Where(x => x.ClientId == _sessionService.LocalUserId).ToListAsync();

            var trips = sqliteTrips.Select(x => new Trip
            {
                ClientId = x.ClientId,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Destination = x.Destination,
                IsActive = x.IsActive,
                IsInTrash = x.IsInTrash
            });

            return trips.Select(trip =>
            {
                var stat = new TripItemsStats { IsNotPackedCount = 0, IsPackedCount = 0 };
                var summary = stat == null
                    ? "0 / 0"
                    : $"{stat.IsPackedCount} / {stat.IsPackedCount + stat.IsNotPackedCount}";

                return new TripWithStats(trip, summary);
            }).ToList();

        }

        public async Task<Trip?> GetTripAsync(Trip trip)
        {
            var sqliteTrip = await _db.Table<SQLiteTrip>().FirstOrDefaultAsync(x => x.ClientId == trip.ClientId);

            return new Trip
            {
                ClientId = sqliteTrip.ClientId,
                CreatedDate = sqliteTrip.CreatedDate,
                ModifiedDate = sqliteTrip.ModifiedDate,
                StartDate = sqliteTrip.StartDate,
                EndDate = sqliteTrip.EndDate,
                Destination = sqliteTrip.Destination,
                IsActive = sqliteTrip.IsActive,
                IsInTrash = sqliteTrip.IsInTrash
            };
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            var localTrip = new SQLiteTrip()
            {
                ClientId = trip.ClientId,
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
