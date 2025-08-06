using PackMeUp.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackMeUp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly ISupabaseService _supabase;

        public BaseViewModel(ISupabaseService supabase)
        {
            _supabase = supabase;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        private string title = string.Empty;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}
