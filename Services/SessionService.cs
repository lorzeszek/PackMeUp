using PackMeUp.Services.Interfaces;
using Supabase.Gotrue;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackMeUp.Services
{
    public class SessionService : ISessionService, INotifyPropertyChanged
    {
        public string? UserId { get; private set; }

        public bool IsLoggedIn => User != null;

        //public User? User { get; private set; }
        private User? _user;
        public User? User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

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

        //public event PropertyChangedEventHandler? PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
