using CommunityToolkit.Mvvm.ComponentModel;
using PackMeUp.Services;
using Supabase.Realtime.Interfaces;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject, IQueryAttributable //: INotifyPropertyChanged
    {
        protected readonly ISupabaseService _supabase;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool isRefreshing;

        public BaseViewModel(ISupabaseService supabase)
        {
            _supabase = supabase;

            //RefreshCommand = new Command(async () => await ExecuteRefreshCommand());
        }

        public virtual ICommand RefreshCommand { get; }

        public IRealtimeChannel _subscription;

        /// <summary>
        /// Shell wywoła tę metodę przy wejściu na stronę z parametrami.
        /// </summary>
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            await OnNavigatedToAsync(query);
        }

        /// <summary>
        /// Odpowiednik Prismowego OnNavigatedTo, nadpisuj w swoich ViewModelach.
        /// </summary>
        protected virtual Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            // domyślnie nic nie robi
            return Task.CompletedTask;
        }

        protected virtual async Task ExecuteRefreshCommand()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(500); // symulacja odświeżania
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged([CallerMemberName] string name = null)
        //    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        //protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(backingStore, value))
        //        return false;

        //    backingStore = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        //private bool isBusy = false;
        //public bool IsBusy
        //{
        //    get => isBusy;
        //    set => SetProperty(ref isBusy, value);
        //}

        //private bool isRefreshing;
        //public bool IsRefreshing
        //{
        //    get => isRefreshing;
        //    set => SetProperty(ref isRefreshing, value);
        //}

        //private string title = string.Empty;
        //public string Title
        //{
        //    get => title;
        //    set => SetProperty(ref title, value);
        //}

    }
}
