using PackMeUp.Models.Pages;
using PackMeUp.ViewModels;

namespace PackMeUp.Views
{
    public partial class WeatherPage : BasePage
    {
        public WeatherPage(WeatherViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
