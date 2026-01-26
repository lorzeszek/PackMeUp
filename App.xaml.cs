using PackMeUp.Models.SQLite;
using PackMeUp.Services.Interfaces;
using SQLite;

namespace PackMeUp
{
    public partial class App : Application
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ISessionService _sessionService;
        private readonly SQLiteAsyncConnection _db;

        public App(ISupabaseService supabaseService, ISessionService sessionService, SQLiteAsyncConnection db)
        {
            InitializeComponent();
            _supabaseService = supabaseService;
            _sessionService = sessionService;
            _db = db;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            _ = InitializeAppAsync();

            return new Window(new AppShell());
        }

        private async Task InitializeAppAsync()
        {
            await _sessionService.InitializeAsync();
            await _db.CreateTableAsync<SQLiteTrip>();
            await _db.CreateTableAsync<SQLitePackingItem>();
            await _supabaseService.InitializeAsync();
        }
    }
}
