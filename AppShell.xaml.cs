using PackMeUp.Views;

namespace PackMeUp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Routing.RegisterRoute(nameof(TripListPage), typeof(TripListPage));
            Routing.RegisterRoute(nameof(TripSetupPage), typeof(TripSetupPage));
            Routing.RegisterRoute(nameof(PackingListPage), typeof(PackingListPage));

            Navigating += OnShellNavigating;
        }

        private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            if (e.Source == ShellNavigationSource.ShellSectionChanged
                && Current.Navigation.NavigationStack.Count > 1)
            {
                var deferral = e.GetDeferral();
                await Current.Navigation.PopToRootAsync(animated: false);
                deferral.Complete();
            }
        }
    }
}
