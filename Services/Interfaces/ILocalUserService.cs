using Packo.Models.SQLite;

namespace Packo.Services.Interfaces
{
    public interface ILocalUserService
    {
        //Task<SQLiteUser> GetOrCreateAsync();
        Task<SQLiteUser?> GetLocalUserAsync();
        Task<SQLiteUser> CreateLocalUserAsync();
        Task LinkSupabaseUserAsync(string supabaseUserId);
    }
}
