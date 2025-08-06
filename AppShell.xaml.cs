using PackMeUp.Views;

namespace PackMeUp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(TripListPage), typeof(TripListPage));
            Routing.RegisterRoute(nameof(PackingListPage), typeof(PackingListPage));
        }
    }
}
