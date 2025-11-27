using CommunityToolkit.Mvvm.ComponentModel;
using PackMeUp.Services.Interfaces;
using Supabase.Realtime.Interfaces;

namespace PackMeUp.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject, IQueryAttributable //: INotifyPropertyChanged
    {
        public readonly ISupabaseService _supabase;
        //public readonly ISessionService Session;

        public ISessionService Session { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public BaseViewModel(ISupabaseService supabase, ISessionService sessionService)
        {
            _supabase = supabase;
            Session = sessionService;

            //RefreshCommand = new Command(async () => await ExecuteRefreshCommand());
        }

        //public virtual ICommand RefreshCommand { get; }

        public IRealtimeChannel? _subscription;

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
    }
}
