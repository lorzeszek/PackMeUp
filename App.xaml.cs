using PackMeUp.Helpers;
using PackMeUp.Models.SQLite;
using PackMeUp.Repositories.Models;
using PackMeUp.Services.Interfaces;
using PackMeUp.Views;
using SQLite;
using System.ComponentModel;

namespace PackMeUp
{
    public partial class App : Application
    {
        private readonly ISessionService _sessionService;
        private readonly SQLiteAsyncConnection _db;
        private readonly ILocalUserService _localUserService;
        private LoadingPage? _loadingPage;

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

            MainThread.BeginInvokeOnMainThread(async () => await UpdateLoadingModalAsync(_sessionService.GlobalIsBusy));
        }

        private async Task UpdateLoadingModalAsync(bool show)
        {
            if (MainPage is null)
                return;

            var navigation = Shell.Current?.Navigation ?? MainPage.Navigation;

            if (show)
            {
                if (_loadingPage != null)
                    return;

                _loadingPage = new LoadingPage();
                await navigation.PushModalAsync(_loadingPage, false);
                return;
            }

            if (_loadingPage is null)
                return;

            try
            {
                if (navigation.ModalStack.LastOrDefault() == _loadingPage)
                {
                    await navigation.PopModalAsync(false);
                }
                else if (navigation.ModalStack.Contains(_loadingPage))
                {
                    await _loadingPage.Navigation.PopModalAsync(false);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _loadingPage = null;
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
            //SecureStorage.Remove("access_token");
            //SecureStorage.Remove("refresh_token");

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLiteTrip");
            await _db.CreateTableAsync<SQLiteTrip>();

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePackingItem");
            await _db.CreateTableAsync<SQLitePackingItem>();

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLiteUser");
            await _db.CreateTableAsync<SQLiteUser>();

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePendingTripChange");
            await _db.CreateTableAsync<SQLitePendingTripChange>();

            //await _db.ExecuteAsync("DROP TABLE IF EXISTS SQLitePendingPackingItemChange");
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