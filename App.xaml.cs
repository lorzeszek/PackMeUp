using PackMeUp.Services.Interfaces;

namespace PackMeUp
{
    public partial class App : Application
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ISessionService _sessionService;

        public App(ISupabaseService supabaseService, ISessionService sessionService)
        {
            InitializeComponent();
            _supabaseService = supabaseService;
            _sessionService = sessionService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            _ = InitializeAppAsync();

            return new Window(new AppShell());
        }

        private async Task InitializeAppAsync()
        {
            await _sessionService.InitializeAsync();

            _ = _supabaseService.InitializeAsync();
        }
    }
}
