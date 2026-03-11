using Supabase.Gotrue;

namespace PackMeUp.Services.Interfaces
{
    public interface ISessionService
    {
        string? UserId { get; }
        User? User { get; }
        bool IsAuthenticated { get; }
        bool HasLocalUser { get; }
        string? LocalUserId { get; }
        Task InitializeAsync();
        void SetUser(User? user);
        void ClearUser();
        void SetLocalUser(string localUserId);
    }
}
