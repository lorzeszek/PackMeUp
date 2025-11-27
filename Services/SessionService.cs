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
            var user = _supabase.Client.Auth.CurrentUser;

            if (user != null)
            {
                SetUser(user);
            }

            _supabase.Client.Auth.AddStateChangedListener(async (sender, e) =>
            {
                var currentUser = _supabase.Client.Auth.CurrentUser;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (currentUser != null)
                    {
                        SetUser(currentUser);
                    }
                    else
                    {
                        ClearUser();
                    }
                });
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
