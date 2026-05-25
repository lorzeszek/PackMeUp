using CommunityToolkit.Maui.Extensions;
using PackMeUp.Helpers;
using PackMeUp.Models.SQLite;
using PackMeUp.Popups;
using PackMeUp.Repositories.Models;
using PackMeUp.Services.Interfaces;
using SQLite;
using System.ComponentModel;

namespace PackMeUp
{
    public partial class App : Application
    {
        private readonly ISessionService _sessionService;
        private readonly SQLiteAsyncConnection _db;
        private readonly ILocalUserService _localUserService;
        private BusyPopup? _busyPopup;
        public App(ISessionService sessionService, SQLiteAsyncConnection db, ILocalUserService localUserService)
        {
            InitializeComponent();
            AppThemeManager.ApplySavedTheme();
            _sessionService = sessionService;
            _db = db;
            _localUserService = localUserService;

            if (_sessionService is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += OnSessionPropertyChanged;
            }
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ISessionService.GlobalIsBusy))
                return;

            var isBusy = _sessionService.GlobalIsBusy;
            MainThread.BeginInvokeOnMainThread(async () => await UpdateBusyPopupAsync(isBusy));
        }

        private async Task UpdateBusyPopupAsync(bool show)
        {
            var mainPage = MainPage;
            if (mainPage == null)
                return;

            if (show)
            {
                if (_busyPopup != null)
                    return;

                _busyPopup = new BusyPopup();
                mainPage.ShowPopup(_busyPopup);
                return;
            }

            if (_busyPopup != null)
            {
                try
                {
                    await _busyPopup.CloseAsync();
                }
                catch (Exception)
                {
                }

                _busyPopup = null;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var bootstrapPage = new ContentPage(); // pusta strona
            var window = new Window(bootstrapPage);

            _ = InitializeAndNavigateAsync();

            return window;
        }

        private async Task InitializeAndNavigateAsync()
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");

            await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLiteTrip");
            await _db.CreateTableAsync<SQLiteTrip>();

            await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePackingItem");
            await _db.CreateTableAsync<SQLitePackingItem>();

            await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLiteUser");
            await _db.CreateTableAsync<SQLiteUser>();

            await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePendingTripChange");
            await _db.CreateTableAsync<SQLitePendingTripChange>();

            await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePendingPackingItemChange");
            await _db.CreateTableAsync<SQLitePendingPackingItemChange>();

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
                    await Shell.Current.GoToAsync("//Home");
                }
            });
        }
    }
}