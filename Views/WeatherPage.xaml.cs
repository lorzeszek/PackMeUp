using Packo.Models.Pages;
using Packo.ViewModels;

namespace Packo.Views
{
    public partial class WeatherPage : BasePage
    {
        private readonly WeatherViewModel _viewModel;

        public WeatherPage(WeatherViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearingAsync();
        }
    }
}
