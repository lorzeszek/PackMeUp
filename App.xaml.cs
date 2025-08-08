using PackMeUp.Services;

namespace PackMeUp
{
    public partial class App : Application
    {
        public App(ISupabaseService supabaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            Task.Run(async () => await supabaseService.InitializeAsync());
        }
    }
}
