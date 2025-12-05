using Supabase.Gotrue;

namespace PackMeUp.Services.Interfaces
{
    public interface ISessionService
    {
        string? UserId { get; }
        User? User { get; }
        bool IsLoggedIn { get; }
        Task InitializeAsync();
        void SetUser(User? user);
        void ClearUser();
    }
}
