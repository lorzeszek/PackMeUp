using PackMeUp.Models;
using PackMeUp.Repositories.Enums;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Repositories.Models;
using SQLite;
using System.Text.Json;

namespace PackMeUp.Repositories
{
    public class SyncPackingItemRepository : IPackingItemRepository
    {
        private readonly IPackingItemRepository _local;
        private readonly IPackingItemRepository _remote;
        private readonly SQLiteAsyncConnection _pendingDb;

        public event Action<PackingItemChange>? PackingItemChanged
        {
            add => _remote.PackingItemChanged += value;
            remove => _remote.PackingItemChanged -= value;
        }

        public SyncPackingItemRepository(IPackingItemRepository local, IPackingItemRepository remote, SQLiteAsyncConnection pendingDb)
        {
            _local = local;
            _remote = remote;
            _pendingDb = pendingDb;
        }

        public async Task AddPackingItemAsync(PackingItem item)
        {
            await _local.AddPackingItemAsync(item);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _remote.AddPackingItemAsync(item);
                }
                catch
                {
                    var pending = new PendingPackingItemChange
                    {
                        //TripId = trip.Id,
                        Operation = "Add",
                        PackingItemJson = JsonSerializer.Serialize(item)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }

            else
            {
                var pending = new PendingPackingItemChange
                {
                    //TripId = trip.Id,
                    Operation = "Add",
                    PackingItemJson = JsonSerializer.Serialize(item)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task DeletePackingItemAsync(PackingItem item)
        {
            await _local.DeletePackingItemAsync(item);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _remote.DeletePackingItemAsync(item);
                }
                catch
                {
                    var pending = new PendingPackingItemChange
                    {
                        //TripId = trip.Id,
                        Operation = "Delete",
                        PackingItemJson = JsonSerializer.Serialize(item)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }

            else
            {
                var pending = new PendingPackingItemChange
                {
                    //TripId = trip.Id,
                    Operation = "Delete",
                    PackingItemJson = JsonSerializer.Serialize(item)
                };
                await _pendingDb.InsertAsync(pending);
            }
        }

        public async Task<IReadOnlyList<PackingItem>> GetPackingItemsForTripAsync(int tripId)
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var packingItems = await _remote.GetPackingItemsForTripAsync(tripId);
                if (packingItems != null)
                {
                    //await _local.UpdatePackingItemAsync(packingItems); // synchronizacja lokalna
                }
                return packingItems;
            }

            var localTrip = await _local.GetPackingItemsForTripAsync(tripId);

            return localTrip;
        }

        public async Task UnsubscribeFromPackingItemChangesAsync()
        {
            if (_remote is null) return;
            await _remote.UnsubscribeFromPackingItemChangesAsync();
        }

        public async Task UpdatePackingItemAsync(PackingItem item)
        {
            await _local.UpdatePackingItemAsync(item);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _remote.UpdatePackingItemAsync(item);
                }
                catch
                {
                    var pending = new PendingPackingItemChange
                    {
                        //TripId = trip.Id,
                        Operation = "Update",
                        PackingItemJson = JsonSerializer.Serialize(item)
                    };
                    await _pendingDb.InsertAsync(pending);
                }
            }

            else
            {
                var pending = new PendingPackingItemChange
                {
                    //TripId = trip.Id,
                    Operation = "Update",
                    PackingItemJson = JsonSerializer.Serialize(item)
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
                var packingItem = JsonSerializer.Deserialize<PackingItem>(change.TripJson);

                try
                {
                    switch (change.Operation)
                    {
                        case "Add":
                            await _remote.AddPackingItemAsync(packingItem!);
                            break;
                        case "Update":
                            await _remote.UpdatePackingItemAsync(packingItem!);
                            break;
                        case "Delete":
                            await _remote.DeletePackingItemAsync(packingItem!);
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

        public Task StartRealtimeAsync()
        {
            return _remote.StartRealtimeAsync();
        }
    }
}