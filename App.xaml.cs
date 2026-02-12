using PackMeUp.Models.SQLite;
using PackMeUp.Repositories.Models;
using PackMeUp.Services.Interfaces;
using PackMeUp.Views;
using SQLite;

namespace PackMeUp
{
    public partial class App : Application
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ISessionService _sessionService;
        private readonly SQLiteAsyncConnection _db;
        private readonly ILocalUserService _localUserService;
        public App(ISupabaseService supabaseService, ISessionService sessionService, SQLiteAsyncConnection db, ILocalUserService localUserService)
        {
            InitializeComponent();
            _supabaseService = supabaseService;
            _sessionService = sessionService;
            _db = db;
            _localUserService = localUserService;
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    _ = InitializeAppAsync();

        //    return new Window(new AppShell());
        //}

        //private async Task InitializeAppAsync()
        //{
        //    await _sessionService.InitializeAsync();
        //    await _db.CreateTableAsync<SQLiteTrip>();
        //    await _db.CreateTableAsync<SQLitePackingItem>();
        //    await _supabaseService.InitializeAsync();
        //}

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var bootstrapPage = new ContentPage(); // pusta strona
            var window = new Window(bootstrapPage);

            _ = InitializeAndNavigateAsync();

            return window;
        }

        private async Task InitializeAndNavigateAsync()
        {
            //SecureStorage.Remove("access_token");
            //SecureStorage.Remove("refresh_token");

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLiteTrip");
            await _db.CreateTableAsync<SQLiteTrip>();

            await _db.CreateTableAsync<SQLitePackingItem>();

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLiteUser");
            await _db.CreateTableAsync<SQLiteUser>();

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePendingTripChange");
            await _db.CreateTableAsync<SQLitePendingTripChange>();


            //await _localUserService.GetOrCreateAsync();

            var localUser = await _localUserService.GetLocalUserAsync();

            if (localUser != null)
            {
                _sessionService.SetLocalUser(localUser.LocalUserId);
            }

            await _sessionService.InitializeAsync();

            //if (_sessionService.IsAuthenticated)
            //{
            //    await _supabaseService.InitializeAsync();
            //}

            // 🔥 routing
            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //    MainPage = new AppShell();

            //    //if (_sessionService.IsLoggedIn)
            //    if (true)
            //    {
            //        Shell.Current.GoToAsync("//TripListPage");
            //    }
            //    else
            //    {
            //        Shell.Current.GoToAsync("//StartPage");
            //    }
            //});

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                MainPage = new AppShell();

                //if (!_sessionService.IsAuthenticated)
                //{
                //    // overlay flow (onboarding)
                //    await Shell.Current.GoToAsync(nameof(StartPage));
                //}

                if (_sessionService.HasLocalUser)
                {
                    await Shell.Current.GoToAsync("//TripList");
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(StartPage));
                }
            });
        }
    }
}