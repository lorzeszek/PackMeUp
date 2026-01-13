using PackMeUp.Models;
using PackMeUp.Models.SQLite;
using PackMeUp.Repositories.Enums;
using PackMeUp.Repositories.Interfaces;
using SQLite;

namespace PackMeUp.Repositories.Local
{
    public class LocalPackingItemRepository : IPackingItemRepository
    {
        private readonly SQLiteAsyncConnection _db;

        public event Action<PackingItemChange>? PackingItemChanged;

        public LocalPackingItemRepository(SQLiteAsyncConnection db)
        {
            _db = db;
            //_db.CreateTableAsync<PackingItem>();
        }

        public async Task AddPackingItemAsync(PackingItem item)
        {
            var localPackingItem = new SQLitePackingItem()
            {
                SupabaseItemId = item.Id,
                TripId = item.TripId,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                User_id = item.User_id,
                IsPacked = item.IsPacked,
                Category = item.Category,
            };

            await _db.InsertAsync(localPackingItem);
        }

        public async Task DeletePackingItemAsync(PackingItem item)
        {
            var localPackingItem = new SQLitePackingItem()
            {
                SupabaseItemId = item.Id,
                TripId = item.TripId,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                User_id = item.User_id,
                IsPacked = item.IsPacked,
                Category = item.Category,
            };

            await _db.DeleteAsync(localPackingItem);
        }

        public async Task UpdatePackingItemAsync(PackingItem item)
        {
            var localPackingItem = new SQLitePackingItem()
            {
                SupabaseItemId = item.Id,
                TripId = item.TripId,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                User_id = item.User_id,
                IsPacked = item.IsPacked,
                Category = item.Category,
            };

            await _db.UpdateAsync(localPackingItem);
        }

        public async Task<IReadOnlyList<PackingItem>> GetPackingItemsForTripAsync(int tripId)
        {
            var sqlitePackingItem = await _db.Table<SQLitePackingItem>().Where(x => x.TripId == tripId).ToListAsync();

            return sqlitePackingItem.Select(x => new PackingItem
            {
                TripId = x.TripId,
                Id = x.SupabaseItemId,
                Name = x.Name,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                User_id = x.User_id,
                IsPacked = x.IsPacked,
                Category = x.Category,
            }).ToList();
        }

        public Task SubscribeToPackingItemChangesAsync(int tripId, Action<PackingItemChange> onChange)
        {
            // brak działania w lokalnym repo
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromPackingItemChangesAsync() => Task.CompletedTask;

        public Task SyncPendingChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task StartRealtimeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
