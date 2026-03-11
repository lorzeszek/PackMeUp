using PackMeUp.Models.SQLite;
using PackMeUp.Services.Interfaces;
using SQLite;

namespace PackMeUp.Services
{
    public class LocalUserService : ILocalUserService
    {
        private readonly SQLiteAsyncConnection _db;

        public LocalUserService(SQLiteAsyncConnection db)
        {
            _db = db;
        }

        public async Task<SQLiteUser> CreateLocalUserAsync()
        {
            var user = new SQLiteUser
            {
                LocalUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            };

            await _db.InsertAsync(user);
            return user;
        }

        public async Task<SQLiteUser?> GetLocalUserAsync()
        {
            return await _db.Table<SQLiteUser>().FirstOrDefaultAsync();
        }

        public async Task<SQLiteUser> GetOrCreateAsync()
        {
            var user = await _db.Table<SQLiteUser>().FirstOrDefaultAsync();

            if (user != null)
                return user;

            user = new SQLiteUser
            {
                LocalUserId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await _db.InsertAsync(user);
            return user;
        }

        public async Task LinkSupabaseUserAsync(string supabaseUserId)
        {
            var user = await _db.Table<SQLiteUser>().FirstAsync();
            user.SupabaseUserId = supabaseUserId;
            await _db.UpdateAsync(user);
        }
    }
}