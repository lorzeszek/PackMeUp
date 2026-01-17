using PackMeUp.Models;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Repositories.Models;
using SQLite;
using System.Text.Json;

namespace PackMeUp.Repositories
{
    public class SyncTripRepository : ITripRepository
    {
        private readonly ITripRepository _local;
        private readonly ITripRepository _remote;
        private readonly SQLiteAsyncConnection _pendingDb;

        public event Action<Trip, string>? TripChanged
        {
            add => _remote.TripChanged += value;
            remove => _remote.TripChanged -= value;
        }

        public SyncTripRepository(ITripRepository local, ITripRepository remote, SQLiteAsyncConnection pendingDb)
        {
            _local = local;
            _remote = remote;
            _pendingDb = pendingDb;
        }

        // Wyłączenie subskrypcji Realtime
        public async Task UnsubscribeFromTripChangesAsync()
        {
            if (_remote is null) return;
            await _remote.UnsubscribeFromTripChangesAsync();
        }

        public async Task AddTripAsync(Trip trip)
        {
            await _local.AddTripAsync(trip);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _remote.AddTripAsync(trip);
                }
                catch
                {
                    var pending = new PendingTripChange
                    {
                        //TripId = trip.Id,
                        Operation = "Add",
                        TripJson = JsonSerializer.Serialize(trip)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }
            else
            {
                var pending = new PendingTripChange
                {
                    //TripId = trip.Id,
                    Operation = "Add",
                    TripJson = JsonSerializer.Serialize(trip)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task DeleteTripAsync(Trip trip)
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
                    var pending = new PendingTripChange
                    {
                        //TripId = trip.Id,
                        Operation = "Delete",
                        TripJson = JsonSerializer.Serialize(trip)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }
            else
            {
                var pending = new PendingTripChange
                {
                    //TripId = trip.Id,
                    Operation = "Delete",
                    TripJson = JsonSerializer.Serialize(trip)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task<Trip?> GetTripAsync(Trip trip)
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
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

        public async Task UpdateTripAsync(Trip trip)
        {
            await _local.UpdateTripAsync(trip);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _remote.UpdateTripAsync(trip);
                }
                catch
                {
                    var pending = new PendingTripChange
                    {
                        //TripId = trip.Id,
                        Operation = "Update",
                        TripJson = JsonSerializer.Serialize(trip)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }
            else
            {
                var pending = new PendingTripChange
                {
                    //TripId = trip.Id,
                    Operation = "Update",
                    TripJson = JsonSerializer.Serialize(trip)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task SyncPendingChangesAsync()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            var pendingChanges = await _pendingDb.Table<PendingTripChange>().ToListAsync();

            foreach (var change in pendingChanges)
            {
                var trip = JsonSerializer.Deserialize<Trip>(change.TripJson);

                try
                {
                    switch (change.Operation)
                    {
                        case "Add":
                            await _remote.AddTripAsync(trip!);
                            break;
                        case "Update":
                            await _remote.UpdateTripAsync(trip!);
                            break;
                        case "Delete":
                            await _remote.DeleteTripAsync(trip!);
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
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            //if (true)
            {
                var localTrips = await _local.GetActiveTripsWithStatsAsync();
                //return localTrips.Select(t => new TripWithStats(t, "summary")).ToList();
                return localTrips.ToList();
            }

            //await _supabase.EnsureRealtimeConnectedAsync();

            var trips = await _remote.GetActiveTripsWithStatsAsync();
            //var trips = await _remote.GetActiveTripsAsync();
            //var stats = await _remote.GetTripStatsAsync(); // ← tutaj Twoja funkcja RPC

            //var result = trips.Select(trip =>
            //{
            //    var stat = stats.FirstOrDefault(s => s.TripId == trip.Id);
            //    var summary = stat == null
            //        ? "0 / 0"
            //        : $"{stat.IsPackedCount} / {stat.IsPackedCount + stat.IsNotPackedCount}";

            //    return new TripWithStats(trip, summary);
            //}).ToList();

            //await _local.ReplaceAllAsync(trips);

            return trips.ToList();
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
