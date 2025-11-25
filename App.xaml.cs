using PackMeUp.Services;
using static Supabase.Gotrue.Constants;

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

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            if (uri != null && uri.Scheme == "packmeup" && uri.Host == "auth-callback")
            {
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var idToken = query["access_token"]; // lub inny parametr z URL
                var refreshToken = query["refresh_token"];

                _ = Task.Run(async () =>
                {
                    try
                    {
                        var user = await _supabaseService.Client.Auth.SignInWithIdToken(Provider.Google, idToken);
                        // Teraz user jest zalogowany
                        //await MainThread.InvokeOnMainThreadAsync(() =>
                        //    Application.Current.MainPage.DisplayAlert("Sukces", $"Zalogowano jako {user.Email}", "OK"));
                    }
                    catch (Exception ex)
                    {
                        //await MainThread.InvokeOnMainThreadAsync(() =>
                        //    Application.Current.MainPage.DisplayAlert("Błąd", ex.Message, "OK"));
                    }
                });
            }
        }
    }
}
