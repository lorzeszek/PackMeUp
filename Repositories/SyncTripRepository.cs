using PackMeUp.Models.DTO;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Repositories.Models;
using PackMeUp.Services.Interfaces;
using SQLite;
using System.Text.Json;

namespace PackMeUp.Repositories
{
    public class SyncTripRepository : ITripRepository
    {
        private readonly ITripRepository _local;
        private readonly ITripRepository _remote;
        private readonly SQLiteAsyncConnection _pendingDb;
        private readonly ISessionService _sessionService;

        public event Action<TripDTO, string>? TripChanged
        {
            add => _remote.TripChanged += value;
            remove => _remote.TripChanged -= value;
        }

        public SyncTripRepository(ITripRepository local, ITripRepository remote, ISessionService sessionService, SQLiteAsyncConnection pendingDb)
        {
            _local = local;
            _remote = remote;
            _sessionService = sessionService;
            _pendingDb = pendingDb;
        }

        // Wyłączenie subskrypcji Realtime
        public async Task UnsubscribeFromTripChangesAsync()
        {
            if (_remote is null) return;
            await _remote.UnsubscribeFromTripChangesAsync();
        }

        public async Task<int> AddTripAsync(TripDTO trip)
        {
            int localTripId = await _local.AddTripAsync(trip);

            trip.LocalTripId = localTripId;

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && _sessionService.IsAuthenticated)
            {
                try
                {
                    await _remote.AddTripAsync(trip);
                }
                catch
                {
                    var pending = new SQLitePendingTripChange
                    {
                        LocalUserId = trip.LocalUserId,
                        Operation = "Add",
                        TripJson = JsonSerializer.Serialize(trip)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }
            else
            {
                var pending = new SQLitePendingTripChange
                {
                    LocalUserId = trip.LocalUserId,
                    Operation = "Add",
                    TripJson = JsonSerializer.Serialize(trip)
                };
                await _pendingDb.InsertAsync(pending);
            }

            return localTripId;
        }

        public async Task DeleteTripAsync(TripDTO trip)
        {
            await _local.DeleteTripAsync(trip);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _remote.DeleteTripAsync(trip);
                }
                catch
                {
                    var pending = new SQLitePendingTripChange
                    {
                        Operation = "Delete",
                        TripJson = JsonSerializer.Serialize(trip)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }
            else
            {
                var pending = new SQLitePendingTripChange
                {
                    Operation = "Delete",
                    TripJson = JsonSerializer.Serialize(trip)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task<TripDTO?> GetTripAsync(TripDTO trip)
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && _sessionService.IsAuthenticated)
            {
                var remoteTrip = await _remote.GetTripAsync(trip);
                //if (remoteTrip != null)
                //{
                //    await _local.UpdateTripAsync(remoteTrip); // synchronizacja lokalna
                //}
                return remoteTrip;
            }

            var localTrip = await _local.GetTripAsync(trip);

            return localTrip;
        }

        public async Task UpdateTripAsync(TripDTO trip)
        {
            await _local.UpdateTripAsync(trip);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && _sessionService.IsAuthenticated)
            {
                try
                {
                    await _remote.UpdateTripAsync(trip);
                }
                catch
                {
                    var pending = new SQLitePendingTripChange
                    {
                        Operation = "Update",
                        TripJson = JsonSerializer.Serialize(trip)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }
            else
            {
                var pending = new SQLitePendingTripChange
                {
                    Operation = "Update",
                    TripJson = JsonSerializer.Serialize(trip)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task SyncPendingChangesAsync()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet || !_sessionService.IsAuthenticated)
                return;

            var pendingChanges = await _pendingDb.Table<SQLitePendingTripChange>().Where(x => x.LocalUserId == _sessionService.LocalUserId).ToListAsync();

            foreach (var change in pendingChanges)
            {
                var tripDeserialized = JsonSerializer.Deserialize<TripDTO>(change.TripJson);

                if (tripDeserialized == null)
                    return;

                tripDeserialized.RemoteUserId = _sessionService.UserId;

                try
                {
                    switch (change.Operation)
                    {
                        case "Add":
                            await _remote.AddTripAsync(tripDeserialized!);
                            break;
                        case "Update":
                            await _remote.UpdateTripAsync(tripDeserialized!);
                            break;
                        case "Delete":
                            await _remote.DeleteTripAsync(tripDeserialized!);
                            break;
                    }

                    // Po sukcesie – usuń z kolejki
                    await _pendingDb.DeleteAsync(change);
                }
                catch
                {
                    // Jeśli nie uda się wysłać – zostaje w kolejce
                }
            }
        }



        public async Task<IReadOnlyList<TripWithStats>> GetActiveTripsWithStatsAsync()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet || !_sessionService.IsAuthenticated)
            {
                var localTrips = await _local.GetActiveTripsWithStatsAsync();
                return localTrips.ToList();
            }

            return await _remote.GetActiveTripsWithStatsAsync();
        }

        public Task StartRealtimeAsync()
        {
            // delegujemy – BEZ sprawdzania internetu
            return _remote.StartRealtimeAsync();
        }

        public Task<bool> IsChannelCreatedAsync()
        {
            return _remote.IsChannelCreatedAsync();
        }
    }
}
