using PackMeUp.Services.Interfaces;
using Supabase.Gotrue;

namespace PackMeUp.Services
{
    public class SessionService : ISessionService
    {
        public string? UserId { get; private set; }
        public User? User { get; private set; }

        public readonly ISupabaseService _supabase;


        //public SessionService(Supabase.Client client)
        public SessionService(ISupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task InitializeAsync()
        {
            var user = _supabase.Client.Auth.CurrentUser;

            if (user != null)
            {
                SetUser(user);
            }

            _supabase.Client.Auth.AddStateChangedListener((sender, e) =>
            {
                var currentUser = _supabase.Client.Auth.CurrentUser;

                if (currentUser != null)
                {
                    SetUser(currentUser);
                }
                else
                {
                    ClearUser();
                }
            });
        }

        public void SetUser(User? user)
        {
            User = user;
            UserId = user?.Id;
        }

        public void ClearUser()
        {
            User = null;
            UserId = null;
        }
    }
}
