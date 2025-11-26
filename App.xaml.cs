using PackMeUp.Services;

namespace PackMeUp
{
    public partial class App : Application
    {
        private readonly ISupabaseService _supabaseService;

        public App(ISupabaseService supabaseService)
        {
            InitializeComponent();
            _supabaseService = supabaseService;

            //MainPage = new AppShell();

            //Task.Run(async () => await supabaseService.InitializeAsync());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Ustawiamy główną stronę tutaj
            var window = new Window(new AppShell());

            // Inicjalizacja supabase w tle
            _ = Task.Run(async () => await _supabaseService.InitializeAsync());

            return window;
        }
    }
}
