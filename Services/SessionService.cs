using PackMeUp.Services.Interfaces;
using Supabase.Gotrue;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackMeUp.Services
{
    public class SessionService : ISessionService, INotifyPropertyChanged
    {
        public readonly ISupabaseService _supabase;

        public bool IsLoggedIn => User != null;

        private string? _userId;
        public string? UserId
        {
            get => _userId;
            private set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

        private User? _user;
        public User? User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        public SessionService(ISupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task InitializeAsync()
        {
            await TryRestoreSessionAsync();

            _supabase.Client.Auth.AddStateChangedListener(async (sender, e) =>
            {
                var currentSession = _supabase.Client.Auth.CurrentSession;

                if (currentSession != null)
                {
                    if (!string.IsNullOrEmpty(currentSession.AccessToken))
                        await SecureStorage.SetAsync("access_token", currentSession.AccessToken);

                    if (!string.IsNullOrEmpty(currentSession.RefreshToken))
                        await SecureStorage.SetAsync("refresh_token", currentSession.RefreshToken);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        SetUser(currentSession.User);
                    });
                }
                else
                {
                    SecureStorage.Remove("access_token");
                    SecureStorage.Remove("refresh_token");

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ClearUser();
                    });
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

        private async Task TryRestoreSessionAsync()
        {
            try
            {
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                var accessToken = await SecureStorage.GetAsync("access_token");

                if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
                    return;

                _ = _supabase.Client.Auth.SetSession(accessToken, refreshToken, true);

                var user = await _supabase.Client.Auth.GetUser(accessToken);

                if (user == null)
                    throw new Exception("User returned null");

                SetUser(User);
            }
            catch (Exception ex)
            {
                // Refresh się nie udał → usuwamy stary token
                SecureStorage.Remove("access_token");
                SecureStorage.Remove("refresh_token");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
